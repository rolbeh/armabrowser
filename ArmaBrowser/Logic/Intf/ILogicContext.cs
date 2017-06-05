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

        void LookForArmaPath();

        void ReloadServerItems(IEnumerable<System.Net.IPEndPoint> lastAddresses, CancellationToken cancellationToken);

        void ReloadServerItem(System.Net.IPEndPoint iPEndPoint, CancellationToken cancellationToken);

        void Open(IServerItem _selectedServerItem, IAddon[] addon, bool runAsAdmin = false);

        void RefreshServerInfo(IServerItem[] items);

        Task RefreshServerInfoAsync(IServerItem[] items);

        //void TestJson();

        //event EventHandler<string> LiveAction;

        string ArmaVersion { get; }
    }
}
