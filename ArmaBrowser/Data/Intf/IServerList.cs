using System;
using System.Net;
using Magic.Steam;

namespace ArmaBrowser.Data
{
    interface IServerRepository
    {
        ISteamGameServer[] GetServerList(Action<ISteamGameServer> itemGenerated = null);

        ISteamGameServer GetServerInfo(IPEndPoint gameServerQueryEndpoint);
    }
}
