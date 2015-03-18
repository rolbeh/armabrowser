using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmaBrowser.Data
{
    internal interface ISteamGameServerPlayer
    {
        byte Idx { get; }

        string Name { get; }

        Int32 Score { get; }

        TimeSpan OnlineTime { get; }

    }
}
