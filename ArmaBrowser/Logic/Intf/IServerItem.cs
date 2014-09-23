using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmaBrowser.Logic
{
    interface IServerItem
    {
        /// <summary>
        /// Arma3
        /// </summary>
        string Gamename { get; }

        string Mode { get; }

        /// <summary>
        /// Name des GameServer
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Name der Mission
        /// </summary>
        string Mission { get; }

        /// <summary>
        /// Länder Code e.g.'DE'
        /// </summary>
        //string Country { get; }
        
        /// <summary>
        /// IP - Adresse
        /// </summary>
        System.Net.IPAddress Host { get; }

        /// <summary>
        /// GamePort
        /// </summary>
        int Port { get; }

        /// <summary>
        /// Passwortgeschützt
        /// </summary>
        bool Passworded { get; }

        /// <summary>
        /// 
        /// </summary>
        string Version { get; }

        /// <summary>
        /// 
        /// </summary>
        string ModsText { get; }

        /// <summary>
        /// 
        /// </summary>
        string[] Mods { get; }
        
        /// <summary>
        /// 
        /// </summary>
        string Modhashs { get; }

        /// <summary>
        /// Anzahl der maximalen Spieler
        /// </summary>
        int CurrentPlayerCount { get; }

        /// <summary>
        /// maximale Spieleranzahl
        /// </summary>
        int MaxPlayers { get; set; }

        /// <summary>
        /// Karte
        /// </summary>
        string Island { get; }


        string Signatures { get; }

        string FullText { get; }

        /// <summary>
        /// Name der aktuellen Spieler
        /// </summary>
        string CurrentPlayersText { get; }

        Data.ISteamGameServerPlayer[] CurrentPlayers { get; }

        /// <summary>
        /// QueryPort für Serverstates
        /// </summary>
        int QueryPort { get;   }

        long Ping { get; }

    }
}
