using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using Onlab.DAL.Interfaces;



namespace Onlab.BLL
{
    /*
    Class: GlobalAlgorithms
    Description:
        Provides static and global functions for the application.
        Main point of the BLL layer.
        Uses GlobalVariables data.
    */
    public static class GlobalAlgorithms
    {
        public static void Initialize() //first function to be called after OS gave control (and before GUI loads)
        {
            GlobalVariables.Initialize(); //prepares data structures

            if (!GlobalVariables.DatabaseProvider.DatabaseExists) FirstRunSetup(); //we don't have a database file, that means it's the first time the app runs
            else LoadPersistence(); //app has run before, we need to load persistence from DB
        }
        private static void FirstRunSetup() //creates a new empty database and forms it's data structure
        {
            GlobalVariables.DatabaseProvider.CreateDatabase(); //creating empty
            GlobalVariables.DatabaseProvider.FormatNewDatabase(); //formatting existing
        }
        private static void LoadPersistence() //loads all data from an already existing database file
        {
            List<string[]> lmpRows = GlobalVariables.DatabaseProvider.GetAllRows("LocalMediaPacks"); //getting LocalMediaPack objects

            // ============================== casting and loading LocalMediaPack objects ==============================

            if (lmpRows.Count > 0)
            {
                foreach (string[] row in lmpRows)
                {
                    string rootPath = row[0];
                    ExtensionType baseExtension = (row[1].Length > 0) ? (ExtensionType)Enum.Parse(typeof(ExtensionType), row[1]) : ExtensionType.MP3; //default type is MP3 if cell is null
                    bool isResultOfDriveSearch = Int32.Parse(row[2]) == 1;
                    string filePaths = row[3]; //array of strings, divided by "|"

                    LocalMediaPack lmp = new LocalMediaPack(rootPath, isResultOfDriveSearch, baseExtension);
                    foreach (string path in filePaths.Split('|'))
                    {
                        ExtensionType type = (ExtensionType)Enum.Parse(typeof(ExtensionType), GlobalVariables.FileProvider.GetExtensionFromFilePath(path).ToUpper()); //eg. "MP3" or "FLAC"
                        lmp.AddFilePath(path, type);
                    }
                    GlobalVariables.LocalMediaPackContainer.AddLMP(lmp, false); //adding to current container
                }
            }
        }

        public static void LoadFilesFromDrive(IFileProvider provider, LocalMediaPack lmp, string driveLetter, ExtensionType type) //loads all the files with the given extension from a given drive into an LMP object, using a provider
        {
            List<string> paths = provider.GetAllFilesFromDrive(driveLetter, type.ToString()); //no typed extensions when calling to DAL
            foreach (string path in paths)
            {
                lmp.AddFilePath(path, type); //loading up the LMP object
            }
        }
        public static void LoadFilesFromDirectory(IFileProvider provider, LocalMediaPack lmp, string path) //loads all the files with the given extension from a given directory into an LMP object, using a provider
        {
            //when loading from a directory, we want all the supported file types to be read, so we iterate through the extensions
            foreach (ExtensionType ext in Enum.GetValues(typeof(ExtensionType)).Cast<ExtensionType>()) //casting to get typed iteration, just in case
            {
                List<string> paths = provider.GetAllFilesFromDirectory(path, ext.ToString()); //no typed extensions when calling to DAL
                foreach (string currPath in paths)
                {
                    lmp.AddFilePath(currPath, ext); //loading up the LMP object
                }
            }
        }
        public static bool GetInternetState() //returns true if the application has live internet connection
        {
            return GlobalVariables.EnvironmentProvider.InternetConnectionIsAlive();
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
                string trackName = GlobalVariables.FileProvider.GetFileNameFromFilePath(track.FileHandle.Name);
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
            Dictionary<string, string> result = await GlobalVariables.MusicBrainzProvider.GetRecordingByMBID(MBID);

            ;

            throw new Exception();
            //TODO breakpoint debug
        }
        private async static Task<List<Track>> GetMatchesByMetaData(string title, string artist = null, string album = null)
        {
            List<Dictionary<string, string>> results = await GlobalVariables.MusicBrainzProvider.GetRecordingsByMetaData(title, 10, artist, album);
            //TODO: do not hardcode limit here

            foreach (Dictionary<string, string> result in results)
            {

            }

            throw new Exception();
            //TODO: breakpoint debug
            /*
            try
            {
                foreach (var item in recordings.Results)
                {
                    string title = item.Title;
                    string artist = null;
                    if (item.ArtistCredit.Count > 0) artist = item.ArtistCredit[0].Artist.Name;
                    else artist = "Unknown";
                    string mbid = item.MbId.ToString();
                    test_MatchTableRow row = new test_MatchTableRow(artist, title, mbid);
                    rows.Add(row);
                }
                return rows;
            }
            catch (Exception exc)
            {
                Dialogs.ExceptionNotification en = new Dialogs.ExceptionNotification("Error while searching MusicBrainz",
                    exc.Message, "File: " + track.MetaData.Title);
                en.ShowDialog();
                //TODO: not here
            }
            */
        }

        /*
        private static MetaBrainz.MusicBrainz.Interfaces.Searches.ISearchResults<MetaBrainz.MusicBrainz.Interfaces.Searches.IFoundRelease> GetReleases(string title, int? tries)
        {
            MetaBrainz.MusicBrainz.Query q = new MetaBrainz.MusicBrainz.Query("TrackTracker");

            try
            {
                return q.FindRecordingsAsync(title, tries);
            }
            catch (Exception)
            {
                if (tries == null) tries = 10;
                else if (tries > 1) tries--;
                else return null;

                return GetReleases(title, tries);
            }
        }
        */

        /*
        else
        {
        var recordings = q.FindRecordings(selectedTrack.MetaData.Title);
        List<test_MatchTableRow> rows = new List<test_MatchTableRow>();
        foreach (var item in recordings.Results)
        {
        string title = item.Title;
        string artist = null;
        if (item.ArtistCredit.Count > 0) artist = item.ArtistCredit[0].Artist.Name;
        else artist = "Unknown";
        string mbid = item.MbId.ToString();
        test_MatchTableRow row = new test_MatchTableRow(artist, title, mbid);
        rows.Add(row);
        }
        tracklist_dataGridMatchList.ItemsSource = rows;
        }
        */
    }
}
