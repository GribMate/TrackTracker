using System;
using System.Collections.Generic;
using System.Text;



namespace Onlab.BLL
{
    /*
    Class: LocalMediaPack
    Description:
        Represents a collection of offline files (paths) on local computer. Attained by grouped search (either an external drive or a folder).
        Collections of this class is stored in LocalMediaPackContainer class and persisted in database.
    */
    public class LocalMediaPack
    {
        private string rootPath; //the root of the LMP can be the drive letter (external drive search) or the root folder path (folder search)
        private ExtensionType baseExtension; //only relevant if the pack contains the results of an external drive search
        private Dictionary<string, ExtensionType> filePaths; //the found paths (each path is unique)
        private bool isResultOfDriveSearch; //decides between the two supported modes of searches

        public static string GetRootPathFromFormattedName(string formattedName) //returns the rootPath of any LMP object by its formatted name
        {
            if (formattedName.Contains("|")) //is a result of drive search
            {
                string[] splitted = formattedName.Split('|');
                return splitted[0].Trim(); //deleting whitespace before "|"
            }
            else return formattedName; //result of folder search
        }

        public LocalMediaPack(string rootPath, bool isResultOfDriveSearch, ExtensionType baseExtension = ExtensionType.MP3)
        {
            this.rootPath = rootPath;
            this.baseExtension = baseExtension; //will not be used if isResultOfDriveSearch = false
            this.filePaths = new Dictionary<string, ExtensionType>();
            this.isResultOfDriveSearch = isResultOfDriveSearch;
        }

        public string RootPath
        {
            get => rootPath;
        }
        public ExtensionType BaseExtension
        {
            get => baseExtension;
        }
        public bool IsResultOfDriveSearch
        {
            get => isResultOfDriveSearch;
        }

        public bool AddFilePath(string path, ExtensionType type) //adds a new path of a file inside the LMP's root directory
        {
            if (path is null) throw new ArgumentNullException();
            if (path.Length < 8) throw new ArgumentException(); //"C:\x.mp3" is 8 chars

            if (filePaths.ContainsKey(path)) return false; //cannot add, already added, use ModifyFilePath() instead
            else
            {
                filePaths.Add(path, type);
                return true; //added successfully
            }
        }
        public bool ModifyFilePath(string oldPath, string newPath, ExtensionType newType) //changes an already existing file path
        {
            if (oldPath is null || newPath is null) throw new ArgumentNullException();
            if (oldPath.Length < 8 || newPath.Length < 8) throw new ArgumentException(); //"C:\x.mp3" is 8 chars

            if (!filePaths.ContainsKey(oldPath)) return false; //cannot modify non existent entry, use AddFilePath() instead
            else
            {
                filePaths.Remove(oldPath);
                filePaths.Add(newPath, newType);
                return true; //added successfully
            }
        }
        public bool DeleteFilePath(string path) //deletes an already existing file path
        {
            if (path is null) throw new ArgumentNullException();
            if (path.Length < 8) throw new ArgumentException(); //"C:\x.mp3" is 8 chars

            if (!filePaths.ContainsKey(path)) return false; //cannot delete non existent entry
            else
            {
                filePaths.Remove(path);
                return true; //deleted successfully
            }
        }
        public bool GetFileExtensionType(string path, out ExtensionType type) //returns an already existing path's extension type
        {
            //we don't have to re-write "get value" logic, since Dictionary already provides it
            //however, we still want to hide the internal data structure
            return filePaths.TryGetValue(path, out type);
        }
        public Dictionary<string, ExtensionType> GetAllFilePaths() //returns all of the file paths if any
        {
            return new Dictionary<string, ExtensionType>(filePaths); //copying to avoid modification via reference
        }
        public string GetFormattedName() //returns the displayable name of the object
        {
            StringBuilder formattedName = new StringBuilder();

            formattedName.Append(rootPath);
            if (isResultOfDriveSearch)
            {
                formattedName.Append(" | ");
                formattedName.Append(baseExtension.ToString());
                formattedName.Append(" file search");
            }

            return formattedName.ToString();
        }
    }
}
