using System;
using System.IO;
using System.Linq;

using Onlab.BLL; //TODO: remove



namespace Onlab.DAL
{
    /*
    Class: AppIO
    State: Under construction | DEBUG
    Description:
        Handles input and output operations of the application.
        Static class because it provides global functions.
    */
    public static class FileProvider
    {
        public static bool MediaPathIsValid(string path) //determines whether the given media folder is valid
        {
            DirectoryInfo media = new DirectoryInfo(path);

            if (!media.Exists) return false;
            if (media.Attributes != FileAttributes.Directory) return false; //just in case

            return true; //none of the criteria returned false
        }
        public static void CacheOfflineFilesFromDriveSearch(LocalMediaPack lmp, ExtensionType type, string driveLetter)
        {
            DriveInfo drive = null;
            string searchPattern = "*." + type.ToString().ToLower(); //case shouldnt matter, but its probably safer in lower case

            foreach (DriveInfo driveCandidate in DriveInfo.GetDrives())
            {
                if (driveCandidate.Name == driveLetter)
                {
                    drive = driveCandidate;
                    break;
                }
            }

            foreach (string file in Directory.GetFiles(drive.RootDirectory.FullName, searchPattern, SearchOption.AllDirectories))
            {
                lmp.AddFilePath(file, type);
            }
        }
        public static void CacheOfflineFilesFromPath(LocalMediaPack lmp, string path)
        {
            RecursiveDirectorySearch(lmp, path);
        }

        private static void RecursiveDirectorySearch(LocalMediaPack lmp, string path) //TODO: avoid LMP param to avoid stack overflow
        {
            foreach (ExtensionType ext in Enum.GetValues(typeof(ExtensionType)).Cast<ExtensionType>()) //casting to get typed iteration, just in case
            {
                string searchPattern = "*." + ext.ToString().ToLower();
                foreach (string file in Directory.GetFiles(path, searchPattern))
                {
                    lmp.AddFilePath(file, ext);
                }
            }

            foreach (string dir in Directory.GetDirectories(path))
            {
                RecursiveDirectorySearch(lmp, dir);
            }
        }
    }
}