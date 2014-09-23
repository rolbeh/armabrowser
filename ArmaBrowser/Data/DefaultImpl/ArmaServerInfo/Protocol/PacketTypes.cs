using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmaBrowser.Data.DefaultImpl.ArmaServerInfo.Protocol
{
    /// <summary>
    /// Packet types
    /// </summary>
    public enum PacketTypes
    {
        Challenge = 0x09,
        ServerInfo = 0x00
    }
}
