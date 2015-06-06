using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Xml;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Win32;
using ArmaBrowser.Data.DefaultImpl;

namespace ArmaBrowser.Data
{
    internal static class DataManager
    {
        public static IArma3DataRepository CreateNewDataRepository()
        {
            return new DataRepository();
        }

        public static ServerRepositorySteam CreateNewServerRepository()
        {
            return new ServerRepositorySteam();
        }

        public static IAddonWebApi CreateNewArmaBrowserServerRepository()
        {
            return new AddonWebApi();
        }
    }
}


//http://api.steampowered.com/ISteamNews/GetNewsForApp/V0001/?format=json&appid=107410&count=3
//http://api.steampowered.com/ISteamWebAPIUtil/GetSupportedAPIList/v0001/?format=xml
//http://api.steampowered.com/ISteamUserStats/GetNumberOfCurrentPlayers/v0001/?format=json&appid=107410