using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmaBrowser.Data.Intf.Asb
{
    interface IHostItem
    {
        Guid Host { get; set; }
        string HostIp { get; set; }
        int Port { get; set; }
        string Name { get; set; }
        string Version { get; set; }
        
    }

    interface IModItem
    {
        string Name { get; set; }
        string Version { get; set; }
        string FilesHash { get; set; }
        string SupportUrl { get; set; }
        string InstallUrl { get; set; }
    }

    interface IHostStartConfig
    {
        
    }
}
