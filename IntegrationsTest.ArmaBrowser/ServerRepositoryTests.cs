using ArmaBrowser.Data;
using ArmaBrowser.Data.DefaultImpl;
using Magic.Steam;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntegrationsTest.ArmaBrowser
{
    [TestClass]
    public class ServerRepositoryTests
    {
        [TestMethod]
        public void GetServerListTest()
        {
            //given 
            IServerRepository serverRepository = new ServerRepositorySteam();

            // when 
            ISteamGameServer[] serverList = serverRepository.GetServerList();

            // Then

            Assert.IsTrue(serverList.Length > 0);
        }
    }
}
