﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Xml;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Win32;
using ArmaServerBrowser.Data.DefaultImpl;

namespace ArmaServerBrowser.Data
{
    internal static class DataManager
    {
        public static IArma3DataRepository CreateNewDataRepository()
        {
            return new DataRepository();
        }

        public static IServerRepository CreateNewServerRepository()
        {
            return new ServerRepositoryHtml();
        }

        public static IArmaBrowserServerRepository CreateNewArmaBrowserServerRepository()
        {
            return new ArmaBrowserServerRepository();
        }
    }
}


//http://api.steampowered.com/ISteamNews/GetNewsForApp/V0001/?format=json&appid=107410&count=3
//http://api.steampowered.com/ISteamWebAPIUtil/GetSupportedAPIList/v0001/?format=xml
//http://api.steampowered.com/ISteamUserStats/GetNumberOfCurrentPlayers/v0001/?format=json&appid=107410