using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArmaBrowser.ViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntegrationsTest.ArmaBrowser
{

    [TestClass]
    public class UpdateAvailableViewModelTests
    {
        [TestMethod, TestCategory("onlylocal")]
        public async Task CheckForUdpateTest()
        {
            // given
            UpdateAvailableViewModel viewModel = new UpdateAvailableViewModel();

            // when
            await viewModel.CheckForNewReleases();
        }
    }
}
