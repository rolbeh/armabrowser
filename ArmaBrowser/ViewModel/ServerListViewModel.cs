using ArmaBrowser.Logic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace ArmaBrowser.ViewModel
{
    class ServerListViewModel : ObjectNotify
    {
        #region Fields

        private readonly LogicContext _context;
        private ListCollectionView _serverItemsView;
        private string _textFilter = string.Empty;
        private System.Net.IPEndPoint _ipEndPointFilter;
        private IServerItem _selectedServerItem;
        private ICollection<LoadingServerListContext> _reloadContexts; 
        private IAddon _selectedAddon;
        private long _loadingBusy;
        private string _selectedEndPoint;
        private bool _runAsAdmin;
        private bool _launchWithoutHost;
        private System.Net.IPEndPoint[] _lastItems = new System.Net.IPEndPoint[0];

        private System.Threading.CancellationTokenSource _reloadingCts = new System.Threading.CancellationTokenSource();
        
        private bool _isJoining;
        private readonly string _version;
        private int _totalPlayerCount;

        #endregion Fields

        #region Properties
        
        public UpdateAvailableViewModel UpdateAvailable { get; }

        public Collection<IServerItem> ServerItems
        {
            get { return _context.ServerItems; }
        }

        public ICollection<LoadingServerListContext> ReloadContexts
        {
            get
            {
                return _reloadContexts ?? (_reloadContexts = new ReadOnlyObservableCollection<LoadingServerListContext>(_context.ReloadThreads));
            }
        }

        public Collection<IAddon> Addons
        {
            get { return _context.Addons; }
        }
        
        public ListCollectionView ServerItemsView
        {
            get { return _serverItemsView ?? CreateServerItemsView(); }
        }

        private ListCollectionView CreateServerItemsView()
        {
            _serverItemsView = new ListCollectionView(ServerItems) { Filter = OnServerItemsFilter };

            _serverItemsView.IsLiveSorting = true;

            // Sorting
            _serverItemsView.SortDescriptions.Add(new System.ComponentModel.SortDescription { PropertyName = "GroupName", Direction = System.ComponentModel.ListSortDirection.Ascending });
            _serverItemsView.SortDescriptions.Add(new System.ComponentModel.SortDescription { PropertyName = "CurrentPlayerCount", Direction = System.ComponentModel.ListSortDirection.Descending });

            // Grouping           
            _serverItemsView.GroupDescriptions.Add(new PropertyGroupDescription("GroupName"));

            return _serverItemsView;
        }

        public string TextFilter
        {
            get { return _textFilter; }
            set
            {
                if (_textFilter == value) return;
                _textFilter = value;

                _ipEndPointFilter = null;

                if (!string.IsNullOrEmpty(_textFilter))
                {
                    _textFilter = _textFilter.Trim();
                    var doppelpunktpos = _textFilter.IndexOf(':');
                    if (doppelpunktpos > 6 && (_textFilter.Length > doppelpunktpos + 4))
                    {
                        var ipstr = _textFilter.Substring(0, doppelpunktpos);
                        var portstr = _textFilter.Substring(doppelpunktpos + 1, _textFilter.Length - doppelpunktpos - 1);
                        System.Net.IPAddress ip;
                        int port;
                        if (System.Net.IPAddress.TryParse(ipstr, out ip)
                                && Int32.TryParse(portstr, out port)
                                && port > System.Net.IPEndPoint.MinPort
                                && port < System.Net.IPEndPoint.MaxPort)
                        {
                            _ipEndPointFilter = new System.Net.IPEndPoint(ip, port);
                        }
                    }
                }

                OnPropertyChanged();

                Properties.Settings.Default.TextFilter = _textFilter;
                ServerItemsView.Refresh();
                RememberLastVisiblyItems();
            }
        }

        private void RememberLastVisiblyItems()
        {
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
                
                RefreshServerInfoAsync(new[] { _selectedServerItem });
                
                RefreshUsedAddons();
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

        #region AddonFolder

        public string ArmaBrowserAddonFolder
        {
            get
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments, Environment.SpecialFolderOption.DoNotVerify) +
                    System.IO.Path.DirectorySeparatorChar + @"ArmaBrowser" + System.IO.Path.DirectorySeparatorChar + "Arma 3" +
                    System.IO.Path.DirectorySeparatorChar + "Addons" + System.IO.Path.DirectorySeparatorChar;
            }
        }

        public string Arma3UserAddonFolder
        {
            get
            {
                return
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments,
                        Environment.SpecialFolderOption.DoNotVerify) +
                    System.IO.Path.DirectorySeparatorChar + "Arma 3" + System.IO.Path.DirectorySeparatorChar;
            }
        }
        
        public string Arma3SteamAddonFolder
        {
            get
            {
                return ArmaPath + System.IO.Path.DirectorySeparatorChar + "!Workshop" + System.IO.Path.DirectorySeparatorChar;
            }
        }

        #endregion

        public ServerListViewModel()
        {
            if (!System.IO.Directory.Exists(ArmaBrowserAddonFolder))
                System.IO.Directory.CreateDirectory(ArmaBrowserAddonFolder);

            UpdateAvailable = new UpdateAvailableViewModel();
            UpdateAvailable.CheckForNewReleases().Wait(0);
            
            _version = this.GetType().Assembly.GetName().Version.ToString();
            
            UiTask.Initialize();
            ModInstallPath[] modFolders =
            {
                new ModInstallPath(ArmaBrowserAddonFolder, false),
                new ModInstallPath(Arma3UserAddonFolder, false)
            };
            _context = new LogicContext(modFolders);
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime) return;

            _context.ServerItems.CollectionChanged += _serverItems_CollectionChanged;           
            _context.PropertyChanged += Context_PropertyChanged;
                
            TextFilter = Properties.Settings.Default.TextFilter;
            _selectedEndPoint = Properties.Settings.Default.LastPlayedHost;

            LookForInstallation();

            string[] recentlyHosts = Properties.Settings.Default.RecentlyHosts.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Reverse().ToArray();
            IServerItem[] recentlyServerItems = _context.AddServerItems(recentlyHosts);
            IServerItem[] favoriteServerItems = ServiceHub.Instance.GetService<FavoriteService>().Get();
            _context.AddServerItems(favoriteServerItems);
            IServerItem[] serverItems = favoriteServerItems.Union(recentlyServerItems).ToArray();
            foreach (var item in recentlyServerItems)
            {
                item.LastPlayed = DateTime.Now;
            }
            var refreshRecentlyServerItemsTask = _context.RefreshServerInfoAsync(serverItems);
            refreshRecentlyServerItemsTask.ContinueWith((t,o) => ServerItemsView.Refresh(), null, TaskScheduler.FromCurrentSynchronizationContext());
            
            Task.Run((Action)EndlessRefreshSelecteItem);
        }

        async void RefreshServerInfoAsync(IServerItem[] serverItems)
        {
            await _context.RefreshServerInfoAsync(serverItems);
            //ServerItemsView.Refresh(); // LostFocus Problem      
        }

        void _serverItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewItems != null)
                        TotalPlayerCount += (e.NewItems.Cast<IServerItem>().First()).CurrentPlayerCount;
                    break;
                case NotifyCollectionChangedAction.Remove:
                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    TotalPlayerCount = 0;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        void Context_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "ArmaVersion": //_context.ArmaVersion
                    OnPropertyChanged("ArmaVersion");
                    break;
            }
        }
        
        public async void ReloadServerList()
        {
            BeginLoading();

            _reloadingCts.Cancel();
            var oldsrc = _reloadingCts;
            _reloadingCts = new System.Threading.CancellationTokenSource();
            CancellationToken token = _reloadingCts.Token;

            oldsrc.Cancel(false);
            try
            {
                await Task.Factory.StartNew(o => ReloadInternal((CancellationToken)o), token, token).
                ContinueWith(t =>
                {
                    if (t.Status == TaskStatus.RanToCompletion)
                        RememberLastVisiblyItems();

                });
            }
            catch (Exception)
            {
                // ignored
            }

            EndLoading();
        }
        
        public bool LoadingBusy
        {
            get { return Interlocked.Read( ref _loadingBusy) > 0; }
        }

        private void BeginLoading()
        {
            Interlocked.Add(ref _loadingBusy, +1L);
            OnPropertyChanged("LoadingBusy");
        }

        private void EndLoading()
        {
            if (Interlocked.Read(ref _loadingBusy) > 0)
                Interlocked.Add(ref _loadingBusy, -1L);
            OnPropertyChanged("LoadingBusy");
        }

        void ReloadInternal(CancellationToken cancellationToken)
        {
            if (_ipEndPointFilter != null)
                _context.ReloadServerItem(_ipEndPointFilter, cancellationToken);
            else
                _context.ReloadServerItems(_lastItems, cancellationToken);
            
            RefreshUsedAddons();
        }

        private bool OnServerItemsFilter(object o)
        {
            var result = true;
            var item = (IServerItem)o;

            if (_ipEndPointFilter != null)
            {
                result = item.Host.Equals(_ipEndPointFilter.Address) && item.Port == _ipEndPointFilter.Port;
                return result;
            }

            if (!string.IsNullOrWhiteSpace(_textFilter))
            {
                var strs = _textFilter.Trim().Split(' ').ToArray();

                foreach (var s in strs)
                {
                    if (s == string.Empty) continue;

                    if (!result)
                        break;

                    if (s[0] == '-')
                    {
                        var s2 = s.Substring(1);
                        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                        result = result && !(item.FullText.IndexOf(s2, StringComparison.CurrentCultureIgnoreCase) > -1);
                        continue;
                    }
                    if (s[0] == '>')
                    {
                        var s2 = s.Substring(1);
                        int playerCount;
                        if (Int32.TryParse(s2, out playerCount))
                        {
                            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                            result = result && item.CurrentPlayerCount > playerCount;
                            continue;
                        }
                    }
                    // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                    result = result && (item.FullText.IndexOf(s, StringComparison.CurrentCultureIgnoreCase) > -1);

                }

            }
            //if (result && _selectedAddon != null)
            //{
            //    result = item.Mods.Contains(_selectedAddon.ModName);
            //}

            return result;

        }

        private async void RefreshUsedAddons()
        {
            var selectedItem = _selectedServerItem;
            
            if (_selectedServerItem == null || _selectedServerItem.Mods == null)
            {
                return;
            }
            //if (_selectedServerItem.Mods != null)
            //{
            int sortNr = 1;
            var hostAddons = _selectedServerItem.Mods.Select(m => new { SortNr = sortNr++, ModName = m });

            var endpoint = string.Format("{0}:{1}", _selectedServerItem.Host, _selectedServerItem.Port);
            var hostCfgItem = HostConfigCollection.Default.Cast<HostConfig>()
                                .FirstOrDefault(h => h.EndPoint == endpoint);

            if (hostCfgItem != null)
            {
                int sortNr2 = 1;
                hostAddons = hostCfgItem.PossibleAddons.Split(';').Select(m => new { SortNr = sortNr2++, ModName = m }).ToArray();
            }

            var mods = (from mod in Addons
                        join selectedMod in hostAddons on mod.ModName equals selectedMod.ModName into selectedMods
                        from selectedMod in selectedMods.DefaultIfEmpty()
                        let sortnr = selectedMod == null ? 0 : selectedMod.SortNr
                        let selectedModName = selectedMod == null ? null : selectedMod.ModName
                        where mod.IsInstalled
                        orderby sortnr
                        select new { mod, selectedMod = selectedModName, Sortnr = sortnr }).ToArray();
            //}

            var s = selectedItem.Signatures ?? string.Empty;
            var hostAddonKeyNames = s.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToArray();

            await _context.UpdateAddonInfos(hostAddonKeyNames);

            // Addons automatisch ab- oder aus- wählen
            foreach (var item in mods)
            {
                item.mod.IsActive = item.mod.IsInstalled && !string.IsNullOrWhiteSpace(item.selectedMod);
               
                var canActive = false;
                if (hostAddonKeyNames.Any())
                {
                    foreach (var addonKey in item.mod.KeyNames.Select(k => k.Name))
                    {
                        if (hostAddonKeyNames.Contains(addonKey))
                        {
                            canActive = true;
                            break;
                        }
                    }
                }
                item.mod.CanActived = canActive;
                if (Properties.Settings.Default.SelectAllAceptedAddons)
                    item.mod.IsActive = canActive;

            }


            //Task<IEnumerable<RestAddonInfoResult>> addonInfosTask = await _context.GetAddonInfosAsync(hostAddonKeyNames);
            //addonInfosTask.ContinueWith(parentTask =>
            //{
            //    if (parentTask.Status != TaskStatus.RanToCompletion) return;

            //    var msgStr = new StringBuilder();

            //    foreach (var addonInfo in parentTask.Result)
            //    {
            //        msgStr.AppendLine(addonInfo.name);

            //        var updAddon =
            //            Addons.FirstOrDefault(
            //                a =>
            //                    a.KeyNames.Any(
            //                        tag => tag.Hash.Equals(addonInfo.hash, StringComparison.OrdinalIgnoreCase)));

            //        //if (!Addons.Any(a => a.ModName.Equals(addonInfo.name, StringComparison.OrdinalIgnoreCase)))
            //        if (updAddon == null)
            //        {
            //            UiTask.Run(a =>
            //            {
            //                Addons.Add(new Addon()
            //                {
            //                    Name = a.name,
            //                    ModName = a.name,
            //                    DisplayText = a.name,
            //                    KeyNames = new[] { new Data.AddonKey { Name = a.keytag, Hash = a.hash} },
            //                    //DownlandUris = new Uri[] { new Uri("http://www.armabrowser.de/"), },
            //                    IsInstalled = false,
            //                    IsEasyInstallable = addonInfo.easyinstall
            //                });
            //            }, addonInfo);
            //        }
            //    }

                //MessageBox.Show(msgStr.ToString());



            //});

        }

        /// <summary>
        /// Steam install folder of Arma3
        /// </summary>
        public string ArmaPath
        {
            get
            {
                return _context.ArmaPath;
            }
        }

        public string ArmaVersion
        {
            get
            {
                return _context.ArmaVersion;
            }
        }

        public bool IsJoinig
        {
            get { return _isJoining; }
            set
            {
                _isJoining = value;
                OnPropertyChanged();
            }
        }

        public string Version
        {
            get { return _version; }
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


        public int TotalPlayerCount
        {
            get { return _totalPlayerCount; }
            set
            {
                _totalPlayerCount = value;
                OnPropertyChanged();
            }
        }

        internal void OpenArma()
        {
            var host = _selectedServerItem;
            if (_launchWithoutHost)
                host = null;

            if (host != null && host.Port <= System.Net.IPEndPoint.MinPort)
                return;

            var endpoint = host != null ? string.Format("{0}:{1}", host.Host, host.Port) : string.Empty;
            var usedAddons = Addons.Where(a => a.IsActive).OrderBy(a => a.ActivationOrder).ToArray();
            var hostCfgItem = HostConfigCollection.Default.Cast<HostConfig>().FirstOrDefault(h => h.EndPoint == endpoint);
            if (hostCfgItem == null)
            {
                hostCfgItem = new HostConfig
                {
                    EndPoint = endpoint,
                    PossibleAddons = string.Empty
                };
                HostConfigCollection.Default.Add(hostCfgItem);
                Properties.Settings.Default.LastPlayedHost = endpoint;
            }
            hostCfgItem.PossibleAddons = string.Join(";", usedAddons.OrderBy(a => a.ActivationOrder).Select(a => a.ModName).ToArray());


            SaveHistory();

            _context.Open(host, usedAddons.ToArray());
        }

        private void SaveHistory()
        {
            if (_selectedServerItem == null) return;

            _selectedServerItem.LastPlayed = DateTime.Now;

            // limit addresses to 10 entries
            {
                var items = _context.ServerItems.Where(srv => srv.LastPlayed.HasValue).OrderByDescending(srv => srv.LastPlayed).Skip(10);
                foreach (var item in items)
                {
                    item.LastPlayed = null;
                }
            }

            // Save histroy addresses
            {
                var items = _context.ServerItems.Where(srv => srv.LastPlayed.HasValue).OrderByDescending(srv => srv.LastPlayed).Select(srv => string.Format("{0}:{1}", srv.Host, srv.QueryPort)).ToArray();
                Properties.Settings.Default.RecentlyHosts = string.Join(" ", items);
            }


            // Save favorits
            {
                var items = _context.ServerItems.Where(srv => srv.IsFavorite).Select(srv => string.Format("{0}:{1}", srv.Host, srv.QueryPort)).ToArray();
                Properties.Settings.Default.Favorits = string.Join(" ", items);
            }

            Properties.Settings.Default.Save();
        }

        //internal void TestJson()
        //{
        //    _context.TestJson();
        //}

        private async void EndlessRefreshSelecteItem()
        {
            while (true)
            {
                var mainwin = await UiTask.Run(() => Application.Current.MainWindow);

                if (mainwin == null) break;
                try
                {
                    var task = UiTask.Run(() => mainwin.WindowState != WindowState.Minimized);
                    task.Wait();
                    if (task.IsCompleted && task.Result && !_isJoining)
                    {
                        var item = SelectedServerItem;
                        if (item != null)
                        {
                            _context.RefreshServerInfo(new[] { item });
                        }
                    }
                    await Task.Delay(3000);
                }
                catch
                {
                    // ignore
                }
            }
        }

        internal void LookForInstallation()
        {
            _context.LookForArmaPath();
            OnPropertyChanged("ArmaPath");
            OnPropertyChanged("ArmaVersion");
            Properties.Settings.Default.ArmaPath = _context.ArmaPath;
        }

        internal void SaveFavorits(IServerItem item)
        {
            if (item.IsFavorite)
            {
                ServiceHub.Instance.GetService<FavoriteService>().Add(item);
            }
            else
            {
                ServiceHub.Instance.GetService<FavoriteService>().Remove(item);
            }
        }

        internal void StopAll()
        {
            _reloadingCts.Cancel();
            _reloadingCts = new System.Threading.CancellationTokenSource();
        }

        internal void RefreshAddons()
        {
            _context.ReloadAddons();
        }

        public void DownloadAddon(IAddon addon)
        {
            _context.DownloadAddonAsync(addon);
        }
    }

}
