using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using ArmaBrowser.Data;
using ArmaBrowser.Data.DefaultImpl;
using ArmaBrowser.Data.DefaultImpl.Rest;
using ArmaBrowser.Properties;
using Magic.Steam;

namespace ArmaBrowser.Logic
{
    internal class LogicContext : ObjectNotify, ILogicContext
    {
        public LogicContext()
        {
            _defaultDataRepository = DataManager.CreateNewDataRepository();

            _defaultServerRepository = DataManager.CreateNewServerRepository();

            //_defaultBrowserServerRepository = Data.DataManager.CreateNewArmaBrowserServerRepository();
        }

        public LogicContext(ModInstallPath[] modFolders) : this()
        {
            this._modFolders = modFolders;
        }

        public void LookForArmaPath()
        {
            _armaPath = _defaultDataRepository.GetArma3Folder();
            OnPropertyChanged(nameof(ArmaPath));
            _armaVersion = null;
            OnPropertyChanged(nameof(ArmaVersion));
        }

        public void ReloadServerItems(IPEndPoint[] lastAddresses, CancellationToken cancellationToken)
        {
            var recently = this.ServerItems.Where(srv => srv.LastPlayed.HasValue)
                .Select(a => new Data.DefaultImpl.ServerItem { Host = a.Host, QueryPort = a.QueryPort })
                .ToArray();

            try
            {
                UiTask.Run(this.ServerItems.Clear, cancellationToken: cancellationToken).Wait(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                Trace.WriteLine("Reloading canceled");
                return;
            }

            ISteamGameServer[] lastServer = lastAddresses
                .Select(a => (ISteamGameServer) new Data.DefaultImpl.ServerItem { Host = a.Address, QueryPort = a.Port})
                .ToArray();

            IEnumerable<ISteamGameServer> discoveredServer = _defaultServerRepository.GetServerList()
                                                                    .Except(lastServer, ServerQueryAddressComparer.Default)
                                                                    .Except(recently, ServerQueryAddressComparer.Default);

            var allServer = recently.Union(lastServer.Except(recently, ServerQueryAddressComparer.Default))
                .Union(discoveredServer);

            if (cancellationToken.IsCancellationRequested)
                return;

            var maxParallelCount = Environment.ProcessorCount*4;

            var _serverIPListe = allServer.ToArray();
            var blockCount = Convert.ToInt32(Math.Floor(_serverIPListe.Length / (maxParallelCount * 1d)));

            if (_serverIPListe.Length == 0) return;

            var waitArray = new WaitHandle[maxParallelCount + 1];

            for (var i = 0; i < maxParallelCount; i++)
            {
                LoadingServerListContext threadContext = NewQueryThread(this.ServerItems, cancellationToken, _serverIPListe.Skip(blockCount * i).Take(blockCount).ToArray());
                waitArray[i] = threadContext.Reset;
            }

            // for the rest a single thread
            {
                var threadContext = NewQueryThread(this.ServerItems, cancellationToken, _serverIPListe.Skip(blockCount * waitArray.Length - 1).Take(blockCount).ToArray());
                waitArray[waitArray.Length - 1] = threadContext.Reset;
            }

            WaitHandle.WaitAll(waitArray);
        }

        public void ReloadServerItem(IPEndPoint iPEndPoint, CancellationToken cancellationToken)
        {
            var dest = ServerItems;
            try
            {
                UiTask.Run(dest.Clear, cancellationToken).Wait(cancellationToken);
            }

            catch (OperationCanceledException)
            {
                Trace.WriteLine("Reloading canceled");
                return;
            }

            var vo = _defaultServerRepository.GetServerInfo(new IPEndPoint(iPEndPoint.Address, iPEndPoint.Port + 1));
            if (cancellationToken.IsCancellationRequested || vo == null)
                return;
            var item = new ServerItem();
            AssignProperties(item, vo);

            UiTask.Run((dest2, item2) => dest2.Add(item2), dest, item).Wait(0);
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
                            KeyNames = new[] {new AddonKey {Name = "key"}}
                        });

                        _addons.Add(new Addon
                        {
                            Name = "new Sign",
                            DisplayText = string.Format("{0} ({1})", "DisplayText CanActived", "Name"),
                            ModName = "@modename",
                            Version = "000.00.0",
                            KeyNames = new[] {new AddonKey {Name = "key"}},
                            CanActived = true
                        });
                    }
                    else
                    {
                        ReloadAddons();
                    }
                }

                return _addons;
            }
        }


        public void Open(IServerItem serverItem, IAddon[] addons, bool runAsAdmin = false)
        {
            string addonArgs = string.Empty;
            if (addons.Any())
            {
                addonArgs = $@" ""-mod={string.Join(";", addons.Select(i => i.CommandlinePath).ToArray())}""";
            }

            var serverArgs = serverItem != null
                ? string.Format(" -connect={0} -port={1}", serverItem.Host, serverItem.Port)
                : string.Empty;
            var path = Settings.Default.ArmaPath;
            var psInfo = new ProcessStartInfo
            {
                FileName = Path.Combine(path, "arma3battleye.exe"),
                Arguments = " 2 1 1 -exe Arma3_x64 -skipintro -noSplash -noPause -world=empty " + addonArgs + serverArgs,
                // -malloc=system
                WorkingDirectory = path,
                UseShellExecute = true
            };

            if (runAsAdmin)
                psInfo.Verb = "runsas";

            try
            {
                var ps = Process.Start(psInfo);
                if (ps != null)
                {
                    ps.EnableRaisingEvents = true;
                    ps.Exited += ps_Exited;
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }


        public void RefreshServerInfo(IServerItem[] serverItems)
        {
            if (serverItems == null) return;
            foreach (ServerItem serverItem in serverItems.Cast<ServerItem>())
            {
                if (serverItem?.Host == null) return;
                if (serverItem.QueryPort == 0) return;


                ISteamGameServer dataItem = null;
                try
                {
                    dataItem =
                        _defaultServerRepository.GetServerInfo(new IPEndPoint(serverItem.Host, serverItem.QueryPort));
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
                if (dataItem == null)
                {
                    serverItem.Ping = 999;
                    continue;
                }

                UiTask.Run(AssignProperties, serverItem, dataItem).Wait();
                //AssignProperties(serverItem, dataItem);
            }
        }

        public async Task RefreshServerInfoAsync(IServerItem[] items)
        {
            await Task.Run(() => RefreshServerInfo(items));
        }

        private LoadingServerListContext NewQueryThread(Collection<IServerItem> dest, CancellationToken token, 
            ISteamGameServer[] ips)
        {
            var thread = new Thread(LoadingServerList)
            {
                Name = "UDP " + ReloadThreads.Count + 1,
                IsBackground = true,
                Priority = ThreadPriority.Lowest
            };
            var threadContext = new LoadingServerListContext
            {
                Dest = dest,
                Ips = ips,
                Token = token
            };
            UiTask.Run(ReloadThreads.Add, threadContext).Wait(0);
            thread.Start(threadContext);
            return threadContext;
        }

        private void LoadingServerList(object loadingServerListContext)
        {
            var threadId = Thread.CurrentThread.ManagedThreadId;
            var state = (LoadingServerListContext) loadingServerListContext;
            foreach (var dataItem in state.Ips)
            {
                if (state.Token.IsCancellationRequested)
                    break;

                ServerItem item = AddGameServer(dataItem, state.Dest, state.Token).Result;
                if (item != null)
                {
                    state.ProgressValue++;
                    state.Ping = item.Ping;
                }
            }
            state.Finished();
            Task.Delay(TimeSpan.FromSeconds(0.5))
                .ContinueWith((t,ctx) => ReloadThreads.Remove((LoadingServerListContext)ctx), state, UiTask.UiTaskScheduler)
                .Wait();
            //UiTask.Run(ctx => ReloadThreads.Remove(ctx), state).Wait();
        }

        private async Task<ServerItem> AddGameServer(ISteamGameServer dataItem, ICollection<IServerItem> dest, CancellationToken cancellationToken)
        {
            var serverQueryEndpoint = new IPEndPoint(dataItem.Host, dataItem.QueryPort);

            var vo = _defaultServerRepository.GetServerInfo(serverQueryEndpoint);

            if (cancellationToken.IsCancellationRequested)
            {
                return null;
            }

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

            await UiTask.Run((dest2, item2) => dest2.Add(item2), dest, item);
            return item;
        }

        private static void AssignProperties(ServerItem item, ISteamGameServer vo)
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
            if (vo.Players != null)
            {
                item.CurrentPlayersText = string.Join(", ", vo.Players.Select(p => p.Name).OrderBy(s => s));
                item.CurrentPlayers = vo.Players.OrderBy(p => p.Name).ToArray();
            }
        }

        internal async void ReloadAddons()
        {
            if (_addons == null)
                _addons = new ObservableCollection<IAddon>();

            _addons.Clear();

            var armaPath = Settings.Default.ArmaPath;

            await ReloadAddonsAsync(armaPath, true);
            await
                ReloadAddonsAsync(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments,
                        Environment.SpecialFolderOption.DoNotVerify) +
                    Path.DirectorySeparatorChar + "Arma 3" + Path.DirectorySeparatorChar, true);

            await
                ReloadAddonsAsync(this.ArmaPath + Path.DirectorySeparatorChar + "!Workshop" + Path.DirectorySeparatorChar, false);

            foreach (var modInstallPath in this._modFolders)
            {
                await ReloadAddonsAsync(modInstallPath.Path, modInstallPath.IsDefault);
            }
        
            AddonWebApi.PostInstalledAddonsKeysAsync(_addons.ToArray()).Wait(0);
        }

        private async Task ReloadAddonsAsync(string path, bool isArmaDefaultPath)
        {
            if (!string.IsNullOrEmpty(path) && Directory.Exists(path))
            {
                var items = await Task.Run(() => _defaultDataRepository.GetInstalledAddons(path));
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

        //public void TestJson()
        //{
        //    _defaultBrowserServerRepository.Test();
        //}

        private void ps_Exited(object sender, EventArgs e)
        {
            //Todo Move to ViewModel Code
            var t = UiTask.Run(() =>
            {
                if (Application.Current.MainWindow != null)
                {
                    Application.Current.MainWindow.WindowState = WindowState.Normal;
                    Application.Current.MainWindow.Activate();
                }
            });
        }


        internal IServerItem[] AddServerItems(IEnumerable<string> hostQueryAddresses)
        {
            var result = new List<IServerItem>(hostQueryAddresses.Count());
            foreach (var hostQueryAddress in hostQueryAddresses)
            {
                var pos = hostQueryAddress.IndexOf(':');
                var addr = hostQueryAddress.Substring(0, pos);
                var port = 0;
                int.TryParse(hostQueryAddress.Substring(pos + 1), out port);

                var serverItem = new ServerItem {Host = IPAddress.Parse(addr), QueryPort = port, Name = addr};
                ServerItems.Add(serverItem);
                result.Add(serverItem);
            }
            return result.ToArray();
        }

        public async Task<IEnumerable<RestAddonInfoResult>> GetAddonInfosAsync(params string[] addonKeynames)
        {
            return await AddonWebApi.GetAddonInfosAsync(addonKeynames);
        }

        internal void AddAddonUri(IAddon addon, string uri)
        {
            //Task.Run(() =>
            //{
            //    var webapi = new AddonWebApi();
            //    webapi.AddAddonDownloadUri(addon,uri);
            //});
        }

        internal static async Task UploadAddonAsync(IAddon addon)
        {
                IEnumerable<RestAddonInfoResult> infos = await AddonWebApi.GetAddonInfosAsync(addon.KeyNames.Select(k => k.Hash).ToArray());
                if (infos.Count() == 0 || infos.Any(a => a.easyinstall))
                    return;

            AddonWebApi.UploadAddon(addon);
        }

        internal async void DownloadAddonAsync(IAddon addon)
        {
            if (addon.IsEasyInstallable.HasValue && addon.IsEasyInstallable.Value && addon.KeyNames.Any() &&
                addon.KeyNames.Any(k => !string.IsNullOrEmpty(k.Hash)))
            {
                var installedAddon = await Task.Run(() =>
                {
                    var targetFolder =
                        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments,
                            Environment.SpecialFolderOption.DoNotVerify) +
                        Path.DirectorySeparatorChar + @"ArmaBrowser" + Path.DirectorySeparatorChar +
                        "Arma 3" +
                        Path.DirectorySeparatorChar + "Addons" + Path.DirectorySeparatorChar;
                    var hash = addon.KeyNames.First(k => !string.IsNullOrEmpty(k.Hash)).Hash;
                    AddonWebApi.DownloadAddon(addon, hash, targetFolder);

                    var addons = _defaultDataRepository.GetInstalledAddons(targetFolder);
                    var item = addons.FirstOrDefault(a => a.KeyNames.Any(k => k.Hash == hash));
                    if (item != null)
                    {
                        return new Addon
                        {
                            Name = item.Name,
                            DisplayText = string.Format("{0} ({1})", item.DisplayText, item.Name),
                            Path = item.Path,
                            ModName = item.ModName,
                            Version = item.Version,
                            KeyNames = item.KeyNames,
                            IsInstalled = true,
                            IsEasyInstallable = true,
                            CanActived = addon.CanActived,
                            IsActive = addon.CanActived
                        };
                    }
                    return null;
                });

                if (installedAddon != null)
                {
                    var idx = _addons.IndexOf(addon);
                    _addons[idx] = installedAddon;
                }
            }
        }

        public async Task UpdateAddonInfos(string[] hostAddonKeyNames)
        {
            var addonInfosTask = await GetAddonInfosAsync(hostAddonKeyNames);

            var addonInfosTaskResult = addonInfosTask;

            //await addonInfosTask.ContinueWith(parentTask =>
            {
                //if (parentTask.Status != TaskStatus.RanToCompletion) return;

                //var addonInfosTaskResult = parentTask.Result;
                foreach (var addonInfo in addonInfosTaskResult)
                {
                    var updAddon =
                        Addons.FirstOrDefault(
                            a =>
                                a.KeyNames.Any(
                                    tag => tag.Hash.Equals(addonInfo.hash, StringComparison.OrdinalIgnoreCase)));

                    if (updAddon != null)
                        updAddon.IsEasyInstallable = addonInfo.easyinstall;
                    else
                    {
                        if (addonInfo.easyinstall)
                            Addons.Add(new Addon
                            {
                                Name = addonInfo.name,
                                ModName = addonInfo.name,
                                DisplayText = addonInfo.name,
                                KeyNames = new[] {new AddonKey {Name = addonInfo.keytag, Hash = addonInfo.hash}},
                                //DownlandUris = new Uri[] { new Uri("http://www.armabrowser.de/"), },
                                IsInstalled = false,
                                IsEasyInstallable = addonInfo.easyinstall,
                                CanActived = true
                            });
                    }
                }
            } //);
        }

        private class ServerQueryAddressComparer : IEqualityComparer<ISteamGameServer>
        {
            internal static readonly ServerQueryAddressComparer Default = new ServerQueryAddressComparer();

            bool IEqualityComparer<ISteamGameServer>.Equals(ISteamGameServer x, ISteamGameServer y)
            {
                if (x == null || y == null)
                    return false;

                return x.Host + " " + x.QueryPort == y.Host + " " + y.QueryPort;
            }

            public int GetHashCode(ISteamGameServer obj)
            {
                return (obj.Host + " " + obj.QueryPort).GetHashCode();
            }
        }

        #region Fields

        private readonly IArma3DataRepository _defaultDataRepository;
        private ObservableCollection<IPEndPoint> _favoritServerEndPoints;
        private ObservableCollection<IAddon> _addons;
        private readonly Arma3ServerRepositorySteam _defaultServerRepository;
        //private Data.IArmaBrowserServerRepository _defaultBrowserServerRepository;
        private string _armaPath;
        private string _armaVersion;
        private readonly ModInstallPath[] _modFolders;

        #endregion Fields

        #region Properties

        public ObservableCollection<IServerItem> ServerItems { get; } = new ObservableCollection<IServerItem>();

        public ObservableCollection<LoadingServerListContext> ReloadThreads { get; } = new ObservableCollection<LoadingServerListContext>();

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
                    var folder = Settings.Default.ArmaPath;
                    if (!string.IsNullOrEmpty(folder) && File.Exists(Path.Combine(folder, "arma3.exe")))
                    {
                        var version = FileVersionInfo.GetVersionInfo(Path.Combine(folder, "arma3.exe"));
                        _armaVersion = string.Format("{0}.{1}.{2:000}{3}", version.FileMajorPart, version.FileMinorPart,
                            version.FileBuildPart, version.FilePrivatePart);
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

        #endregion Properties
    }

    internal class LoadingServerListContext : ObjectNotify
    {
        private readonly ManualResetEvent _reset;
        private int _ping;
        private int _progressValue;
        private bool _isRemoving;

        public LoadingServerListContext()
        {
            _reset = new ManualResetEvent(false);
        }

        public ISteamGameServer[] Ips { get; set; }
        public Collection<IServerItem> Dest { get; set; }
        public CancellationToken Token { get; set; }

        public WaitHandle Reset
        {
            get { return _reset; }
        }

        public int MaximumValue
        {
            get { return Ips != null ? Ips.Length : 0; }
        }

        public int ProgressValue
        {
            get { return _progressValue; }
            set
            {
                if (value == _progressValue) return;
                _progressValue = value;
                OnPropertyChanged();
                OnPropertyChanged("Procent");
            }
        }


        public double Procent
        {
            get
            {
                if (MaximumValue <= 0) return 0;
                return ProgressValue/(MaximumValue*1d);
            }
        }

        public int Ping
        {
            get { return _ping; }
            set
            {
                if (value == _ping) return;
                _ping = value;
                OnPropertyChanged();
            }
        }

        public bool IsRemoving
        {
            get { return _isRemoving; }
            private set
            {
                if (value == _isRemoving) return;
                _isRemoving = value;
                OnPropertyChanged();
            }
        }

        public void Finished()
        {
            IsRemoving = true;
            _reset.Set();
        }
    }
}