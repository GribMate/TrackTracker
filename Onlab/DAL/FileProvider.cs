using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;



namespace Onlab.DAL
{
    /*
    Class: FileProvider
    Description:
        Implements IFileProvider on Windows file systems.
    */
    public class FileProvider : IFileProvider
    {
        //TODO: try-catch error handling

        private List<string> paths; //helper variable to hold paths for recursive search (avoid stack overflow via parameter)
        private string extension; //helper variable to hold supported extension for recursive search (avoid stack overflow via parameter)

        public FileProvider()
        {
            paths = new List<string>();
            extension = null; //must be overwritten by function calls
        }

        public bool MediaPathIsValid(string path) //determines whether the given media folder is valid
        {
            DirectoryInfo media = new DirectoryInfo(path);

            if (!media.Exists) return false; //exists
            if (media.Attributes != FileAttributes.Directory) return false; //is a directory

            return true; //none of the criteria returned false
        }
        public bool FileExists(string path) //determines wether a given file path is valid and the file does exist
        {
            FileInfo file = new FileInfo(path);

            if (!file.Exists) return false; //exists
            if (file.Attributes == FileAttributes.Directory) return false; //is NOT a directory

            return true; //none of the criteria returned false
        }
        public List<string> GetAllFilesFromDrive(string driveLetter, string extension) //returns all file paths from a given drive with a given extension
        {
            paths.Clear(); //clearing helper variable from previous function data

            DriveInfo drive = null;
            string searchPattern = "*." + extension.ToLower(); //case shouldnt matter, but its probably safer in lower case
            foreach (DriveInfo driveCandidate in DriveInfo.GetDrives()) //selecting the given drive
            {
                if (driveCandidate.Name == driveLetter)
                {
                    drive = driveCandidate;
                    break; //every drive letter must be distinct; first hit = last hit
                }
            }
            foreach (string file in Directory.GetFiles(drive.RootDirectory.FullName, searchPattern, SearchOption.AllDirectories))
            {
                paths.Add(file);
            }

            return new List<string>(paths); //we need to copy since List is a reference
        }
        public List<string> GetAllFilesFromDirectory(string path, string extension) //return all file paths under a given directory (and all it's children) with a given extension
        {
            paths.Clear(); //clearing helper variable from previous function data
            this.extension = extension; //setting the helper variable to hold supported extension

            RecursiveDirectorySearch(path); //helper function to implement recursive search into this.paths List

            return new List<string>(paths); //we need to copy since List is a reference
        }

        private void RecursiveDirectorySearch(string path)
        {
            string searchPattern = "*." + extension.ToLower(); //case shouldnt matter, but its probably safer in lower case
            foreach (string file in Directory.GetFiles(path, searchPattern))
            {
                paths.Add(file);
            }
            foreach (string dir in Directory.GetDirectories(path))
            {
                RecursiveDirectorySearch(dir); //recursive call on child directory
            }
        }
    }
}