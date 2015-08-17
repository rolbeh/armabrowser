using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmaBrowser.Data
{
    interface IServerVo
    {

        string Gamename { get; }
        string Mode { get; }
        string Name { get; }
        string Mission { get; }
        string Country { get; }
        System.Net.IPAddress Host { get; }
        int Port { get; }
        /// <summary>
        /// QueryPort für Serverstates
        /// </summary>
        int QueryPort { get; set; }

        string Version { get; }
        string Mods { get; }
        string Modhashs { get; }
        /// <summary>
        /// maximale Spieleranzahl
        /// </summary>
        int MaxPlayers { get; set; }
        int CurrentPlayerCount { get; }
        string Map { get;}
        string Signatures { get; }
        bool Passworded { get;}

        int Ping { get;}

        IEnumerable<ISteamGameServerPlayer> Players { get; }

        string Keywords { get; }

        bool VerifySignatures { get; }
    }

}
