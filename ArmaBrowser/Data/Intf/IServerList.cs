using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ArmaBrowser.Data
{
    interface IServerRepository
    {
        IPEndPoint[] GetServerEndPoints();

        IServerVo[] GetServerList(string hostAndMissionFilter, Action<IServerVo> itemGenerated = null);

        IServerVo GetServerInfo(IPEndPoint gameServer);
    }
}
