using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;
using System.Text;
using ArmaBrowser.Helper;

namespace ArmaBrowser
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    partial class App : Application
    {
        public App() : base()
        {

            this.DispatcherUnhandledException += App_DispatcherUnhandledException;

            if (string.IsNullOrEmpty(ArmaBrowser.Properties.Settings.Default.Id)
                || ArmaBrowser.Properties.Settings.Default.Id.Length != 32)
            {
                ArmaBrowser.Properties.Settings.Default.Id = Guid.NewGuid().ToByteArray().ToHexString();
                ArmaBrowser.Properties.Settings.Default.Save();

            }

            if (!ArmaBrowser.Properties.Settings.Default.Upgraded)
            {
                ArmaBrowser.Properties.Settings.Default.Upgrade();
                ArmaBrowser.Properties.Settings.Default.Upgraded = true;
                ArmaBrowser.Properties.Settings.Default.Save();
            }
        }


        private void Application_Startup(object sender, StartupEventArgs e)
        {
            MainWindow = new MainWindow();
            MainWindow.Show();

        }

        
        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            Debug.WriteLine(e.Exception);
            try
            {
                var exception = e.Exception as AggregateException;
                if (exception != null)
                {
                    var i = 0;
                    foreach (var item in exception.InnerExceptions)
                    {
                        i++;
                        Debug.WriteLine(i.ToString() + ". Exception: ");

                        Debug.WriteLine(item);
                        Debug.WriteLine("----------------------------");
                    }
                }
            }
            catch 
            {
                
                
            }
            
        }
    }
}
