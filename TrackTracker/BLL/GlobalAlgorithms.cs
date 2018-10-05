﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using TrackTracker.BLL.Enums;
using TrackTracker.Services;
using TrackTracker.Services.Interfaces;



namespace TrackTracker.BLL
{
    /*
    Class: GlobalAlgorithms
    Description:
        Provides static and global functions for the application.
        Main point of the BLL layer.
        Uses GlobalAlgorithms data.
    */
    public static class GlobalAlgorithms
    {
        public static IDatabaseService DatabaseService = new SQLiteService();
        public static IFileService FileService = new FileService();
        public static IEnvironmentService EnvironmentService = new WindowsEnvironmentService();
        public static IMetadataService MetadataService = new MusicBrainzService();
        public static IFingerprintService FingerprintService = new AcoustIDService();
        public static ISpotifyService SpotifyService = new SpotifyService();

        public static AppConfig AppConfig = new AppConfig();
        public static LocalMediaPackContainer LocalMediaPackContainer = new LocalMediaPackContainer(); //persistent settings through the whole application
        public static TracklistData TracklistData = new TracklistData(); //dynamic wrapper of data currently represented @ Tracklist tab table
        public static PlayzoneData PlayzoneData = new PlayzoneData(); //dynamic wrapper of data currently represented @ Playzone tab table
        //TODO: MVVM and DI

        
        
        public static void Initialize() //first function to be called after OS gave control (and before GUI loads)
        {
            if (!DatabaseService.DatabaseExists) FirstRunSetup(); //we don't have a database file, that means it's the first time the app runs
            else LoadPersistence(); //app has run before, we need to load persistence from DB
        }
        private static void FirstRunSetup() //creates a new empty database and forms it's data structure
        {
            DatabaseService.CreateDatabase(); //creating empty
            DatabaseService.FormatNewDatabase(); //formatting existing
        }
        private static void LoadPersistence() //loads all data from an already existing database file
        {
            List<string[]> lmpRows = DatabaseService.GetAllRows("LocalMediaPacks"); //getting LocalMediaPack objects

            // ============================== casting and loading LocalMediaPack objects ==============================

            if (lmpRows.Count > 0)
            {
                foreach (string[] row in lmpRows)
                {
                    string rootPath = row[0];
                    SupportedFileExtension baseExtension = (row[1].Length > 0) ? (SupportedFileExtension)Enum.Parse(typeof(SupportedFileExtension), row[1]) : SupportedFileExtension.MP3; //default type is MP3 if cell is null
                    bool isResultOfDriveSearch = Int32.Parse(row[2]) == 1;
                    string filePaths = row[3]; //array of strings, divided by "|"

                    LocalMediaPack lmp = new LocalMediaPack(rootPath, isResultOfDriveSearch, baseExtension);
                    foreach (string path in filePaths.Split('|'))
                    {
                        SupportedFileExtension type = (SupportedFileExtension)Enum.Parse(typeof(SupportedFileExtension), FileService.GetExtensionFromFilePath(path).ToUpper()); //eg. "MP3" or "FLAC"
                        lmp.AddFilePath(path, type);
                    }
                    LocalMediaPackContainer.AddLMP(lmp, false); //adding to current container
                }
            }
        }

        public static void LoadFilesFromDrive(IFileService service, LocalMediaPack lmp, string driveLetter, SupportedFileExtension type) //loads all the files with the given extension from a given drive into an LMP object, using a service
        {
            List<string> paths = service.GetAllFilesFromDrive(driveLetter, type.ToString()); //no typed extensions when calling to DAL
            foreach (string path in paths)
            {
                lmp.AddFilePath(path, type); //loading up the LMP object
            }
        }
        public static void LoadFilesFromDirectory(IFileService service, LocalMediaPack lmp, string path) //loads all the files with the given extension from a given directory into an LMP object, using a service
        {
            //when loading from a directory, we want all the supported file types to be read, so we iterate through the extensions
            foreach (SupportedFileExtension ext in Enum.GetValues(typeof(SupportedFileExtension)).Cast<SupportedFileExtension>()) //casting to get typed iteration, just in case
            {
                List<string> paths = service.GetAllFilesFromDirectory(path, ext.ToString()); //no typed extensions when calling to DAL
                foreach (string currPath in paths)
                {
                    lmp.AddFilePath(currPath, ext); //loading up the LMP object
                }
            }
        }
        public static bool GetInternetState() //returns true if the application has live internet connection
        {
            return EnvironmentService.InternetConnectionIsAlive();
        }



        public async static Task<List<Track>> GetMatchesForTrack(Track track)
        {
            List<Track> matches = new List<Track>();

            if (!String.IsNullOrEmpty(track.MetaData.MusicBrainzTrackId)) //track has an MBID, we need a lookup
            {
                Track result = await GetMatchByMBID(track.MetaData.MusicBrainzTrackId);
                matches.Add(result);
            }
            else if (!String.IsNullOrEmpty(track.MetaData.Title)) //we don't have MBID, we need a search by some metadata
            {
                matches = await GetMatchesByMetaData(track.MetaData.Title,
                    track.MetaData.JoinedAlbumArtists.Split(';').First(), //can be null
                    track.MetaData.Album); //can be null
            }
            else if (!String.IsNullOrEmpty(track.FileHandle.Name)) //we don't have MBID, nor a title, but we can try some magic from the file name
            {
                string trackName = FileService.GetFileNameFromFilePath(track.FileHandle.Name);
                if (trackName.Contains("-"))
                {
                    string[] splitted = trackName.Split('-');
                    if (splitted.Length == 2)
                    {
                        if (splitted[0].Length > 0 && splitted[1].Length > 0)
                        {
                            string supposedArtist = splitted[0].Trim();
                            string supposedTitle = splitted[1].Trim();
                            matches = await GetMatchesByMetaData(supposedTitle, supposedArtist);
                        }
                    }
                }
            }
            else
            {
                Dialogs.ExceptionNotification en = new Dialogs.ExceptionNotification(
                    "Track matching error",
                    "Cannot query this track against MusicBrainz, since it has no relevant metadata.",
                    "Try to use fingerprinting!");
                en.ShowDialog();
            }

            return matches;
        }
        private async static Task<Track> GetMatchByMBID(string MBID)
        {
            AudioMetaData result = await MetadataService.GetRecordingByMBID(MBID);

            return new Track(result);
        }
        private async static Task<List<Track>> GetMatchesByMetaData(string title, string artist = null, string album = null)
        {
            List<AudioMetaData> results = await MetadataService.GetRecordingsByMetaData(title, artist, album);

            List<Track> toReturn = new List<Track>();

            foreach (AudioMetaData result in results)
            {
                toReturn.Add(new Track(result));
            }

            return toReturn;
        }
    }
}
