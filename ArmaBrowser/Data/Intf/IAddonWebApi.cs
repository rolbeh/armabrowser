using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArmaBrowser.Logic;

namespace ArmaBrowser.Data
{
    interface IAddonWebApi
    {
        Task PostInstalledAddonsKeysAsync(IEnumerable<IAddon> addons);
    }
}
