
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ArmaBrowser.Logic.DefaultImpl
{
    class LogicContext : LogicModelBase, ILogicContext
    {
        #region Fields
//        private readonly LimitedConcurrencyLevelTaskScheduler _taskScheduler = new LimitedConcurrencyLevelTaskScheduler(25);

        private readonly Data.IArma3DataRepository _defaultDataRepository;
        private ObservableCollection<IPEndPoint> _favoritServerEndPoints;
        private ObservableCollection<IServerItem> _serverItems;
        private ObservableCollection<IAddon> _addons;
        private Data.IServerRepository _defaultServerRepository;
        private Data.IArmaBrowserServerRepository _defaultBrowserServerRepository;
        private string _armaPath = null;
        private string _armaVersion = null;
        private Data.IServerVo[] _serverIPListe;

        #endregion Fields

        #region Properties

        public ObservableCollection<IServerItem> ServerItems
        {
            get
            {
                if (_serverItems == null)
                {
                    _serverItems = new ObservableCollection<IServerItem>();
                }

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
                        _armaVersion = string.Format("{0}.{1}.{2:000}{3}", version.FileMajorPart, version.FileMinorPart, version.FilePrivatePart, version.FileBuildPart);  
                    }
                    else
                        _armaVersion = "";
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

        public LogicContext()
        {
            _defaultDataRepository = Data.DataManager.CreateNewDataRepository();
            _defaultServerRepository = Data.DataManager.CreateNewServerRepository();
            _defaultBrowserServerRepository = Data.DataManager.CreateNewArmaBrowserServerRepository();
        }


        /*public void ReloadServerItems()
        {
            var dataItems = _defaultDataRepository.GetServerList();
            var dest = ServerItems;
            UiTask.RunInUiAsync(() => dest.Clear()).Wait();

            foreach (var dataItem in dataItems)
            {
                var dest2 = dest;
                var item = new ServerItem
                {
                    Country = dataItem.Country,
                    Gamename = dataItem.Gamename,
                    Host = dataItem.Host,
                    Island = dataItem.Island,
                    Mission = dataItem.Mission,
                    Mode = dataItem.Mode,
                    Modhashs = dataItem.Modhashs,
                    ModsText = dataItem.Mods,
                    Name = dataItem.Name,
                    Players = dataItem.Players,
                    Port = dataItem.Port,
                    Signatures = dataItem.Signatures,
                    Version = dataItem.Version,
                    Passworded = dataItem.Passworded
                };
                UiTask.RunInUi((d, i) => d.Add(i), dest2, item);
            }
        }*/



        public void ReloadServerItems(string filterByHostOrMission, CancellationToken cancellationToke)
        {

            //var hostEndpoints = _defaultServerRepository.GetServerEndPoints();

            var dest = ServerItems;
            try
            {
                UiTask.Run(() => dest.Clear(), cancellationToke).Wait();
            }

            catch (OperationCanceledException)
            {
                System.Diagnostics.Debug.WriteLine("Reloading canceled");
                return;
            }

            _serverIPListe = _defaultServerRepository.GetServerList(filterByHostOrMission);
            


            cancellationToke.ThrowIfCancellationRequested();
            
            var token = cancellationToke;

            Parallel.ForEach(_serverIPListe, new ParallelOptions() { CancellationToken = token }, dataItem =>
                                {
                                    if (token.IsCancellationRequested) 
                                        return;

                                    var serverQueryEndpoint = new System.Net.IPEndPoint(dataItem.Host, dataItem.QueryPort);


                                    var vo = _defaultServerRepository.GetServerInfo(serverQueryEndpoint);

                                    if (cancellationToke.IsCancellationRequested || vo == null) 
                                        return;
                                    
                                            var item = new ServerItem();
                                            AssignProperties(item,vo);
                                            //{
                                            //    //Country = vo.Country,
                                            //    Gamename = vo.Gamename,
                                            //    Host = vo.Host,
                                            //    Island = vo.Map,
                                            //    Mission = vo.Mission,
                                            //    Mode = vo.Mode,
                                            //    Modhashs = vo.Modhashs,
                                            //    ModsText = vo.Mods,
                                            //    Name = vo.Name,
                                            //    CurrentPlayerCount = vo.CurrentPlayerCount,
                                            //    MaxPlayers = vo.MaxPlayers,
                                            //    Port = vo.Port,
                                            //    QueryPort = vo.QueryPort,
                                            //    Signatures = vo.Signatures,
                                            //    Version = vo.Version,
                                            //    Passworded = vo.Passworded,
                                            //    Ping = vo.Ping,
                                            //    IsVersionOk = ArmaVersion == vo.Version,
                                            //    CurrentPlayers = string.Join(", ", vo.Players.Select(p => p.Name).OrderBy(s => s))

                                            //};
                                           
                                     
                                    UiTask.Run((dest2, item2) => dest2.Add(item2), dest, item);
                                });

        }

        private async Task UpdateServerInfo(IServerItem server)
        {
            await Task.Run(() =>
            {
                var serverItem = server as ServerItem;
                if (serverItem == null) return; 
                if (server.Host == null) return;
                if (server.QueryPort == 0) return;
                

                Data.IServerVo dataItem = null;
                try
                {
                    dataItem = _defaultServerRepository.GetServerInfo(new System.Net.IPEndPoint(server.Host, server.QueryPort));
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                }
                if (dataItem == null)
                    return;

                UiTask.Run(AssignProperties, serverItem, dataItem).Wait();


            });
        }

        void AssignProperties(ServerItem item, Data.IServerVo vo)
        {
            var keyWords = vo.Keywords.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToDictionary(s => s.Substring(0, 1), e => e.Substring(1));

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
            item.Version = vo.Version;
            item.Passworded = keyWords.FirstOrDefault(k => k.Key == "l").Value == "t"; //dataItem1.Passworded;
            item.Ping = vo.Ping;
            item.CurrentPlayersText = string.Join(", ", vo.Players.Select(p => p.Name).OrderBy(s => s));
            item.CurrentPlayers = vo.Players.OrderBy(p => p.Name).ToArray() ;
        }


        public Collection<IAddon> Addons
        {
            get
            {
                if (_addons == null)
                {
                    _addons = new ObservableCollection<IAddon>();
                    var armaPath = Properties.Settings.Default.ArmaPath;
                    if (!string.IsNullOrEmpty(armaPath))
                    {
                        var items = _defaultDataRepository.GetInstalledAddons(armaPath);
                        foreach (var item in items)
                        {
                            _addons.Add(new Addon
                                        {
                                            Name = item.Name,
                                            DisplayText = string.Format("{0} ({1})", item.DisplayText, item.Name),
                                            ModName = item.ModName,
                                            Version = item.Version,
                                            KeyNames = item.KeyNames
                                        });
                        }
                    }
                }

                return _addons;
            }
        }


        public void Open(IServerItem serverItem, IAddon[] addon, bool runAsAdmin = false)
        {
            var addonArgs = string.Format(" -mod={0}", string.Join(";", addon.Select(i => i.Name).ToArray()));

            var serverArgs = serverItem != null ? string.Format(" -connect={0} -port={1}", serverItem.Host, serverItem.Port) : string.Empty;
            var path = Properties.Settings.Default.ArmaPath;
            var psInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = Path.Combine(path, "Arma3.exe"),
                Arguments = " -noSplash -noPause -world=empty " + addonArgs + serverArgs,// -malloc=system
                WorkingDirectory = path,
                UseShellExecute = true,

            };

            if (runAsAdmin)
                psInfo.Verb = "runsas";

            try
            {
                var ps = System.Diagnostics.Process.Start(psInfo);
                ps.EnableRaisingEvents = true;
                ps.Exited += ps_Exited;
            }
            catch (Exception)
            {

            }

        }

        public void TestJson()
        {
            _defaultBrowserServerRepository.Test();
        }

        void ps_Exited(object sender, EventArgs e)
        {

        }


        public void RefreshServerInfo(IServerItem item)
        {
            if (item == null) return;
            UpdateServerInfo(item).Wait();
        }

        public Task RefreshServerInfoAsync(IServerItem item)
        {
            return UpdateServerInfo(item);
        }

    }
}
