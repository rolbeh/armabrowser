using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmaBrowser.Logic
{
    internal interface IAddon  
    {
        string Name { get; }
        string ModName { get; }
        string DisplayText { get; }
        string Version { get; }
        bool IsActive { get; set; }
        bool CanActived { get; set; }
        long ActivationOrder { get;}
        IEnumerable<string> KeyNames { get; }
    }
}
