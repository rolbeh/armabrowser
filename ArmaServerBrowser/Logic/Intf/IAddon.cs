using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmaServerBrowser.Logic
{
    internal interface IAddon  
    {
        string Name { get; }
        string ModName { get; }
        string DisplayText { get; }
        string Version { get; }
        bool IsActive { get; set; }
        long ActivationOrder { get;}
    }
}
