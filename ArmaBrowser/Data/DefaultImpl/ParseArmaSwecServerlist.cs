using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ArmaBrowser.Data.DefaultImpl
{
    static class ParseArmaSwecServerlist
    {
        public static IServerVo[] GetServerList(string localPath)
        {
            using (FileStream fs = new FileStream(localPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                return GetServerList(fs);
            }
        }
            public static IServerVo[] GetServerList(Stream stream)
        {
            var result = new List<IServerVo>(5000);
            // read swec Serverlist

            using (XmlReader xr = new XmlTextReader(stream))
            {
                ReadToNextElement(xr);

                // Server
                if (xr.Name == "servers")
                {
                    // Server
                    DefaultImpl.ServerItem item = null;
                    while (ReadToNextElement(xr))
                    {
                        if (xr.Name == "server")
                        {
                            item = new DefaultImpl.ServerItem();
                            result.Add(item);
                        }
                        else
                        {
                            if (item == null) throw new Exception("Item is null");
                            AssingElementValue(xr, item);
                        }
                    }

                }
            }

            return result.ToArray();
        }

        static void AssingElementValue(XmlReader xr, DefaultImpl.ServerItem item)
        {
            var elementName = xr.Name;
            xr.Read();
            if (xr.NodeType == XmlNodeType.EndElement) return;
            if (xr.NodeType != XmlNodeType.Text) throw new Exception("Erwarete XmlNodeType.Text als NodeType");

            if (elementName == "name")
                item.Name = xr.Value;

            if (elementName == "host")
                item.Host = System.Net.IPAddress.Parse( xr.Value);

            if (elementName == "port")
                item.Port = Int32.Parse(xr.Value);

            if (elementName == "players")
                item.CurrentPlayerCount = Int32.Parse(xr.Value);

            if (elementName == "mission")
                item.Mission = xr.Value;

            if (elementName == "gamename")
                item.Gamename = xr.Value;

            if (elementName == "island")
                item.Map = xr.Value;

            if (elementName == "country")
                item.Country = xr.Value;

            if (elementName == "signatures")
                item.Signatures = xr.Value;

            if (elementName == "mod")
                item.Mods = xr.Value;

            if (elementName == "modhash")
                item.Modhashs = xr.Value;

            if (elementName == "mode")
                item.Mode = xr.Value;

            if (elementName == "version")
                item.Version = xr.Value;

            if (elementName == "version")
                item.Version = xr.Value;

            if (elementName == "passworded")
                item.Passworded = xr.Value == "true";

        }

        static bool ReadToNextElement(XmlReader xr)
        {
            while (xr.Read())
            {
                if (xr.NodeType == XmlNodeType.Element)
                    return true;
            }
            return false;
        }
    }
}
