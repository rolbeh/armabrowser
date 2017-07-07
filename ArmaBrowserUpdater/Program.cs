using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ArmaBrowserUpdater
{
    class Program
    {
        static void Main(string[] args)
        {
            if (IsUpdated())
            {
                return;
            }

            if (args.Length < 2 || !File.Exists(args[0]) || !Directory.Exists(args[1]))
            {
                Updated("ERROR_ARGUMENT");
                return;
            }

            int argIdx = 0;
            if ((argIdx = ((IList<string>) args).IndexOf("--wait-exit-pid")) > -1
                && args.Length > argIdx)
            {
                int pid = 0;
                if (int.TryParse(args[argIdx + 1], out pid))
                {
                    var waitForExsitProcess = Process.GetProcessById(pid);
                    if (!waitForExsitProcess.WaitForExit(15000))
                    {
                        waitForExsitProcess.Kill();
                    }
                    if (!waitForExsitProcess.WaitForExit(15000))
                    {
                        Updated("ERROR_WAITFOREXIT");
                        return;
                    }
                }
            }
            bool rollback = true;
            string sourceFile = args[0];
            string destinationPath = args[1];
            string destinationBackupPath = args[1] + "backup";
            try
            {
                if (Directory.Exists(destinationBackupPath))
                {
                    Directory.Delete(destinationBackupPath, true);
                }
                Directory.Move(destinationPath, destinationBackupPath);

                ZipFile.ExtractToDirectory(sourceFile, destinationPath);

                rollback = false;
                Updated("SUCCESSFUL");
            }
            catch (Exception)
            {
                rollback = true;
            }
            finally
            {
                if (rollback)
                {
                    if (Directory.Exists(destinationBackupPath))
                    {
                        Directory.Delete(destinationPath, true);
                        Directory.Move(destinationBackupPath, destinationPath);
                    }
                    Updated("rollback");
                }
                StartArmaBrowser(destinationPath);
                if (Directory.Exists(destinationBackupPath))
                {
                    Directory.Delete(destinationBackupPath, true);
                }
            }
        }

        private static void StartArmaBrowser(string destinationPath)
        {
            string path = Path.Combine(destinationPath, "ArmaBrowser.exe");
            if (!File.Exists(path)) {return;}
            ProcessStartInfo psInfo = new ProcessStartInfo(path)
            {
                UseShellExecute = true
            };
            Process.Start(psInfo);
        }


        private static bool IsUpdated()
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            return File.Exists(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().FullName), "done"));
        }

        private static void Updated(string state)
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            using (var fileStream = File.Open(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().FullName), "done"), FileMode.OpenOrCreate))
            using (var bw = new BinaryWriter(fileStream))
            {
                fileStream.SetLength(0);
                if (state != null)
                {
                    bw.Write(Encoding.ASCII.GetBytes(state));
                }
            }
        }
    }
}