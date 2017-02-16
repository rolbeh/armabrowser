using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArmaBrowser.Data.DefaultImpl
{
    class ArmaAddon : IArmaAddon
    {
        public ArmaAddon()
        {
            KeyNames = Enumerable.Empty<AddonKey>();
        }

        public string Name { get; internal set; }

        public string DisplayText { get; internal set; }

        public string Version { get; internal set; }

        public string Path { get; internal set; }

        public string ModName { get; set; }

        public IEnumerable<AddonKey> KeyNames { get; set; }
    }
}
