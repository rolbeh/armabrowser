using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArmaBrowser.Data
{
    internal interface IArmaAddOn
    {
        string Name { get; }
        string ModName { get; }
        string DisplayText { get; }
        string Version { get; }
        IEnumerable<string> KeyNames { get; }
    }
}
