using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmaBrowser.Data 
{
    internal interface IArma3DataRepository
    {
        string GetArma3Folder();

        IArmaAddon[] GetInstalledAddons(string baseFolder);

        //IServerRepository ServerListManager();
    }
}
