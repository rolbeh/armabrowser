using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace ArmaBrowser.Logic
{
    internal sealed class AppPathService
    {
        public string UserSettingsPath =>
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile,
                    Environment.SpecialFolderOption.DoNotVerify), "ArmaBrowser");

        public void EnsureDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}
