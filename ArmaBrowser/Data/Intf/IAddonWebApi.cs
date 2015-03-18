using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArmaBrowser.Logic;

namespace ArmaBrowser.Data.Intf
{
    interface IAddonWebApi
    {
        void PostInstalledAddonsKeysAsync(IEnumerable<IAddon> addons);
    }
}
