using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ArmaServerBrowser.Logic
{
    internal interface ILogicContext
    {
        ObservableCollection<IServerItem> ServerItems { get; }

        Collection<IAddon> Addons { get; }

        string ArmaPath { get; }

        void ReloadServerItems(string filterByHostOrMission, CancellationToken cancellationToken);


        void Open(IServerItem _selectedServerItem, IAddon[] addon, bool runAsAdmin = false);

        void RefreshServerInfo(IServerItem item);

        void TestJson();
    }
}
