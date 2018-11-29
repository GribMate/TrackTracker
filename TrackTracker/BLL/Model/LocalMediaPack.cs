using System;
using System.Collections.Generic;
using System.Text;

using TrackTracker.BLL.Enums;
using TrackTracker.BLL.Model.Base;



namespace TrackTracker.BLL.Model
{
    /*
     * Represents a collection of offline files (paths) on local computer. Attained by grouped search (either an external drive or a folder).
     * Collections of this class is stored in LocalMediaPackContainer class and persisted in database.
    */
    public class LocalMediaPack : SelectableObject
    {
        private Dictionary<string, SupportedFileExtension> filePaths; // The found paths (each path is unique)

        public LocalMediaPack(string rootPath, bool isResultOfDriveSearch, SupportedFileExtension baseExtension = SupportedFileExtension.Unknown)
        {
            filePaths = new Dictionary<string, SupportedFileExtension>();

            RootPath = rootPath;
            BaseExtension = baseExtension; // Will not be used if IsResultOfDriveSearch = false
            IsResultOfDriveSearch = isResultOfDriveSearch;
        }

        public string RootPath { get; private set; } // The root of the LMP can be the drive letter(external drive search) or the root folder path(folder search)
        public SupportedFileExtension BaseExtension { get; private set; } // Only relevant if the pack contains the results of an external drive search
        public bool IsResultOfDriveSearch { get; private set; } // Decides between the two supported modes of searches


        // TODO: consider workaround or place elsewhere
        public static string GetRootPathFromFormattedName(string formattedName) // Returns the rootPath of any LMP object by its formatted name
        {
            if (formattedName.Contains("|")) // Is a result of drive search
            {
                string[] splitted = formattedName.Split('|');
                return splitted[0].Trim(); // Deleting whitespace before "|"
            }
            else return formattedName; // Result of folder search
        }


        public bool AddFilePath(string path, SupportedFileExtension type) // Adds a new path of a file inside the LMP's root directory
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path), $"Cannot add file path, since it is null.");
            if (path.Length < 8)
                throw new ArgumentException($"Cannot add file path ({path}), it is too short.", nameof(path)); // "C:\x.abc" is 8 characters long

            if (filePaths.ContainsKey(path))
                return false; // Cannot add, already added, use ModifyFilePath() instead
            else
            {
                filePaths.Add(path, type);
                return true; // Added successfully
            }
        }
        public bool ModifyFilePath(string oldPath, string newPath, SupportedFileExtension newType) // Changes an already existing file path
        {
            if (oldPath == null || newPath == null)
                throw new ArgumentNullException($"Cannot add file path, since one or both of the parameters ({oldPath}, {newPath}) are null.");
            if (oldPath.Length < 8 || newPath.Length < 8)
                throw new ArgumentException($"Cannot add file path, one or both of the paramteres is too short ({oldPath}, {newPath})."); // "C:\x.abc" is 8 characters long

            if (!filePaths.ContainsKey(oldPath))
                return false; // Cannot modify non existent entry, use AddFilePath() instead
            else
            {
                filePaths.Remove(oldPath);
                filePaths.Add(newPath, newType);
                return true; // Added successfully
            }
        }
        public bool DeleteFilePath(string path) // Deletes an already existing file path
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path), $"Cannot add file path, since it is null.");
            if (path.Length < 8)
                throw new ArgumentException($"Cannot add file path ({path}), it is too short.", nameof(path)); // "C:\x.abc" is 8 characters long

            if (!filePaths.ContainsKey(path))
                return false; // Cannot delete non existent entry
            else
            {
                filePaths.Remove(path);
                return true; // Deleted successfully
            }
        }

        public bool GetFileSupportedFileExtension(string path, out SupportedFileExtension type) // Returns an already existing path's extension type
        {
            // We don't have to re-write "get value" logic, since Dictionary already provides it,
            // however, we still want to hide the internal data structure
            return filePaths.TryGetValue(path, out type);
        }
        public Dictionary<string, SupportedFileExtension> GetAllFilePaths() // Returns all of the file paths if any
        {
            return new Dictionary<string, SupportedFileExtension>(filePaths); // Copying to avoid modification via reference
        }
        public string GetFormattedName() // Returns the displayable name of the object
        {
            StringBuilder formattedName = new StringBuilder();

            formattedName.Append(RootPath);

            if (IsResultOfDriveSearch)
            {
                formattedName.Append(" | ");
                formattedName.Append(BaseExtension.ToString());
                formattedName.Append(" file search");
            }

            return formattedName.ToString();
        }
    }
}
