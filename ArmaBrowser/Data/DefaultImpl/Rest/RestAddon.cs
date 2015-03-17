using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmaBrowser.Data.DefaultImpl.Rest
{
    class RestAddon
    {
        public string Name { get; set; }
        public string ModName { get; set; }
        public string DisplayText { get; set; }
        public string Version { get; set; }
        public RestAddonKey[] Keys { get; set; }
    }
    
    class RestAddonKey
    {
        public string Key { get; set; }
        public string PubK { get; set; }
    }


}
