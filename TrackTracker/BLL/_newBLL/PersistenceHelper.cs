using System;
using System.Collections.Generic;

using TrackTracker.BLL.Enums;
using TrackTracker.Services.Interfaces;



namespace TrackTracker.BLL
{
    /*
     * Static helper class, that checks if it's the first run of the app and instantiates a new physical database if so,
     * or loads app level data (like settings, static environment variables, etc.) from an existing one.
    */
    public static class PersistenceHelper
    {
        public static void InitPersistenceOnStartup(IDatabaseService dbService, IFileService fileService)
        {
            if (!dbService.DatabaseExists)
                FirstRunSetup(dbService); // We don't have a database file, that means it's the first time the app runs
            else
                LoadPersistence(dbService, fileService); // App has run before, we need to load persistence from DB
        }

        private static void FirstRunSetup(IDatabaseService dbService) // Creates a new empty database and forms it's data structure
        {
            dbService.CreateDatabase(); // Creating empty
            dbService.FormatNewDatabase(); // Formatting existing
        }
        private static void LoadPersistence(IDatabaseService dbService, IFileService fileService) // Loads all data from an already existing database file
        {
            List<string[]> lmpRows = dbService.GetAllRows("LocalMediaPacks"); // Getting LocalMediaPack objects

            if (lmpRows.Count > 0)
            {
                foreach (string[] row in lmpRows)
                {
                    string rootPath = row[0];
                    SupportedFileExtension baseExtension = (row[1].Length > 0) ? (SupportedFileExtension)Enum.Parse(typeof(SupportedFileExtension), row[1]) : SupportedFileExtension.MP3; // Default type is MP3 if cell is null
                    bool isResultOfDriveSearch = Int32.Parse(row[2]) == 1;
                    string filePaths = row[3]; // Array of strings, divided by "|"

                    LocalMediaPack lmp = new LocalMediaPack(rootPath, isResultOfDriveSearch, baseExtension);
                    foreach (string path in filePaths.Split('|'))
                    {
                        SupportedFileExtension type = (SupportedFileExtension)Enum.Parse(typeof(SupportedFileExtension), fileService.GetExtensionFromFilePath(path).ToUpper()); // Eg. "MP3" or "FLAC"
                        lmp.AddFilePath(path, type);
                    }
                    GlobalContext.LocalMediaPackContainer.AddLMP(lmp, false); // Adding to current container
                }
            }
        }
    }
}
