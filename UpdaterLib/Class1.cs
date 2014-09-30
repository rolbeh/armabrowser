using System;
using System.Collections.Generic;
using System.Deployment.Application;
using System.Diagnostics;
using System.Linq;
using System.Text;
using DownloadProgressChangedEventArgs = System.Net.DownloadProgressChangedEventArgs;

namespace SilentlInstaller
{
        private class MyInstaller
        {
            private string AppName = "";
            InPlaceHostingManager iphm = null;
            public void InstallApplication(string deployManifestUriStr)
            {
                try
                {
                    Uri deploymentUri = new Uri(deployManifestUriStr);
                    iphm = new InPlaceHostingManager(deploymentUri, false);
                }
                catch (UriFormatException uriEx)
                {
                    Console.WriteLine("Cannot install the application: " +
                                     "The deployment manifest URL supplied is not a valid URL. " +
                                     "Error: " + uriEx.Message);
                    return;
                }
                catch (PlatformNotSupportedException platformEx)
                {
                    Console.WriteLine("Cannot install the application: " +
                                    "This program requires Windows XP or higher. " +
                                    "Error: " + platformEx.Message);
                    return;
                }
                catch (ArgumentException argumentEx)
                {
                    Console.WriteLine("Cannot install the application: " +
                                    "The deployment manifest URL supplied is not a valid URL. " +
                                    "Error: " + argumentEx.Message);
                    return;
                }

                iphm.GetManifestCompleted += iphm_GetManifestCompleted;
                iphm.GetManifestAsync();
            }

            private void iphm_GetManifestCompleted(object sender, GetManifestCompletedEventArgs e)
            {
                // Check for an error. 
                if (e.Error != null)
                {
                    // Cancel download and install.
                    Console.WriteLine("Could not download manifest. Error: " + e.Error.Message);
                    return;
                }

                // bool isFullTrust = CheckForFullTrust(e.ApplicationManifest); 

                // Verify this application can be installed. 
                try
                {
                    // the true parameter allows InPlaceHostingManager 
                    // to grant the permissions requested in the applicaiton manifest.
                    iphm.AssertApplicationRequirements(true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred while verifying the application. " +
                                    "Error: " + ex.Message);
                    return;
                }

                // Use the information from GetManifestCompleted() to confirm  
                // that the user wants to proceed. 
                string appInfo = "Installiere " + e.ProductName;
                appInfo += "\nVersion: " + e.Version;
                appInfo += "\nSupport/Help Requests: " + (e.SupportUri != null
                                                              ? e.SupportUri.ToString()
                                                              : "N/A");

                Console.WriteLine(appInfo);

                // Download the deployment manifest. 
                iphm.DownloadProgressChanged += IphmOnDownloadProgressChanged;
                iphm.DownloadApplicationCompleted += iphm_DownloadApplicationCompleted;

                try
                {
                    // Usually this shouldn't throw an exception unless AssertApplicationRequirements() failed,  
                    // or you did not call that method before calling this one.
                    iphm.DownloadApplicationAsync();
                }
                catch (Exception downloadEx)
                {
                    Console.WriteLine("Cannot initiate download of application. Error: " + downloadEx.Message);
                    return;
                }
            }

            private void IphmOnDownloadProgressChanged(object sender,
                                                       System.Deployment.Application.DownloadProgressChangedEventArgs
                                                           downloadProgressChangedEventArgs)
            {
                //Console.Write(".");
            }

            /*
            private bool CheckForFullTrust(XmlReader appManifest)
            {
                if (appManifest == null)
                {
                    throw (new ArgumentNullException("appManifest cannot be null."));
                }

                XAttribute xaUnrestricted =
                    XDocument.Load(appManifest)
                        .Element("{urn:schemas-microsoft-com:asm.v1}assembly")
                        .Element("{urn:schemas-microsoft-com:asm.v2}trustInfo")
                        .Element("{urn:schemas-microsoft-com:asm.v2}security")
                        .Element("{urn:schemas-microsoft-com:asm.v2}applicationRequestMinimum")
                        .Element("{urn:schemas-microsoft-com:asm.v2}PermissionSet")
                        .Attribute("Unrestricted"); // Attributes never have a namespace

                if (xaUnrestricted != null)
                    if (xaUnrestricted.Value == "true")
                        return true;

                return false;
            }
            */

            private void iphm_DownloadApplicationCompleted(object sender, DownloadApplicationCompletedEventArgs e)
            {
                // Check for an error. 
                if (e.Error != null)
                {
                    // Cancel download and install.
                    Console.WriteLine("Could not download and install application. Error: " + e.Error.Message);
                    return;
                }

                // Inform the user that their application is ready for use. 
                Console.WriteLine(apppath); ;
                Process.Start(apppath);
                Process.GetCurrentProcess().Kill();

            }
        }

        //private static string apppath = @"\\deploy.applications.gkl.de\Applications\GKL.OfficeTest\GKL.Office.application";

        //static void Main(string[] args)
        //{
        //    if (args.Length > 0)
        //    {
        //        MyInstaller installer = new MyInstaller();
        //        apppath = args[0];
        //        installer.InstallApplication(apppath);
        //        Console.WriteLine("Installer gestartet");
        //        Console.ReadKey();
        //    }
        //}


    }
