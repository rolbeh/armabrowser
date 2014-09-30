using ArmaBrowser.Logic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ArmaBrowser.ViewModel
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
        private uint _loadingBusy;
        private string _armaPath;
        private string _selectedEndPoint;
        private bool _runAsAdmin;
        private bool _launchWithoutHost;
        private IEnumerable<System.Net.IPEndPoint> _lastItems = new System.Net.IPEndPoint[0];

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
            get { return _serverItemsView ?? CreateServerItemsView(); }
        }

        private ListCollectionView CreateServerItemsView()
        {
            _serverItemsView = new ListCollectionView(ServerItems) { Filter = OnServerItemsFilter };

            _serverItemsView.SortDescriptions.Add(new System.ComponentModel.SortDescription { PropertyName = "CurrentPlayerCount", Direction = System.ComponentModel.ListSortDirection.Descending });

            return _serverItemsView;
        }

        public string TextFilter
        {
            get { return _textFilter; }
            set
            {
                if (_textFilter == value) return;
                _textFilter = value;
                OnPropertyChanged();

                Properties.Settings.Default.TextFilter = _textFilter;
                ServerItemsViewRefresh();
            }
        }

        private void ServerItemsViewRefresh()
        {
            ServerItemsView.Refresh();
            _lastItems = ServerItemsView.Cast<IServerItem>().Take(50).Select(i => { return new System.Net.IPEndPoint(i.Host, i.QueryPort); }).ToArray();

        }

        public IAddon SelectedAddon
        {
            get { return _selectedAddon; }
            set
            {
                if (_selectedAddon == value) return;
                _selectedAddon = value;
                OnPropertyChanged();
                //ServerItemsView.Refresh();
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
                _context.RefreshServerInfoAsync(item: _selectedServerItem)
                    .ContinueWith(t => RefreshUseAddons());
                RefreshUseAddons();
                OnPropertyChanged();

            }
        }

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

            _armaPath = _context.ArmaPath;
            Properties.Settings.Default.ArmaPath = _armaPath;

            Task.Run((Action)EndlessRefreshSelecteItem);          
        }

        public void Reload()
        {
            BeginLoading();

            _reloadingCts.Cancel();
            var oldsrc = _reloadingCts;
            _reloadingCts = new System.Threading.CancellationTokenSource();
            CancellationToken token = _reloadingCts.Token;
            //oldsrc.Dispose();

            var lastSelected = _selectedEndPoint;
            
            Task.Factory.StartNew(o => ReloadInternal((CancellationToken)o), token, token)
                .ContinueWith(t =>
                {

                    //if (!string.IsNullOrEmpty(lastSelected))
                    //{
                    //    SelectedServerItem = ServerItems.FirstOrDefault(s => string.Format("{0}:{1}", s.Host, s.Port) == lastSelected);
                    //}
                    EndLoading();
                });

        }

        void Test()
        {
            var lastItems = ServerItemsView.Cast<IServerItem>().Select(i => { return new System.Net.IPEndPoint(i.Host, i.Port); });

        }

        public bool LoadingBusy
        {
            get { return _loadingBusy > 0; }
        }

        private void BeginLoading()
        {
            _loadingBusy++;
            OnPropertyChanged("LoadingBusy");
        }

        private void EndLoading()
        {
            _loadingBusy--;
            if (_loadingBusy < 0) _loadingBusy = 0;
            OnPropertyChanged("LoadingBusy");
        }

        void ReloadInternal(CancellationToken cancellationToken)
        {
            _context.ReloadServerItems(_lastItems, cancellationToken);
            RefreshUseAddons();
        }

        private bool OnServerItemsFilter(object o)
        {
            var result = true;
            var item = (IServerItem)o;
            if (!string.IsNullOrWhiteSpace(_textFilter))
            {
                var strs = _textFilter.Trim().Split(' ').ToArray();
               

                foreach (var s in strs)
                {
                    if (!result)
                        break;

                    if (s[0] =='-')
                    {
                        var s2 = s.Substring(1);
                        result = result && !(item.FullText.IndexOf(s2, StringComparison.CurrentCultureIgnoreCase) > -1);
                        continue;
                    }
                    if (s[0] == '>')
                    {
                        var s2 = s.Substring(1);
                        var playerCount = 0;
                        if (Int32.TryParse(s2,out playerCount))
                        {
                            result = result && item.CurrentPlayerCount > playerCount;
                            continue;
                        }
                    }
                    result = result && (item.FullText.IndexOf(s, StringComparison.CurrentCultureIgnoreCase) > -1);
                }
                
                
            }
            //if (result && _selectedAddon != null)
            //{
            //    result = item.Mods.Contains(_selectedAddon.ModName);
            //}

            return result;

        }

        private void RefreshUseAddons()
        {
            UiTask.Run(_useAddons.Clear);
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

            var s = _selectedServerItem.Signatures ?? string.Empty;
            var hostAddonKeys = s.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToArray();
            foreach (var item in mods)
            {
                item.mod.IsActive = !string.IsNullOrWhiteSpace(item.selectedMod);
                item.mod.CanActived = false;
                if (hostAddonKeys.Any())
                {
                    foreach (var addonKey in item.mod.KeyNames)
                    {
                        if (hostAddonKeys.Contains(addonKey))
                        {
                            item.mod.CanActived = true;
                            break;
                        }
                    }
                }
                if (item.mod.IsActive)
                    UiTask.Run((list, i) => list.Add(i), _useAddons, item.mod);
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

        private async void EndlessRefreshSelecteItem()
        {
            while (true)
            {
                var item = SelectedServerItem;
                if (item != null)
                {
                    _context.RefreshServerInfo(item);
                }
                await Task.Delay(3000);
            }
        }
    }
}
