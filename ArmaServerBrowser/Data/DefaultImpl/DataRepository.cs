using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ArmaServerBrowser.Data.DefaultImpl
{
    sealed class DataRepository : IArma3DataRepository
    {
        public IServerVo[] GetServerList()
        {

                const string url = "http://arma3.swec.se/server/list.xml";
                try
                {
                    using (var webClient = new WebClient())
                    {
                        var bytes = webClient.DownloadData(url);
                        using (var mem = new MemoryStream(bytes, false))
                        {
                            return ParseArmaSwecServerlist.GetServerList(mem);
                        }
                    }
                }
                catch
                {
                    if (File.Exists("Serverlist.xml"))
                    {
                        using (var fs = new FileStream("Serverlist.xml", FileMode.Open))
                        {
                            return ParseArmaSwecServerlist.GetServerList(fs);
                        }
                    }
                    return new IServerVo[0];
                }
            

        }

        public string GetArma3Folder()
        {
            var steamFolder = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Valve\Steam", "SteamPath", "");

            var steamPath = System.IO.Path.Combine(steamFolder.ToString(), "config", "config.vdf");
            using (var reader = new SteamConfigReader(steamPath))
            {
                return Path.Combine(reader.GetValueOf("\t\t\t\t\"BaseInstallFolder_1\""), "SteamApps", "common", "ARMA 3");
            }

        }

        public IArmaAddOn[] GetInstalledAddons(string baseFolder)
        {
            var addonFolders = Directory.EnumerateDirectories(baseFolder, "@*");
            var result = new List<IArmaAddOn>(addonFolders.Count());
            foreach (var addonFolder in addonFolders)
            {
                var item = new ArmaAddOn
                {
                    Name = Path.GetFileName(addonFolder),
                    ModName = Path.GetFileName(addonFolder),
                    DisplayText = Path.GetFileName(addonFolder).Replace("@", ""),
                    Path = addonFolder
                };
                var addonModcpp = Path.Combine(addonFolder, "mod.cpp");
                if (File.Exists(addonModcpp))
                {
                    using (var reader = new StreamReader(addonModcpp))
                    {
                        while (!reader.EndOfStream)
                        {
                            var line = reader.ReadLine();
                            if (line != null && line.StartsWith("name", StringComparison.OrdinalIgnoreCase))
                            {
                                var fQ = line.IndexOf('"') + 1;
                                var lQ = line.LastIndexOf('"');
                                item.ModName = line.Substring(fQ, lQ - fQ);
                                item.DisplayText = item.ModName;
                            }
                        }
                    }
                }

                result.Add(item);
            }
            return result.ToArray();
        }

        public IServerRepository ServerListManager()
        {
            return new ServerRepositoryHtml();
        }
    }



}
