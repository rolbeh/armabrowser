using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmaServerBrowser.Logic
{
    public interface IServerItem
    {
        string Gamename { get; }
        string Mode { get; }
        string Name { get; }
        string Mission { get; }
        string Country { get; }
        string Host { get; }
        int Port { get; }
        string Version { get; }
        string ModsText { get; }
        string[] Mods { get;}
        string Modhashs { get; }
        int Players { get; }
        string Island { get; }
        string Signatures { get; }

        string FullText { get; }
    }
}
