using System;
using System.Linq;
using System.Collections.Generic;

using Onlab.DAL;



namespace Onlab.BLL
{
    public static class GlobalAlgorithms
    {
        public static void Initialize()
        {
            GlobalVariables.Initialize();
        }

        public static void LoadOfflineFilesFromDrive(IFileProvider provider, LocalMediaPack lmp, string driveLetter, ExtensionType type)
        {
            List<string> paths = provider.GetAllFilesFromDrive(driveLetter, type.ToString());
            foreach (string path in paths)
            {
                lmp.AddFilePath(path, type);
            }
        }
        public static void LoadOfflineFilesFromDirectory(IFileProvider provider, LocalMediaPack lmp, string path)
        {
            foreach (ExtensionType ext in Enum.GetValues(typeof(ExtensionType)).Cast<ExtensionType>()) //casting to get typed iteration, just in case
            {
                List<string> paths = provider.GetAllFilesFromDirectory(path, ext.ToString());
                foreach (string currPath in paths)
                {
                    lmp.AddFilePath(currPath, ext);
                }
            }
        }
    }
}
