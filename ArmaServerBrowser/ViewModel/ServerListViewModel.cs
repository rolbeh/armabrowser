using ArmaServerBrowser.Logic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ArmaServerBrowser.ViewModel
{
    class ServerListViewModel : LogicModelBase
    {
        #region Fields

        private ILogicContext _context;
        private ListCollectionView _serverItemsView;
        private string _textFilter = string.Empty;
        private IServerItem _selectedServerItem;
        private readonly ObservableCollection<IAddon> _useAddons = new ObservableCollection<IAddon>();
        private IAddon _selectedAddon;
        private uint _loadingBussy;
        private string _armaPath;
        private string _selectedEndPoint;

        private System.Threading.CancellationTokenSource _reloadingCts = new System.Threading.CancellationTokenSource();


        #endregion Fields


        #region Properties

        public Collection<IServerItem> ServerItems
        {
            get { return _context.ServerItems; }
        }

        public Collection<IAddon> Addons
        {
            get { return _context.Addons; }
        }

        public Collection<IAddon> UseAddons
        {
            get { return _useAddons; }
        }

        public ListCollectionView ServerItemsView
        {
            get { return _serverItemsView ?? NewMethod(); }
        }

        private ListCollectionView NewMethod()
        {
            var view = _serverItemsView = new ListCollectionView(ServerItems) { Filter = OnServerItemsFilter };

            view.SortDescriptions.Add(new System.ComponentModel.SortDescription { PropertyName = "Players", Direction = System.ComponentModel.ListSortDirection.Descending });

            return view;
        }

        public string TextFilter
        {
            get { return _textFilter; }
            set
            {
                if (_textFilter == value) return;
                _textFilter = value;
                OnPropertyChanged();
                Reload();
                Properties.Settings.Default.TextFilter = _textFilter;
                ServerItemsView.Refresh();
            }
        }

        public IAddon SelectedAddon
        {
            get { return _selectedAddon; }
            set
            {
                if (_selectedAddon == value) return;
                _selectedAddon = value;
                OnPropertyChanged();
                ServerItemsView.Refresh();
            }
        }

        public IServerItem SelectedServerItem
        {
            get { return _selectedServerItem; }
            set
            {
                if (_selectedServerItem == value) return;
                _selectedServerItem = value;
                _selectedEndPoint = _selectedServerItem == null ? string.Empty : string.Format("{0}:{1}", _selectedServerItem.Host, _selectedServerItem.Port);
                _context.RefreshServerInfo(item: _selectedServerItem);
                RefreshUseAddons();
                OnPropertyChanged();

            }
        }

        private bool _runAsAdmin;
        private bool _launchWithoutHost;

        public bool RunAsAdmin
        {
            get { return _runAsAdmin; }
            set
            {
                _runAsAdmin = value;
                OnPropertyChanged();
            }
        }

        #endregion Properties

        public ServerListViewModel()
        {
            if (Properties.Settings.Default.Hosts == null)
                Properties.Settings.Default.Hosts = new Properties.HostConfigCollection();


            UiTask.Initialize();
            _context = LogicManager.CreateNewLogicContext();

            _textFilter = Properties.Settings.Default.TextFilter;
            _selectedEndPoint = Properties.Settings.Default.LastPlayedHost;

            _armaPath = Properties.Settings.Default.ArmaPath;
            if (string.IsNullOrEmpty(_armaPath))
            {
                _armaPath = _context.ArmaPath;
                Properties.Settings.Default.ArmaPath = _armaPath;
            }
        }

        public async void Reload()
        {
            BeginLoading();
            try
            {
                _reloadingCts.Cancel();
                var oldsrc = _reloadingCts;
                _reloadingCts = new System.Threading.CancellationTokenSource();
                CancellationToken token = _reloadingCts.Token;
                oldsrc.Dispose();

                var lastSelected = _selectedEndPoint;

                await Task.Run(() => ReloadInternal(token), token);

                if (!string.IsNullOrEmpty(lastSelected))
                {
                    SelectedServerItem = ServerItems.FirstOrDefault(s => string.Format("{0}:{1}", s.Host, s.Port) == lastSelected);
                }

            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Reloading canceled");
                return;
            }
            finally
            {
                EndLoading();
            }
        }

        public bool LoadingBussy
        {
            get { return _loadingBussy > 0; }
        }

        private void BeginLoading()
        {
            _loadingBussy++;
            OnPropertyChanged("LoadingBussy");
        }

        private void EndLoading()
        {
            _loadingBussy--;
            if (_loadingBussy < 0) _loadingBussy = 0;
            OnPropertyChanged("LoadingBussy");
        }

        void ReloadInternal(CancellationToken cancellationToken)
        {
            _context.ReloadServerItems(TextFilter, cancellationToken);
            RefreshUseAddons();
        }

        private bool OnServerItemsFilter(object o)
        {
            var result = true;
            var item = (IServerItem)o;
            if (!string.IsNullOrWhiteSpace(_textFilter))
            {
                result = item.FullText.IndexOf(TextFilter, StringComparison.CurrentCultureIgnoreCase) > -1;
            }
            if (result && SelectedAddon != null)
            {
                result = item.Mods.Contains(SelectedAddon.ModName);
            }

            return result;

        }

        private void RefreshUseAddons()
        {
            UiTask.RunInUi(_useAddons.Clear);
            if (_selectedServerItem == null)
            {
                return;
            }

            var hostAddons = _selectedServerItem.Mods;

            var endpoint = string.Format("{0}:{1}", _selectedServerItem.Host, _selectedServerItem.Port);
            var hostCfgItem = Properties.Settings.Default.Hosts.Cast<Properties.HostConfig>().FirstOrDefault(h => h.EndPoint == endpoint);

            if (hostCfgItem != null)
            {
                hostAddons = hostCfgItem.PossibleAddons.Split(';').ToArray();

            }

            var mods = from mod in Addons
                       join selectedMod in hostAddons on mod.ModName equals selectedMod into selectedMods
                       from selectedMod in selectedMods.DefaultIfEmpty()
                       select new { mod, selectedMod };
            foreach (var item in mods)
            {
                item.mod.IsActive = !string.IsNullOrWhiteSpace(item.selectedMod);
                if (item.mod.IsActive)
                    UiTask.RunInUiAsync((list, i) => list.Add(i), _useAddons, item.mod);
            }

        }

        public string ArmaPath
        {
            get
            {
                return _armaPath;
            }
        }

        public bool LaunchWithoutHost
        {
            get { return _launchWithoutHost; }
            set
            {
                _launchWithoutHost = value;
                OnPropertyChanged();
            }
        }

        internal void OpenArma()
        {
            var host = _selectedServerItem;
            if (_launchWithoutHost)
                host = null;
            var endpoint = host != null ? string.Format("{0}:{1}", host.Host, host.Port) : string.Empty;
            var usedAddons = Addons.Where(a => a.IsActive).OrderBy(a => a.ActivationOrder).ToArray();
            var hostCfgItem = Properties.Settings.Default.Hosts.Cast<Properties.HostConfig>().FirstOrDefault(h => h.EndPoint == endpoint);
            if (hostCfgItem == null)
            {
                hostCfgItem = new Properties.HostConfig
                {
                    EndPoint = endpoint,
                    PossibleAddons = string.Empty
                };
                Properties.Settings.Default.Hosts.Add(hostCfgItem);
            }
            hostCfgItem.PossibleAddons = string.Join(";", usedAddons.Select(a => a.ModName).ToArray());


            Properties.Settings.Default.LastPlayedHost = endpoint;
            Properties.Settings.Default.Save();

            _context.Open(host, usedAddons.ToArray());
        }

        internal void TestJson()
        {
            _context.TestJson();
        }
    }
}
