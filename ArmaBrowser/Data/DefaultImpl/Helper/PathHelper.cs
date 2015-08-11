using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ArmaBrowser.Data.DefaultImpl.Helper
{
    class PathHelper
    {

        public static string GetRelativePath(string fromPath, string toPath)
        {
            int fromAttr = GetPathAttribute(fromPath);
            int toAttr = GetPathAttribute(toPath);

            StringBuilder path = new StringBuilder(260); // MAX_PATH
            if (PathRelativePathTo(
                path,
                fromPath,
                fromAttr,
                toPath,
                toAttr) == 0)
            {
                throw new ArgumentException("Paths must have a common prefix");
            }
            return path.ToString();
        }

        private static int GetPathAttribute(string path)
        {
            DirectoryInfo di = new DirectoryInfo(path);
            if (di.Exists)
            {
                return FILE_ATTRIBUTE_DIRECTORY;
            }

            FileInfo fi = new FileInfo(path);
            if (fi.Exists)
            {
                return FILE_ATTRIBUTE_NORMAL;
            }

            throw new FileNotFoundException();
        }

        private const int FILE_ATTRIBUTE_DIRECTORY = 0x10;
        private const int FILE_ATTRIBUTE_NORMAL = 0x80;
        //https://stackoverflow.com/questions/275689/how-to-get-relative-path-from-absolute-path
        [DllImport("shlwapi.dll", SetLastError = true)]
        private static extern int PathRelativePathTo(StringBuilder pszPath,
            string pszFrom, int dwAttrFrom, string pszTo, int dwAttrTo);



        public static long CalculateFolderSize(string folder)
        {
            long folderSize = 0;

            if (!Directory.Exists(folder))
                return folderSize;

            foreach (string file in Directory.GetFiles(folder))
            {
                try
                {
                    if (File.Exists(file))
                    {
                        FileInfo finfo = new FileInfo(file);
                        folderSize += finfo.Length;
                    }
                }
                catch (NotSupportedException e)
                {
                    Console.WriteLine("Unable to calculate folder size: {0}", e.Message);
                }
                catch (UnauthorizedAccessException e)
                {
                    Console.WriteLine("Unable to calculate folder size: {0}", e.Message);
                }
            }

            folderSize += Directory.GetDirectories(folder).Sum(dir => CalculateFolderSize(dir));

            return folderSize;
        }
    }
}
