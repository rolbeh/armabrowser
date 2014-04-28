using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArmaServerBrowser.Data.DefaultImpl
{
    class ArmaAddOn : IArmaAddOn
    {
        public string Name { get; internal set; }

        public string DisplayText { get; internal set; }

        public string Version { get; internal set; }

        public string Path { get; set; }

        public string ModName { get; set; }
    }
}
