using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmaServerBrowser.Data 
{
    internal interface IArma3DataRepository
    {
        IServerVo[] GetServerList();

        string GetArma3Folder();

        IArmaAddOn[] GetInstalledAddons(string baseFolder);

        IServerRepository ServerListManager();
    }
}
