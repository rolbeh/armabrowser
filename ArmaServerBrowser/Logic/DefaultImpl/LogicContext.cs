
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ArmaServerBrowser.Logic.DefaultImpl
{
    class LogicContext : LogicModelBase, ILogicContext
    {
        #region Fields

        private readonly Data.IArma3DataRepository _defaultDataRepository;
        private ObservableCollection<IServerItem> _serverItems;
        private ObservableCollection<IAddon> _addons;
        private Data.IServerRepository _defaultServerRepository;
        private Data.IArmaBrowserServerRepository _defaultBrowserServerRepository;

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

        public string ArmaPath
        {
            get
            {
                return _defaultDataRepository.GetArma3Folder().ToString();
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
                UiTask.RunInUiAsync(() => dest.Clear(), cancellationToke).Wait();
            }

            catch (OperationCanceledException)
            {
                Console.WriteLine("Reloading canceled");
                return;
            }



            var result = _defaultServerRepository.GetServerList(filterByHostOrMission, dataItem =>
                                {
                                    cancellationToke.ThrowIfCancellationRequested();

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
                                    UiTask.RunInUiAsync((dest2, item2) => dest2.Add(item2), dest, item).Wait();
                                }).ToArray();

            //UiTask.RunInUiAsync((dest2, list) =>
            //{
            //    dest2.Clear();
            //    foreach (var item in list)
            //    {
            //        dest2.Add(item);
            //    }
            //}, dest, result).Wait();

            Array.Resize(ref result, 0);

            //Parallel.ForEach(hostEndpoints, new ParallelOptions { MaxDegreeOfParallelism = System.Environment.ProcessorCount }, UpdateEndPoint);

        }

        //private void UpdateEndPoint(System.Net.IPEndPoint endpoint)
        //{
        //    try
        //    {
        //        Data.IServerVo dataItem = null;

        //        dataItem = _defaultServerRepository.GetServerInfo(endpoint);

        //        if (dataItem == null)
        //            return;

        //        var item = new ServerItem
        //        {
        //            Country = dataItem.Country,
        //            Gamename = dataItem.Gamename,
        //            Host = endpoint.Address.ToString(),
        //            Island = dataItem.Island,
        //            Mission = dataItem.Mission,
        //            Mode = dataItem.Mode,
        //            Modhashs = dataItem.Modhashs,
        //            ModsText = dataItem.Mods,
        //            Name = dataItem.Name,
        //            Players = dataItem.Players,
        //            Port = endpoint.Port,
        //            Signatures = dataItem.Signatures,
        //            Version = dataItem.Version,
        //            Passworded = dataItem.Passworded
        //        };
        //        var dest = ServerItems;
        //        UiTask.RunInUi((d, i) => d.Add(i), dest, item);
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Diagnostics.Debug.WriteLine(ex);
        //    }
        //}

        private async void UpdateServerInfo(IServerItem server)
        {
            await Task.Run(() =>
            {
                if (string.IsNullOrEmpty(server.Host)) return;
                if (server.Port == 0) return;
                var serverItem = server as ServerItem;
                if (serverItem == null) return;

                Data.IServerVo dataItem = null;
                try
                {
                    dataItem = _defaultServerRepository.GetServerInfo(new System.Net.IPEndPoint(System.Net.IPAddress.Parse(server.Host), server.Port));
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                }
                if (dataItem == null)
                    return;
                UiTask.RunInUiAsync((serverItem1, dataItem1) =>
                {
                    serverItem1.Country = dataItem1.Country;
                    serverItem1.Gamename = dataItem1.Gamename;
                    //serverItem.Host = dataItem.Host;
                    serverItem1.Island = dataItem1.Island;
                    serverItem1.Mission = dataItem1.Mission;
                    serverItem1.Mode = dataItem1.Mode;
                    serverItem1.Modhashs = dataItem1.Modhashs;
                    serverItem1.ModsText = dataItem1.Mods;
                    serverItem1.Name = dataItem1.Name;
                    serverItem1.Players = dataItem1.Players;
                    //serverItem.Port = dataItem.Port;
                    serverItem1.Signatures = dataItem1.Signatures;
                    serverItem1.Version = dataItem1.Version;
                    serverItem1.Passworded = dataItem1.Passworded;

                }, serverItem, dataItem).Wait();


            });
        }




        public Collection<IAddon> Addons
        {
            get
            {
                if (_addons == null)
                {
                    _addons = new ObservableCollection<IAddon>();

                    var items = _defaultDataRepository.GetInstalledAddons(ArmaPath);
                    foreach (var item in items)
                    {
                        _addons.Add(new Addon
                                    {
                                        Name = item.Name,
                                        DisplayText = string.Format("{0} ({1})", item.DisplayText, item.Name),
                                        ModName = item.ModName,
                                        Version = item.Version
                                    });
                    }
                }

                return _addons;
            }
        }


        public void Open(IServerItem serverItem, IAddon[] addon, bool runAsAdmin = false)
        {
            var addonArgs = string.Format(" -mod={0}", string.Join(";", addon.Select(i => i.Name).ToArray()));

            var serverArgs = serverItem != null ? string.Format(" -connect={0} -port={1}", serverItem.Host, serverItem.Port) : string.Empty;
            var path = ArmaPath;
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
            UpdateServerInfo(item);
        }

         
    }
}
