using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmaServerBrowser.Data.DefaultImpl
{
    class ServerItem : IServerVo
    {
        public string Gamename { get; set; }

        public string Mission { get; set; }

        public string Country { get; set; }

        public string Host { get; set; }

        public int Port { get; set; }

        public string Version { get; set; }

        public string Name { get; set; }

        public string Mods { get; set; }

        public string Modhashs { get; set; }

        public int Players  { get; set; }

        public string Island { get; set; }

        public string Signatures { get; set; }

        public string Mode { get; set; }

        public bool Passworded { get; set; }

        public int MaxPlayers { get; set; }
    }
}
