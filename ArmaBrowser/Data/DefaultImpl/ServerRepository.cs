using ArmaBrowser.Data.DefaultImpl.ArmaServerInfo;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace ArmaBrowser.Data.DefaultImpl
{

    


    /* sealed class ServerRepository : IServerRepository
    {

        public ServerRepository()
        {
        }

        public System.Net.IPEndPoint[] GetServerEndPoints()
        {
            string reponse = string.Empty;
            const string r = "ArmaBrowser.Resources.gslist.exe";
            var filename = Path.Combine(Path.GetTempPath(), Path.GetTempFileName());
            try
            {

                using (Stream stream = GetType().Assembly.GetManifestResourceStream(r))
                {
                    byte[] bytes = new byte[(int)stream.Length];
                    stream.Read(bytes, 0, bytes.Length);
                    File.WriteAllBytes(filename, bytes);
                }

                ProcessStartInfo psInfo = new ProcessStartInfo
                {
                    FileName = filename,
                    Arguments = "-n arma3pc ",
                    RedirectStandardOutput = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                };
                using (var ps = new Process())
                {
                    ps.StartInfo = psInfo;
                    ps.Start();

                    using (StreamReader reader = ps.StandardOutput)
                    {
                        reponse = reader.ReadToEnd();
                    }

                    ps.WaitForExit();
                }

            }
            finally
            {
                try
                {
                    File.Delete(filename);
                }
                catch { }
            }
            var list = new List<System.Net.IPEndPoint>();
            var splitedReponse = reponse.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToArray();

            foreach (var stringEndpoint in splitedReponse)
            {
                try
                {
                    list.Add(new IPEndPoint(IPAddress.Parse(stringEndpoint.Substring(0, 15).Trim()), Int32.Parse(stringEndpoint.Substring(15, stringEndpoint.Length - 15))));
                }
                catch { }
            }

            return list.ToArray();
        }

        public IServerVo GetServerInfo(IPEndPoint gameServer)
        {
            var qry = new QueryClient();
            var packets = qry.QueryServer(gameServer);
            if (packets == null) return null;
            string dataString = packets.GetDataString();

            string[] data = Regex.Split(
                    dataString,
                    Encoding.ASCII.GetString(new byte[] { 00, 00, 01 }),
                    RegexOptions.Compiled);

            var serverState = ParseServerStats(data[0]);
            if (data.Length > 1)
                serverState.CurrentPlayers = ParsePlayer(data[1]);
            else
                serverState.CurrentPlayers = Enumerable.Empty<string>();
            return serverState;
        }

        private IEnumerable<string> ParsePlayer(string data)
        {
            if (String.IsNullOrWhiteSpace(data)) return Enumerable.Empty<string>();

            string[] dataBlocks = Regex.Split(data, Encoding.ASCII.GetString(new byte[] { 00, 00 }), RegexOptions.Compiled);
            if (dataBlocks == null || dataBlocks.Length == 0)
                return Enumerable.Empty<string>();

            var players = dataBlocks[1].Split('\0').Distinct().ToArray();
            if (players == null || players.Count() == 0)
                return Enumerable.Empty<string>();

            return players;

        }

        static ServerItem ParseServerStats(string data)
        {
            ServerItem info = new ServerItem();
            string[] parts = data.Split('\0');
            Dictionary<string, string> values = new Dictionary<string, string>();
            for (int i = 0; i < parts.Length; i++)
            {
                if ((i & 1) == 0 && !values.ContainsKey(parts[i]) && (i + 1) < parts.Length)
                    values.Add(parts[i], parts[i + 1]);
            }
            //info.Country = GetValueByKey("language", values);
            info.Version = GetValueByKey("gamever", values);
            info.Name = GetValueByKey("hostname", values);
            info.Mission = GetValueByKey("mission", values);
            info.Island = GetValueByKey("mapname", values);
            info.Mode = GetValueByKey("gametype", values);
            info.PlayerNum = ParseInt(GetValueByKey("numplayers", values));
            //info.NumTeam = ParseInt(GetValueByKey("numteams", values));
            info.MaxPlayers = ParseInt(GetValueByKey("maxplayers", values));
            //info.Mode = GetValueByKey("gamemode", values);
            //info.TimeLimit = GetValueByKey("timelimit", values);
            info.Passworded = ParseBoolean(GetValueByKey("password", values));
            //info.CurrentVersion = GetValueByKey("currentVersion", values);
            //info.RequiredVersion = GetValueByKey("requiredVersion", values);
            info.Mods = GetValueByKey("mod", values);
            info.Signatures = GetValueByKey("signatures", values);
            //info.Longitude = ParseDouble(GetValueByKey("lng", values));
            //info.Latitude = ParseDouble(GetValueByKey("lat", values));
            
             ////gamever 
             ////    hostname 
             ////    mapname 
             ////    gametype 
             ////    numplayers 
             ////    numteams 
             ////    maxplayers 
             ////    gamemode 
             ////    timelimit 
             ////    password 
             ////    param1 
             ////    param2 
             ////    ver 
             ////    requiredVersion 
             ////    mod 
             ////    equalModRequired 
             ////    gameState 
             ////    dedicated 
             ////    platform 
             ////    language 
             ////    difficulty 
             ////    mission 
             ////    gamename 
             ////    sv_battleye 
             ////    verifySignatures 
             ////    signatures 
             ////    modhash 
             ////    hash 
             ////    reqBuild 
             ////    reqSecureId 
             ////    lat 
             ////    lng 
             ////    ISO2
            
            return info;
        }

        private static string GetValueByKey(string key, Dictionary<string, string> values)
        {
            if (values.ContainsKey(key))
                return values[key];
            return null;
        }
        private static int ParseInt(string value)
        {
            if (String.IsNullOrWhiteSpace(value))
                return 0;
            int parsedValue = 0;
            Int32.TryParse(value, out parsedValue);
            return parsedValue;
        }
        private static double ParseDouble(string value)
        {
            if (String.IsNullOrWhiteSpace(value))
                return 0;
            double parsedValue = 0;
            Double.TryParse(value, out parsedValue);
            return parsedValue;
        }
        private static bool ParseBoolean(string value)
        {
            if (String.IsNullOrWhiteSpace(value))
                return false;
            if (value == "1" || value.ToLowerInvariant() == "true")
                return true;
            return false;
        }

        public IServerVo[] GetServerList(string hostAndMissionFilter, Action<IServerVo> itemGenerated)
        {
            throw new NotSupportedException();
        }
    }
*/
    /* //ServerRepositoryGamespyHtml
        sealed class ServerRepositoryGamespyHtml : IServerRepository
        {
            private ServerRepository _client;

            public ServerRepositoryGamespyHtml()
            {
            }

            public System.Net.IPEndPoint[] GetServerEndPoints()
            {
                throw new NotSupportedException();
            }

            public IServerVo GetServerInfo(IPEndPoint gameServer)
            {
                var client = _client ?? (_client = new ServerRepository());
                return _client.GetServerInfo(gameServer);
            }


            public IServerVo[] GetServerList(string hostAndMissionFilter, Action<IServerVo> itemGenerated)
            {//hostname LIKE '%Armaj%'

                string filterText = string.IsNullOrEmpty(hostAndMissionFilter) ? string.Empty : string.Format("%28%28mission+LIKE+%27%25{0}%25%27%29+OR+%28hostname+LIKE+%27%25{0}%25%27%29%29", Uri.EscapeUriString(hostAndMissionFilter));
                const string url = @"index.aspx?gamename=arma3pc&fields=%5Chostname%5Cgamever%5Ccountry%5Clanguage%5Cmapname%5Cgametype%5Cgamemode%5Cnumplayers%5Cmaxplayers%5Cmission%5Cpassword%5Cdifficulty%5Cdedicated%5Cplatform%5CgameState%5Cver%5Cmod%5CequalModRequired%5Csignatures%5Clat%5CverifySignatures%5Clng%5Cmodhash%5Chash&overridemaster=&filter=";

                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://gstadmin.gamespy.net/masterserver/");

                    var httpTask = client.GetAsync(url + filterText);
                    httpTask.Wait();

                    var contentTask = httpTask.Result.Content.ReadAsStringAsync();
                    contentTask.Wait();

                    var content = contentTask.Result;
                    var startPos = content.IndexOf("<table border=\"1\">");
                    var endPos = content.IndexOf("</table>") + 8;
                    content = content.Substring(startPos, endPos - startPos).Replace(Encoding.UTF8.GetString(new byte[] { 0x08 }), "");
                    XmlDocument xml = new XmlDocument();
                    xml.LoadXml(content);
                    var fields = new Dictionary<string, string>();
                    var resultList = new List<IServerVo>();
                    // Felder lesen
                    var tr = xml.DocumentElement.FirstChild;
                    foreach (var td in tr.ChildNodes.Cast<XmlNode>())
                    {
                        if (td.NodeType != XmlNodeType.Element) continue;
                        fields.Add(td.FirstChild.Value, string.Empty);
                    }

                    // DatenZeilen lesen
                    tr = tr.NextSibling;
                    while (tr != null)
                    {
                        var idx = 0;
                        foreach (var td in tr.ChildNodes.Cast<XmlNode>())
                        {
                            if (td.NodeType != XmlNodeType.Element) continue;
                            fields[fields.Keys.ElementAt(idx)] = td.FirstChild != null ? td.FirstChild.Value : string.Empty;

                            idx++;
                        }
                        //<td>publicip</td><td>publicport</td><td>privateip</td><td>privateport</td><td>icmpip</td><td>flags</td><td>hostname</td>
                        //<td>hostport</td><td>gamever</td><td>country</td><td>mapname</td><td>gametype</td><td>gamemode</td><td>numplayers</td>
                        //<td>maxplayers</td><td>groupid</td><td>mission</td>
                        ServerItem info = new ServerItem
                            {
                                Host = fields["publicip"],
                                Port = Int32.Parse(fields["publicport"]),
                                Country = fields["country"],
                                Passworded = ParseBooleanHtml(fields["password"]),
                                Name = fields["hostname"],
                                Mission = fields["mission"],
                                PlayerNum = Int32.Parse(fields["numplayers"]),
                                Mode = fields["gametype"],
                                Mods = fields["mod"],
                                Version = fields["gamever"]
                            };
                        if (itemGenerated != null)
                        {
                            try
                            {
                                itemGenerated(info);
                            }
                            catch (OperationCanceledException)
                            {
                                Console.WriteLine("Reloading canceled");
                                return new IServerVo[0];
                            }

                        }
                        resultList.Add(info);

                        tr = tr.NextSibling;
                    }
                    return resultList.ToArray();
                }

            }
     
            #region helper

            static IServerVo Parse(string data)
            {
                ServerItem info = new ServerItem();
                string[] parts = data.Split('\0');
                Dictionary<string, string> values = new Dictionary<string, string>();
                for (int i = 0; i < parts.Length; i++)
                {
                    if ((i & 1) == 0 && !values.ContainsKey(parts[i]) && (i + 1) < parts.Length)
                        values.Add(parts[i], parts[i + 1]);
                }
                //info.Country = GetValueByKey("language", values);
                info.Version = GetValueByKey("gamever", values);
                info.Name = GetValueByKey("hostname", values);
                info.Mission = GetValueByKey("mission", values);
                info.Island = GetValueByKey("mapname", values);
                info.Mode = GetValueByKey("gametype", values);
                info.PlayerNum = ParseInt(GetValueByKey("numplayers", values));
                //info.NumTeam = ParseInt(GetValueByKey("numteams", values));
                info.MaxPlayers = ParseInt(GetValueByKey("maxplayers", values));
                //info.Mode = GetValueByKey("gamemode", values);
                //info.TimeLimit = GetValueByKey("timelimit", values);
                info.Passworded = ParseBoolean(GetValueByKey("password", values));
                //info.CurrentVersion = GetValueByKey("currentVersion", values);
                //info.RequiredVersion = GetValueByKey("requiredVersion", values);
                info.Mods = GetValueByKey("mod", values);
                info.Signatures = GetValueByKey("signatures", values);
                //info.Longitude = ParseDouble(GetValueByKey("lng", values));
                //info.Latitude = ParseDouble(GetValueByKey("lat", values));
             
                 ////gamever 
                 ////hostname 
                 ////mapname 
                 ////gametype 
                 ////numplayers 
                 ////numteams 
                 ////maxplayers 
                 ////gamemode 
                 ////timelimit 
                 ////password 
                 ////param1 
                 ////param2 
                 ////ver 
                 ////requiredVersion 
                 ////mod 
                 ////equalModRequired 
                 ////gameState 
                 ////dedicated 
                 ////platform 
                 ////language 
                 ////difficulty 
                 ////mission 
                 ////gamename 
                 ////sv_battleye 
                 ////verifySignatures 
                 ////signatures 
                 ////modhash 
                 ////hash 
                 ////reqBuild 
                 ////reqSecureId 
                 ////lat 
                 ////lng 
                 ////ISO2
             
                return info;
            }

            private static string GetValueByKey(string key, Dictionary<string, string> values)
            {
                if (values.ContainsKey(key))
                    return values[key];
                return null;
            }
            private static int ParseInt(string value)
            {
                if (String.IsNullOrWhiteSpace(value))
                    return 0;
                int parsedValue = 0;
                Int32.TryParse(value, out parsedValue);
                return parsedValue;
            }
            private static double ParseDouble(string value)
            {
                if (String.IsNullOrWhiteSpace(value))
                    return 0;
                double parsedValue = 0;
                Double.TryParse(value, out parsedValue);
                return parsedValue;
            }
            private static bool ParseBoolean(string value)
            {
                if (String.IsNullOrWhiteSpace(value))
                    return false;
                if (value == "1" || value.ToLowerInvariant() == "true")
                    return true;
                return false;
            }

            private static bool ParseBooleanHtml(string value)
            {
                if (String.IsNullOrWhiteSpace(value))
                    return false;
                if (value != "0")
                    return true;
                return false;
            }

            #endregion helper
        }

        sealed class ServerRepositoryFromFile : IServerRepository
        {
            private ServerRepository _client;

            public IPEndPoint[] GetServerEndPoints()
            {
                return new IPEndPoint[]{
                    new IPEndPoint(IPAddress.Parse("144.76.73.69"),4660),
                    new IPEndPoint(IPAddress.Parse("144.76.73.69"),4670),
                    new IPEndPoint(IPAddress.Parse("144.76.73.69"),4690),
                };
            }

            public IServerVo[] GetServerList(string hostAndMissionFilter, Action<IServerVo> itemGenerated = null)
            {
                var result = new List<ServerItem>{
                    new ServerItem
                    {
                        Host = "144.76.73.69",
                        Port = 4660,
                        Name = "-[EUTW]- CTI Warfare (Teetimes)"
                    },
                    new ServerItem
                    {
                        Host = "144.76.73.69",
                        Port = 4660,
                        Name = "-[EUTW]- CTI Warfare (Teetimes)"
                    },
                    new ServerItem
                    {
                        Host = "144.76.73.69",
                        Port = 4660,
                        Name = "-[EUTW]- CTI Warfare (Teetimes)"
                    }};

                if (itemGenerated != null)
                    foreach (var item in result)
                    {
                        itemGenerated(item);
                    }

                return result.ToArray();
            }

            public IServerVo GetServerInfo(IPEndPoint gameServer)
            {
                var client = _client ?? (_client = new ServerRepository());
                return _client.GetServerInfo(gameServer);
            }
        }
        */

}
