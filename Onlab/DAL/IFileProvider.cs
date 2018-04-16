using System.Collections.Generic;



namespace Onlab.DAL
{
    /*
    Interface: IFileProvider
    Description:
        Handles input and output operations on the file system.
    */
    public interface IFileProvider
    {
        bool MediaPathIsValid(string path); //determines whether the given media folder is valid
        bool FileExists(string path); //determines wether a given file path is valid and the file does exist
        List<string> GetAllFilesFromDrive(string driveLetter, string extension); //returns all file paths from a given drive with a given extension
        List<string> GetAllFilesFromDirectory(string path, string extension); //return all file paths under a given directory (and all it's children) with a given extension
    }
}
