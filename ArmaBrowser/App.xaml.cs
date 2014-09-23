using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ArmaBrowser
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {


            if (!ArmaBrowser.Properties.Settings.Default.Upgraded)
            {
                ArmaBrowser.Properties.Settings.Default.Upgrade();
                ArmaBrowser.Properties.Settings.Default.Upgraded = true;
                ArmaBrowser.Properties.Settings.Default.Save();
            }


            MainWindow = new MainWindow();
            MainWindow.Show();

        }

    }
}
