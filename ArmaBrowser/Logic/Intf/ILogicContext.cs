using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ArmaBrowser.Logic
{
    internal interface ILogicContext
    {
        ObservableCollection<IServerItem> ServerItems { get; }

        Collection<IAddon> Addons { get; }

        string ArmaPath { get; }

        void ReloadServerItems(IEnumerable<System.Net.IPEndPoint> _lastAddresses, CancellationToken cancellationToken);

        void Open(IServerItem _selectedServerItem, IAddon[] addon, bool runAsAdmin = false);

        void RefreshServerInfo(IServerItem item);

        Task RefreshServerInfoAsync(IServerItem item);

        void TestJson();
    }
}
