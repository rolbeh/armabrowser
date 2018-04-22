using System;
using System.Collections.Generic;
using System.Net;
using Magic.Steam;

namespace ArmaBrowser.Data
{
    interface IServerRepository
    {
        IEnumerable<ISteamGameServer> GetServerList();

        ISteamGameServer GetServerInfo(IPEndPoint gameServerQueryEndpoint);
    }
}
