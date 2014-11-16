using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ArmaBrowser.Data.DefaultImpl;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void SmallVdf()
        {
            using (SteamConfigReader reader = new SteamConfigReader(@"C:\Program Files (x86)\Steam\SteamApps\libraryfolders.vdf"))
            {
                var smal = reader.ToXml();
            }
        }

        [TestMethod]
        public void BigVdf()
        {
            using (SteamConfigReader reader = new SteamConfigReader(@"C:\Program Files (x86)\Steam\config\config.vdf"))
            {
                var big = reader.ToXml();
            }
        }
    }
}
