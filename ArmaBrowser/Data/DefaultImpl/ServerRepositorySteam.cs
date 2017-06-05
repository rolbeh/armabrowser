using System;
using System.Linq;
using System.Net;
using Magic.Steam;

namespace ArmaBrowser.Data.DefaultImpl
{
    internal sealed class ServerRepositorySteam : IServerRepository
    {
        private const string SteamGameNameFilter = "Arma3";
        
        #region IServerRepository

        public IPEndPoint[] GetServerEndPoints()
        {
            throw new NotSupportedException();
        }

        public ISteamGameServer[] GetServerList(Action<ISteamGameServer> itemGenerated)
        {
            // https://developer.valvesoftware.com/wiki/Master_Server_Query_Protocol
            
            return ServerQueries.GetServerList(SteamGameNameFilter, null)
                .Select(ep => (ISteamGameServer)new ServerItem{Host =  ep.Host, QueryPort = ep.QueryPort} )
                .ToArray();
        }

        public ISteamGameServer GetServerInfo(IPEndPoint gameServerQueryEndpoint)
        {
            return ServerQueries.GetServerInfo(gameServerQueryEndpoint);
        }

        #endregion IServerRepository
        
    }
}
