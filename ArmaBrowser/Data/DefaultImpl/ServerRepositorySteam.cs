using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ArmaBrowser.Data.DefaultImpl
{
    sealed class ServerRepositorySteam : IServerRepository
    {
        public static string SteamGameNameFilter = "Arma3";

        static Encoding CharEncoding = Encoding.GetEncoding(1252);
        private System.Exception _excetion;

        #region IServerRepository

        public IPEndPoint[] GetServerEndPoints()
        {
            throw new NotSupportedException();
        }

        public IServerVo[] GetServerList(Action<IServerVo> itemGenerated)
        {
            // https://developer.valvesoftware.com/wiki/Master_Server_Query_Protocol

            var bufferList = new List<IServerVo>(5000);

            IPEndPoint lastEndPoint = new IPEndPoint(0, 0);


            var addressBytes = new byte[4];
            var portBytes = new byte[2];
            int port = 0;


            var roundtrips = 0;

            var dnsEntry = System.Net.Dns.GetHostEntry("hl2master.steampowered.com");
            //var ip = dnsEntry.AddressList.Length > 1 ? dnsEntry.AddressList[1] : dnsEntry.AddressList[0];
            foreach (var ip in dnsEntry.AddressList)
            {
                using (System.Net.Sockets.UdpClient udp = new System.Net.Sockets.UdpClient())
                {
                    bufferList.Clear();
                    var hasErros = false;

                    //udp.Connect("208.64.200.52", 27011);
                    while (true)
                    {
                        byte[] buffer = null;
                        try
                        {
                            udp.Connect(ip, 27011);
                            string request = "1ÿ";
                            request += lastEndPoint + "\0";
                            request += "\\gamedir\\" + SteamGameNameFilter;
                            //request += "\\empty\\0";

                            request += "\0";

                            var bytes = CharEncoding.GetBytes(request);

                            //udp.Connect("146.66.155.8", 27019);
                            IPEndPoint endp = udp.Client.RemoteEndPoint as IPEndPoint;
                            udp.Client.ReceiveTimeout = 900;

                            var sendlen = udp.Send(bytes, bytes.Length);
#if DEBUG
                            if (sendlen != bytes.Length)
                                Debug.WriteLine("IServerRepository.GetServerList - sendlen != bytes.Length");
#endif

                            roundtrips++;
                            buffer = udp.Receive(ref endp);
                        }
                        catch (System.Net.Sockets.SocketException soEx)
                        {
                            hasErros = true;
                            _excetion = soEx;
                            break;
                        }
                        catch (TimeoutException timeEx)
                        {
                            hasErros = true;
                            _excetion = timeEx;
                            break;
                        }
                        catch (ObjectDisposedException dispEx)
                        {
                            hasErros = true;
                            _excetion = dispEx;
                            break;
                        }

                        using (var mem = new System.IO.MemoryStream(buffer, false))
                        using (var br = new System.IO.BinaryReader(mem, Encoding.UTF8))
                        {
                            var i = br.ReadUInt32();
                            i = br.ReadUInt16();

                            while (mem.Position < mem.Length)
                            {

                                addressBytes[0] = br.ReadByte();
                                addressBytes[1] = br.ReadByte();
                                addressBytes[2] = br.ReadByte();
                                addressBytes[3] = br.ReadByte();

                                portBytes[0] = br.ReadByte();
                                portBytes[1] = br.ReadByte();

                                port = portBytes[0] << 8 | portBytes[1];


                                if (addressBytes.All(b => b == 0))
                                    break;

                                lastEndPoint = new IPEndPoint(new IPAddress(addressBytes), port);

                                var item = new ServerItem();
                                item.Host = lastEndPoint.Address;
                                item.QueryPort = lastEndPoint.Port;
                                bufferList.Add(item);
                                if (itemGenerated != null)
                                    itemGenerated(item);
                            }

                            Debug.WriteLine("RoundTrips {0} - {1}", roundtrips, lastEndPoint.ToString());

                            if (addressBytes.All(b => b == 0))
                            {
                                break;
                            }
                        }
                    }
                    if (!hasErros)
                        break;
                }
            }

            if (bufferList.Count == 0 || _excetion != null)
            {
                //bufferList.Clear();
                var file = Properties.Settings.Default.HostList;
                if (!string.IsNullOrEmpty(file))
                {

                }
            }
            //else
            //{
            //    var file = Path.GetTempFileName();
            //    Properties.Settings.Default.HostList = file;
            //    using(var fs = new FileStream(file, FileMode.OpenOrCreate))
            //    using(var bw = new BinaryWriter(fs))
            //    {
            //        while(fs.Position < fs.Length) 
            //        {
            //           // bufferList
            //        }
            //    }
            //}


            return bufferList.ToArray();
        }

        public IServerVo GetServerInfo(IPEndPoint gameServerQueryEndpoint)
        {

            byte[] startBytes = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF };
            byte Header = 0x54;
            string qryAsString = "Source Engine Query";

            List<byte> qry = new List<byte>();
            qry.AddRange(startBytes);
            qry.Add(Header);
            qry.AddRange(Encoding.Default.GetBytes(qryAsString));
            qry.Add(0x00);

            ServerQueryRequest item = null;
            var sw = new System.Diagnostics.Stopwatch();
            var buffer = new byte[2048];
            using (System.Net.Sockets.UdpClient udp = new System.Net.Sockets.UdpClient())
            {
                udp.Connect(gameServerQueryEndpoint);

                udp.AllowNatTraversal(true);
                udp.Client.ReceiveTimeout = 300;

                try
                {
                    sw.Start();
                    udp.Send(qry.ToArray(), qry.Count);

                    System.Net.Sockets.SocketError errorCode;
                    var receivedLenght = udp.Client.Receive(buffer, 0, 2048, System.Net.Sockets.SocketFlags.None, out errorCode);
                    sw.Stop();
                    if (errorCode != System.Net.Sockets.SocketError.Success) return null;
                    if (receivedLenght == buffer.Length)
                        throw new Exception("Buffer zu klein");


                    var receivedBytes = new byte[receivedLenght];
                    Buffer.BlockCopy(buffer, 0, receivedBytes, 0, receivedLenght);
                    //var receivedBytes = udp.Receive(ref endp);

                    var enconding = System.Text.Encoding.UTF8; // System.Text.Encoding.ASCII;

                    using (var mem = new System.IO.MemoryStream(receivedBytes))
                    using (var br = new System.IO.BinaryReader(mem, CharEncoding))
                    {
                        br.ReadInt32();
                        br.ReadByte();
                        item = new ServerQueryRequest();
                        item.ProtocolVersion = br.ReadByte();

                        item.Ping = sw.ElapsedMilliseconds;

                        var count = Array.FindIndex(receivedBytes, (int)mem.Position, IsNULL) - (int)mem.Position;
                        item.GameServerName = enconding.GetString(br.ReadBytes(count));
                        br.ReadByte(); // null-byte

                        count = Array.FindIndex(receivedBytes, (int)mem.Position, IsNULL) - (int)mem.Position;
                        item.Map = enconding.GetString(br.ReadBytes(count));
                        br.ReadByte(); // null-byte
                        //item.Map = ReadStringNullTerminated(br, enconding);

                        count = Array.FindIndex(receivedBytes, (int)mem.Position, IsNULL) - (int)mem.Position;
                        item.Folder = enconding.GetString(br.ReadBytes(count));
                        br.ReadByte(); // null-byte

                        count = Array.FindIndex(receivedBytes, (int)mem.Position, IsNULL) - (int)mem.Position;
                        item.Game = enconding.GetString(br.ReadBytes(count));
                        br.ReadByte(); // null-byte

                        item.ID = br.ReadInt16();

                        item.CurrentPlayerCount = br.ReadByte();

                        item.MaxPlayerCount = br.ReadByte();

                        item.CurrentBotsCount = br.ReadByte();

                        item.ServerType = (ServerQueryRequestType)br.ReadByte();

                        item.Password = br.ReadByte() == 1;

                        item.VAC = br.ReadByte() == 1;

                        item.Mode = br.ReadByte() == 1;

                        count = Array.FindIndex(receivedBytes, (int)mem.Position, IsNULL) - (int)mem.Position;
                        item.Version = enconding.GetString(br.ReadBytes(count));
                        br.ReadByte(); // null-byte

                        var edfCode = br.ReadByte();

                        item.Data = receivedBytes;

                        if ((edfCode & 0x80) == 0x80)
                            item.GamePort = br.ReadUInt16();



                        if ((edfCode & 0x10) == 0x10)
                            item.ServerSteamId = br.ReadInt64();

                        //if ((edfCode & 0x40) == 0x40)
                        //    Console.WriteLine("spectator server");

                        if ((edfCode & 0x20) == 0x20)
                        {
                            count = Array.FindIndex(receivedBytes, (int)mem.Position, IsNULL) - (int)mem.Position;
                            item.Keywords = enconding.GetString(br.ReadBytes(count));
                            br.ReadByte(); // null-byte
                        }

                        if ((edfCode & 0x01) == 0x01)
                            item.GameID = br.ReadInt64();

                        // https://community.bistudio.com/wiki/STEAMWORKSquery
                    }

                    RequestRules(item, udp);

                    if (item.CurrentPlayerCount > 0)
                    {
                        RequestPlayers(item, udp);
                    }
                }
                catch
                {
                    sw.Stop();
                }


            }

            //var modName = GetValue("modNames", item.KeyValues);

            //var modHashs = GetValue("modHashes", item.KeyValues);

            return item != null
                        ? new ServerItem
                        {
                            Host = gameServerQueryEndpoint.Address,
                            QueryPort = gameServerQueryEndpoint.Port,
                            Name = item.GameServerName,
                            Gamename = SteamGameNameFilter,
                            Map = item.Map,
                            Mission = item.Game,
                            MaxPlayers = item.MaxPlayerCount,
                            CurrentPlayerCount = item.CurrentPlayerCount,
                            Passworded = item.Password,
                            Port = item.GamePort,
                            Version = item.Version,
                            Keywords = item.Keywords,
                            CurrentPlayers = new string[0],
                            Ping = item.Ping,
                            Players = item.Players.Cast<ISteamGameServerPlayer>().ToArray(),
                            Mods = GetValue("modNames", item.KeyValues),
                            Modhashs = GetValue("modHashes", item.KeyValues),
                            Signatures = GetValue("sigNames", item.KeyValues),
                            VerifySignatures = item.Keywords.Contains(",vt,")
                        }
                        : null;
        }

        #endregion IServerRepository


        static string ReadStringNullTerminated(BinaryReader reader, Encoding encoding)
        {
            var result = new byte[80];
            var i = -1;
            while (reader.PeekChar() != 0)
            {
                i++;
                if (i >= result.Length)
                    Array.Resize(ref result, result.Length + 80);
                result[i] = reader.ReadByte();
            }

            if ((reader.PeekChar() == 0))
                reader.ReadByte();

            if (i == -1)
                return string.Empty;

            return encoding.GetString(result, 0, i);

            //count = Array.FindIndex(receivedBytes, (int)mem.Position, IsNULL) - (int)mem.Position;
            //item.Map = enconding.GetString(br.ReadBytes(count));
            //br.ReadByte(); // null-byte
        }

        private static void RequestRules(ServerQueryRequest item, System.Net.Sockets.UdpClient udp)
        {
            const byte ruleRequestByte = 0x56;
            IPEndPoint endp = null;
            try
            {


                // Rules auslesen
                var challengeRequest = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, ruleRequestByte, 0xFF, 0xFF, 0xFF, 0xFF };
                var sendlen = udp.Send(challengeRequest, 9);

                var respose = udp.Receive(ref endp);

                if (respose[4] != 0x41)
                {
                    System.Diagnostics.Debug.WriteLine("Error in RequestRules - Challenge response");
                    return;
                }
                respose[4] = ruleRequestByte;
                sendlen = udp.Send(respose, 9);
                respose = udp.Receive(ref endp);

                if (respose[4] != 0x45)
                {
                    System.Diagnostics.Debug.WriteLine("Error in RequestRules - Rules response");
                    return;
                }

                int ruleCount = respose[5] + (respose[6] >> 8);
                int offset = 7;

                for (int i = 0; i < ruleCount; i++)
                {
                    var key = ReadStringNullTerminated(respose, ref offset);
                    var value = ReadStringNullTerminated(respose, ref offset);
                    if (!string.IsNullOrEmpty(key))
                        item.KeyValues.Add(key, value);
                }



            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private static void RequestPlayers(ServerQueryRequest item, System.Net.Sockets.UdpClient udp)
        {
            const byte playerRequestByte = 0x55;
            try
            {
                IPEndPoint endp = null;

                var challengeRequest = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, playerRequestByte, 0xFF, 0xFF, 0xFF, 0xFF };
                var sendlen = udp.Send(challengeRequest, 9);

                var respose = udp.Receive(ref endp); // challenge response

                if (respose[4] != 0x41)
                {
                    System.Diagnostics.Debug.WriteLine("Error in RequestPlayers - Challenge response");
                    return;
                }

                respose[4] = playerRequestByte;
                sendlen = udp.Send(respose, 9);
                respose = udp.Receive(ref endp);

                if (respose[4] != 0x44)
                {
                    System.Diagnostics.Debug.WriteLine("Error in RequestPlayers - Players response");
                    return;
                }

                byte playerCount = respose[5];
                var offset = 6;
                for (byte i = 0; i < playerCount; i++)
                {

                    var p = new ServerQueryRequest.Player();
                    p.Idx = respose[offset];
                    offset += 1;
                    p.Name = ReadStringNullTerminated(respose, ref offset);
                    p.Score = BitConverter.ToInt32(respose, offset);
                    offset += 4;
                    p.OnlineTime = TimeSpan.FromSeconds(Convert.ToDouble(BitConverter.ToSingle(respose, offset)));

                    offset += 4;
                    item.Players.Add(p);
                }
            }
            catch
            {


            }
        }

        private static string GetValue(string keyWord, Dictionary<string, string> dic)
        {
            var sb = new StringBuilder();
            var keyWordLen = keyWord.Length;
            var key = dic.Keys.FirstOrDefault(k => k.Length >= keyWordLen && keyWord == k.Substring(0, keyWordLen));
            if (key != null)
            {
                var idx = key.LastIndexOf('-') + 1;
                var count = 0;
                if (Int32.TryParse(key.Substring(idx, key.Length - idx), out count))
                {
                    for (int i = 0; i < count; i++)
                    {
                        sb.Append(dic[string.Format("{0}:{1}-{2}", keyWord, i, count)]);
                    }
                }
            }

            return sb.ToString();
        }

        private static string ReadStringNullTerminated(byte[] bytes, ref int offset)
        {
            var count = Array.FindIndex(bytes, (int)offset, IsNULL) - (int)offset;
            var result = System.Text.Encoding.UTF8.GetString(bytes, offset, count);
            offset = offset + count + 1/*null-byte*/;
            return result;
        }

        static bool IsNULL(byte b)
        {
            return b == System.Byte.MinValue;
        }

        class ServerQueryRequest
        {

            public ServerQueryRequest()
            {
                Keywords = string.Empty;
                KeyValues = new Dictionary<string, string>();
                Players = new List<Player>(byte.MaxValue);
            }

            public byte ProtocolVersion;
            public string GameServerName;
            public string Map;
            public string Folder;
            public string Game;

            public short ID;
            public byte CurrentPlayerCount;
            public byte MaxPlayerCount;
            public byte CurrentBotsCount;
            public ServerQueryRequestType ServerType;
            public bool Password;

            public bool VAC;
            public bool Mode;
            public string Version;
            public Int32 GamePort;
            public byte[] Data;

            public long ServerSteamId;

            public string Keywords;

            public long GameID;
            public long Ping;

            public Dictionary<string, string> KeyValues { get; private set; }

            public class Player : ISteamGameServerPlayer
            {
                public string Name { get; set; }

                public Int32 Score { get; set; }

                public TimeSpan OnlineTime { get; set; }

                public byte Idx { get; set; }
            }

            public List<Player> Players { get; private set; }
        }


        enum ServerQueryRequestType
        {
            DedicatedServer = 0x64, // 'd'
            NonDedicatedServer = 0x6C, // 'l'
            RelayServer = 0x70, // 'p'
        }
    }
}
