
using ArmaBrowser.Data.DefaultImpl;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using ArmaBrowser.Data;
using ArmaBrowser.Data.DefaultImpl.Rest;

namespace ArmaBrowser.Logic
{
    class LogicContext : LogicModelBase, ILogicContext
    {
        #region Fields

        private readonly Data.IArma3DataRepository _defaultDataRepository;
        private ObservableCollection<IPEndPoint> _favoritServerEndPoints;
        private readonly ObservableCollection<IServerItem> _serverItems = new ObservableCollection<IServerItem>();
        private ObservableCollection<IAddon> _addons;
        private ServerRepositorySteam _defaultServerRepository;
        //private Data.IArmaBrowserServerRepository _defaultBrowserServerRepository;
        private string _armaPath = null;
        private string _armaVersion = null;
        private Data.IServerVo[] _serverIPListe;

        #endregion Fields

        public LogicContext()
        {
            _defaultDataRepository = Data.DataManager.CreateNewDataRepository();

            _defaultServerRepository = Data.DataManager.CreateNewServerRepository();

            //_defaultBrowserServerRepository = Data.DataManager.CreateNewArmaBrowserServerRepository();
        }

        #region Properties

        public ObservableCollection<IServerItem> ServerItems
        {
            get
            {
                return _serverItems;
            }
        }

        public ObservableCollection<IPEndPoint> FavoritServerEndPoints
        {
            get
            {
                if (_favoritServerEndPoints == null)
                {
                    _favoritServerEndPoints = new ObservableCollection<IPEndPoint>();
                }

                return _favoritServerEndPoints;
            }
        }

        public string ArmaVersion
        {
            get
            {
                if (_armaVersion == null)
                {
                    var folder = Properties.Settings.Default.ArmaPath;
                    if (!string.IsNullOrEmpty(folder) && System.IO.File.Exists(Path.Combine(folder, "arma3.exe")))
                    {
                        var version = System.Diagnostics.FileVersionInfo.GetVersionInfo(Path.Combine(folder, "arma3.exe"));
                        _armaVersion = string.Format("{0}.{1}.{2:000}{3}", version.FileMajorPart, version.FileMinorPart, version.FileBuildPart, version.FilePrivatePart);
                    }
                    else
                        _armaVersion = "unkown";
                }
                return _armaVersion;
            }
        }

        public string ArmaPath
        {
            get
            {
                if (_armaPath == null)
                    _armaPath = _defaultDataRepository.GetArma3Folder();
                return _armaPath;
            }
        }

        public event EventHandler<string> LiveAction;

        #endregion Properties

        public void LookForArmaPath()
        {
            _armaPath = _defaultDataRepository.GetArma3Folder();
            OnPropertyChanged("ArmaPath");
            _armaVersion = null;
            OnPropertyChanged("ArmaVersion");
        }

        class _comp : System.Collections.Generic.IEqualityComparer<object>
        {
            internal static _comp Default = new _comp();

            bool System.Collections.Generic.IEqualityComparer<object>.Equals(object x, object y)
            {
                if (x == null || y == null)
                    return false;

                if (x is Data.IServerQueryAddress)
                {
                    var addr = (Data.IServerQueryAddress)x;
                    var xAddr = (Data.IServerQueryAddress)y;

                    return (addr.Host.ToString() + " " + addr.QueryPort) == (xAddr.Host.ToString() + " " + xAddr.QueryPort);
                }


                var ip = (System.Net.IPEndPoint)x;
                var server = (Data.IServerQueryAddress)y;

                return (ip.Address.ToString() + " " + ip.Port) == (server.Host.ToString() + " " + server.QueryPort);


            }

            public int GetHashCode(object obj)
            {
                var ip = obj as System.Net.IPEndPoint;
                if (ip != null)
                {
                    return (ip.Address.ToString() + " " + ip.Port).GetHashCode();
                }
                var server = obj as Data.IServerQueryAddress;
                if (server != null)
                {
                    return (server.Host.ToString() + " " + server.QueryPort).GetHashCode();
                }
                return obj.GetHashCode();
            }
        }

        public async void ReloadServerItems(IEnumerable<System.Net.IPEndPoint> lastAddresses, CancellationToken cancellationToken)
        {
            var dest = ServerItems;
            var recently = dest.Where(srv => srv.LastPlayed.HasValue).ToArray(); //.Select(srv => new System.Net.IPEndPoint(srv.Host, srv.Port))

            try
            {
                await UiTask.Run(() => dest.Clear(), cancellationToken);
            }
            catch (OperationCanceledException)
            {
                System.Diagnostics.Debug.WriteLine("Reloading canceled");
                return;
            }

            RefreshServerInfoAsync(recently)
                   .ContinueWith(t =>
                   {
                       foreach (var recentlyItem in recently)
                           dest.Add(recentlyItem);

                   }, UiTask.TaskScheduler);


            _serverIPListe = _defaultServerRepository.GetServerList(OnServerGenerated);
            if (lastAddresses.Count() > 0)
            {
                var last = _serverIPListe.Join<Data.IServerVo, System.Net.IPEndPoint, object, Data.IServerVo>(lastAddresses,
                                                                                    o => (object)o,
                                                                                    i => (object)i,
                                                                                    (o, i) => o,
                                                                                    _comp.Default).ToArray();

                //die EndPoints aus dem Argument lastAddresses aus der Gesamtliste zunächst entfernen
                _serverIPListe = _serverIPListe.Except(last).ToArray();

                // die EndPoints aus dem Argument lastAddresses vorn einreihen
                _serverIPListe = last.Union(_serverIPListe).ToArray();
            }

            if (recently.Length > 0)
            {
                var recentlyData = _serverIPListe.Join<Data.IServerVo, IServerItem, object, Data.IServerVo>(recently,
                                                                                    o => (object)o,
                                                                                    i => i,
                                                                                    (o, i) => o,
                                                                                    _comp.Default).ToArray();

                _serverIPListe = _serverIPListe.Except(recentlyData).ToArray();
            }

            if (cancellationToken.IsCancellationRequested)
                return;

            var token = cancellationToken;

            var threadCount = System.Environment.ProcessorCount * 4;

            var blockCount = Convert.ToInt32(Math.Floor(_serverIPListe.Length / (30 * 1d)));

            if (_serverIPListe.Length == 0) return;

            var waitArray = new ManualResetEventSlim[threadCount + 1];

            for (int i = 0; i < threadCount; i++)
            {
                var reset = new ManualResetEventSlim(false);
                waitArray[i] = reset;
                (new Thread(LoadingServerList) { Name = "UDP" + (i + 1), IsBackground = true, Priority = ThreadPriority.Lowest }).Start(new LoadingServerListContext { dest = dest, ips = _serverIPListe.Skip(blockCount * i).Take(blockCount), Reset = reset, token = token });
            }
            // den Rest als extra Thread starten
            var lastreset = new ManualResetEventSlim(false);
            waitArray[waitArray.Length - 1] = lastreset;
            (new Thread(LoadingServerList) { IsBackground = true, Priority = ThreadPriority.Lowest }).Start(new LoadingServerListContext { dest = dest, ips = _serverIPListe.Skip(blockCount * threadCount), Reset = lastreset });

            WaitHandle.WaitAll(waitArray.Select(r => r.WaitHandle).ToArray());

        }

        class LoadingServerListContext
        {
            public IEnumerable<Data.IServerVo> ips { get; set; }
            public Collection<IServerItem> dest { get; set; }
            public CancellationToken token { get; set; }
            public ManualResetEventSlim Reset { get; set; }
        }

        void LoadingServerList(object loadingServerListContext)
        {
            var threadId = Thread.CurrentThread.ManagedThreadId;
            var state = (LoadingServerListContext)loadingServerListContext;
            foreach (var dataItem in state.ips)
            {
                if (state.token.IsCancellationRequested)
                    break;

                var serverQueryEndpoint = new System.Net.IPEndPoint(dataItem.Host, dataItem.QueryPort);

                if (LiveAction != null)
                {
                    LiveAction(this, string.Format("{2,3} {0} {1}", BitConverter.ToString(dataItem.Host.GetAddressBytes()), BitConverter.ToString(BitConverter.GetBytes(dataItem.QueryPort)), threadId));
                }

                var vo = _defaultServerRepository.GetServerInfo(serverQueryEndpoint);

                if (state.token.IsCancellationRequested)
                    break;

                var item = new ServerItem();
                if (vo != null)
                {
                    AssignProperties(item, vo);
                }
                else
                {
                    item.Name = string.Format("{0}:{1}", serverQueryEndpoint.Address, serverQueryEndpoint.Port - 1);
                    item.Host = serverQueryEndpoint.Address;
                    item.QueryPort = serverQueryEndpoint.Port;
                }

                var t = UiTask.Run((dest2, item2) => dest2.Add(item2), state.dest, item);
            }
            state.Reset.Set();
        }

        public void ReloadServerItem(System.Net.IPEndPoint iPEndPoint, CancellationToken cancellationToken)
        {
            var dest = ServerItems;
            try
            {
                UiTask.Run(() => dest.Clear(), cancellationToken).Wait();
            }

            catch (OperationCanceledException)
            {
                System.Diagnostics.Debug.WriteLine("Reloading canceled");
                return;
            }

            var vo = _defaultServerRepository.GetServerInfo(new System.Net.IPEndPoint(iPEndPoint.Address, iPEndPoint.Port + 1));
            if (cancellationToken.IsCancellationRequested || vo == null)
                return;
            var item = new ServerItem();
            AssignProperties(item, vo);

            UiTask.Run((dest2, item2) => dest2.Add(item2), dest, item);
        }

        private void OnServerGenerated(Data.IServerVo obj)
        {
            if (LiveAction != null)
            {
                LiveAction(this, new IPEndPoint(obj.Host, obj.QueryPort).ToString());
            }
        }

        private async Task UpdateServerInfoAsync(IServerItem[] serverItems)
        {
            await Task.Run(() => UpdateServerInfo(serverItems));
        }

        private void UpdateServerInfo(IServerItem[] serverItems)
        {
            foreach (ServerItem serverItem in serverItems)
            {
                if (serverItem == null) return;
                if (serverItem.Host == null) return;
                if (serverItem.QueryPort == 0) return;


                Data.IServerVo dataItem = null;
                try
                {
                    dataItem = _defaultServerRepository.GetServerInfo(new System.Net.IPEndPoint(serverItem.Host, serverItem.QueryPort));
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                }
                if (dataItem == null)
                    continue;

                UiTask.Run(AssignProperties, serverItem, dataItem).Wait();
            }
        }

        void AssignProperties(ServerItem item, Data.IServerVo vo)
        {
            var keyWordsSplited = vo.Keywords.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).ToArray();
            var keyWords = new Dictionary<string, string>(keyWordsSplited.Length);

            foreach (var s in keyWordsSplited)
            {
                var k = s.Substring(0, 1);
                if (!keyWords.ContainsKey(k))
                {
                    keyWords.Add(k, s.Substring(1));
                }
            }

            //serverItem1.Country = dataItem1.Country;
            item.Gamename = vo.Gamename;
            item.Host = vo.Host;
            item.Island = vo.Map;
            item.Mission = vo.Mission;
            item.Mode = keyWords.FirstOrDefault(k => k.Key == "t").Value ?? string.Empty; //dataItem1.Mode;
            item.Modhashs = vo.Modhashs;
            item.ModsText = vo.Mods;
            item.Name = vo.Name;
            item.CurrentPlayerCount = vo.CurrentPlayerCount;
            item.MaxPlayers = vo.MaxPlayers;
            item.Port = vo.Port;
            item.QueryPort = vo.QueryPort;
            item.Signatures = vo.Signatures;
            item.VerifySignatures = vo.VerifySignatures;
            item.Version = vo.Version;
            item.Passworded = keyWords.FirstOrDefault(k => k.Key == "l").Value == "t"; //dataItem1.Passworded;
            item.Ping = vo.Ping;
            item.CurrentPlayersText = string.Join(", ", vo.Players.Select(p => p.Name).OrderBy(s => s));
            item.CurrentPlayers = vo.Players.OrderBy(p => p.Name).ToArray();
        }


        public Collection<IAddon> Addons
        {
            get
            {
                if (_addons == null)
                {
                    _addons = new ObservableCollection<IAddon>();
                    

                    if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                    {
                        _addons.Add(new Addon
                        {
                            Name = "no Sign",

                            DisplayText = string.Format("{0} ({1})", "DisplayText CannotActived", "Name"),

                            ModName = "@modename",
                            Version = "000.00.0",
                            KeyNames = new[] { new AddonKey() { Name = "key" } }
                        });

                        _addons.Add(new Addon
                        {
                            Name = "new Sign",

                            DisplayText = string.Format("{0} ({1})", "DisplayText CanActived", "Name"),

                            ModName = "@modename",
                            Version = "000.00.0",
                            KeyNames = new[] { new AddonKey() { Name = "key" } },
                            CanActived = true
                        });
                    }
                    else
                    {

                        ReloadAddons();

                    }

                    IAddonWebApi addonWebApi = new AddonWebApi();
                    addonWebApi.PostInstalledAddonsKeysAsync(_addons);
                }

                return _addons;
            }
        }

        internal void ReloadAddons()
        {
            if (_addons == null)
                _addons = new ObservableCollection<IAddon>();
            
            _addons.Clear();

            var armaPath = Properties.Settings.Default.ArmaPath;

            ReloadAddons(armaPath, true);
            ReloadAddons(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments, Environment.SpecialFolderOption.DoNotVerify) +
                            System.IO.Path.DirectorySeparatorChar + "Arma 3" + System.IO.Path.DirectorySeparatorChar, true);

            ReloadAddons(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments, Environment.SpecialFolderOption.DoNotVerify) + System.IO.Path.DirectorySeparatorChar + @"ArmaBrowser" + System.IO.Path.DirectorySeparatorChar + "Arma 3" + System.IO.Path.DirectorySeparatorChar + "Addons", false);
        }

        private void ReloadAddons(string path, bool isArmaDefaultPath)
        {

            if (!string.IsNullOrEmpty(path))
            {
                var items = _defaultDataRepository.GetInstalledAddons(path);
                foreach (var item in items)
                {
                    _addons.Add(new Addon
                    {
                        Name = item.Name,

                        DisplayText = string.Format("{0} ({1})", item.DisplayText, item.Name),
                        Path = item.Path,
                        ModName = item.ModName,
                        Version = item.Version,
                        KeyNames = item.KeyNames,
                        IsInstalled = true,
                        IsArmaDefaultPath = isArmaDefaultPath
                    });
                }
            }
        }


        public void Open(IServerItem serverItem, IAddon[] addons, bool runAsAdmin = false)
        {
            var addonArgs = string.Format(" \"-mod={0}\"", string.Join(";", addons.Select(i => i.CommandlinePath).ToArray()));

            var serverArgs = serverItem != null ? string.Format(" -connect={0} -port={1}", serverItem.Host, serverItem.Port) : string.Empty;
            var path = Properties.Settings.Default.ArmaPath;
            var psInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = Path.Combine(path, "arma3battleye.exe"),
                Arguments = " 2 1 1 -skipintro -noSplash -noPause -world=empty " + addonArgs + serverArgs,// -malloc=system
                WorkingDirectory = path,
                UseShellExecute = true,

            };

            if (runAsAdmin)
                psInfo.Verb = "runsas";

            System.Diagnostics.Process ps = null;
            try
            {
                ps = System.Diagnostics.Process.Start(psInfo);
                ps.EnableRaisingEvents = true;
                ps.Exited += ps_Exited;
            }
            catch (Exception)
            {

            }

        }

        //public void TestJson()
        //{
        //    _defaultBrowserServerRepository.Test();
        //}

        void ps_Exited(object sender, EventArgs e)
        {
            //Todo Move to ViewModel Code
            var t = UiTask.Run(() =>
            {
                Application.Current.MainWindow.WindowState = System.Windows.WindowState.Normal;
                Application.Current.MainWindow.Activate();
            });
        }


        public void RefreshServerInfo(IServerItem[] items)
        {
            if (items == null) return;
            UpdateServerInfoAsync(items).Wait();
        }

        public Task RefreshServerInfoAsync(IServerItem[] items)
        {
            return UpdateServerInfoAsync(items);
        }


        internal IServerItem[] AddServerItems(IEnumerable<string> hostQueryAddresses)
        {
            var result = new List<IServerItem>(hostQueryAddresses.Count());
            foreach (var hostQueryAddress in hostQueryAddresses)
            {
                var pos = hostQueryAddress.IndexOf(':');
                var addr = hostQueryAddress.Substring(0, pos);
                int port = 0;
                int.TryParse(hostQueryAddress.Substring(pos + 1), out port);

                var serverItem = new ServerItem() { Host = System.Net.IPAddress.Parse(addr), QueryPort = port, Name = addr };
                ServerItems.Add(serverItem);
                result.Add(serverItem);
            }
            return result.ToArray();
        }

        public async Task<IEnumerable<RestAddonInfoResult>> GetAddonInfosAsync(params string[] addonKeynames)
        {
            return await Task.Run(() =>
            {
                var webapi = new AddonWebApi();
                return webapi.GetAddonInfos(addonKeynames);
            });
        }

        internal void AddAddonUri(IAddon addon, string uri)
        {
            //Task.Run(() =>
            //{
            //    var webapi = new AddonWebApi();
            //    webapi.AddAddonDownloadUri(addon,uri);
            //});
        }

        internal static void UploadAddon(IAddon addon)
        {
            Task.Run(() =>
            {
                var webapi = new AddonWebApi();

                var infos = webapi.GetAddonInfos(addon.KeyNames.Select(k => k.Hash).ToArray());
                if (infos.Count() == 0 || infos.Any(a => a.easyinstall))
                    return;

                webapi.UploadAddon(addon);
            });
        }

        internal static void DownloadAddon(IAddon addon)
        {
            if (addon.IsEasyInstallable && addon.KeyNames.Any() && addon.KeyNames.Any(k => !string.IsNullOrEmpty(k.Hash)))
            {
                var webapi = new AddonWebApi();
                webapi.DownloadAddon(addon.KeyNames.First(k => !string.IsNullOrEmpty(k.Hash)).Hash);

            }
        }
    }
}
