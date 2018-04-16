using System;
using System.Collections.Generic;
using System.Text;

namespace Onlab.BLL
{
    public class LocalMediaPack
    {
        private string rootPath;
        private ExtensionType baseExtension;
        private Dictionary<string, ExtensionType> filePaths;
        private bool isOnRemovableDrive;
        private bool isResultOfDriveSearch;

        public string RootPath
        {
            get => rootPath;
        }

        public LocalMediaPack(string rootPath, bool isOnRemovableDrive, bool isResultOfDriveSearch, ExtensionType baseExtension = ExtensionType.MP3)
        {
            this.rootPath = rootPath;
            this.baseExtension = baseExtension; //will not be used if isResultOfDriveSearch = false
            this.filePaths = new Dictionary<string, ExtensionType>();
            this.isOnRemovableDrive = isOnRemovableDrive;
            this.isResultOfDriveSearch = isResultOfDriveSearch;
        }

        public Dictionary<string, ExtensionType> GetFilePaths
        {
            get => filePaths;
        }

        public static string GetRootPathFromFormattedName(string formattedName)
        {
            if (formattedName.Contains("|"))
            {
                string[] splitted = formattedName.Split('|');
                return splitted[0].Trim();
            }
            else return formattedName;
        }

        public void AddFilePath(string path, ExtensionType type)
        {
            filePaths.Add(path, type);
        }
        public string GetFormattedName()
        {
            StringBuilder formattedName = new StringBuilder();

            formattedName.Append(rootPath);
            if (isResultOfDriveSearch)
            {
                formattedName.Append(" | ");
                formattedName.Append(baseExtension.ToString());
                formattedName.Append(" file search ");
                if (isOnRemovableDrive) formattedName.Append("[Removable]");
            }

            return formattedName.ToString();
        }
    }
}
