using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmaBrowser.Data
{
    interface IServerQueryAddress
    {
        System.Net.IPAddress Host { get; }
        int QueryPort { get; }
    }

}
