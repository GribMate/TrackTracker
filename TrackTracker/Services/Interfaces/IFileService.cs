using System.Collections.Generic;



namespace Onlab.Services.Interfaces
{
    /*
    Interface: IFileService
    Description:
        Handles input and output operations on the file system.
    */
    public interface IFileService
    {
        bool MediaPathIsValid(string path); //determines whether the given media folder is valid
        bool FileExists(string path); //determines wether a given file path is valid and the file does exist
        string GetExtensionFromFilePath(string path); //returns only the extension part of a file path, without leading "."
        string GetFileNameFromFilePath(string path); //returns only the file name of a fully specified path, without the extension

        List<string> GetAllFilesFromDrive(string driveLetter, string extension); //returns all file paths from a given drive with a given extension
        List<string> GetAllFilesFromDirectory(string path, string extension); //return all file paths under a given directory (and all it's children) with a given extension
    }
}
