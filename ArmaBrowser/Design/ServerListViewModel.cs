using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmaBrowser.Design
{
    class DesignServerListViewModel
    {
        public DesignServerListViewModel()
        {
            ServerItemsView = new ServerItem[]
            { 
                new ServerItem
                {   
                    Name = "Recently",
                    GroupName = Logic.ServerItemGroup.Recently
                },
                 new ServerItem
                {   
                    Name = "Favorite",
                    GroupName = Logic.ServerItemGroup.Favorite
                },
                 new ServerItem
                {   
                    Name = "Found",
                    GroupName = Logic.ServerItemGroup.Found
                },
            };
        }

        public IEnumerable<ServerItem> ServerItemsView { get; private set; }

        public ServerItem SelectedServerItem { get; set; }
    }
}
