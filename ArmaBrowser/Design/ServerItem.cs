using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ArmaBrowser.Data;
using ArmaBrowser.Logic;
using Magic.Steam;

namespace ArmaBrowser.Design
{
    class ServerItem : IServerItem
    {
        public bool IsFavorite { get; set; }
        public DateTime? LastPlayed { get; set; }
        public string Gamename { get; private set; }
        public string Mode { get; private set; }
        public String Name { get; set; }
        public string Mission { get; private set; }
        public IPAddress Host { get; private set; }
        public int Port { get; private set; }
        public bool Passworded { get; private set; }
        public string Version { get; private set; }
        public string ModsText { get; private set; }
        public string[] Mods { get; private set; }
        public string Modhashs { get; private set; }
        public int CurrentPlayerCount { get; private set; }
        public int MaxPlayers { get; set; }
        public bool IsPlayerSlotsFull { get; private set; }
        public string Island { get; private set; }
        public bool VerifySignatures { get; private set; }
        public string Signatures { get; private set; }
        public string FullText { get; private set; }
        public string CurrentPlayersText { get; private set; }
        public ISteamGameServerPlayer[] CurrentPlayers { get; private set; }
        public int QueryPort { get; private set; }
        public int Ping { get; private set; }

        public ArmaBrowser.Logic.ServerItemGroup GroupName { get; set; }
    }
}
