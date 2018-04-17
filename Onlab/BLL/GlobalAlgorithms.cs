using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using Onlab.DAL;



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

        private static void FirstRunSetup()
        {
            GlobalVariables.DatabaseProvider.CreateDatabase();
            GlobalVariables.DatabaseProvider.FormatNewDatabase();
        }
        private static void LoadPersistence()
        {
            List<string[]> lmpRows = GlobalVariables.DatabaseProvider.GetRowsByWhere("LocalMediaPacks", "1=1"); //HACK: 1=1 where expression ensures we get all records

            if (lmpRows.Count > 0)
            {
                foreach (string[] row in lmpRows)
                {
                    string rootPath = row[0];
                    ExtensionType baseExtension = (row[1].Length > 0) ? (ExtensionType)Enum.Parse(typeof(ExtensionType), row[1]) : ExtensionType.MP3;
                    bool isOnRemovableDrive = Int32.Parse(row[2]) == 1;
                    bool isResultOfDriveSearch = Int32.Parse(row[3]) == 1;
                    string filePaths = row[4]; //array of strings, divided by "|"

                    LocalMediaPack lmp = new LocalMediaPack(rootPath, isOnRemovableDrive, isResultOfDriveSearch, baseExtension);
                    foreach (string path in filePaths.Split('|'))
                    {
                        ExtensionType type = (ExtensionType)Enum.Parse(typeof(ExtensionType), GlobalVariables.FileProvider.GetExtensionFromFilePath(path).ToUpper());
                        lmp.AddFilePath(path, type);
                    }
                    GlobalVariables.LocalMediaPackContainer.AddLocalMediaPack(lmp, false);
                }
            }
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
        public static bool GetInternetState()
        {
            return GlobalVariables.EnvironmentProvider.InternetConnectionIsAlive();
        }



        public async static Task<List<test_MatchTableRow>> MBTEST(Track track)
        {
            MetaBrainz.MusicBrainz.Query q = new MetaBrainz.MusicBrainz.Query("TrackTracker");

            if (track.MetaData.Title != null)
            {
                try
                {
                    //var releases = GetReleases(track.MetaData.Title, null);
                    var recordings = await q.FindRecordingsAsync(track.MetaData.Title);
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
                    return rows;
                }
                catch (Exception exc)
                {
                    Dialogs.ExceptionNotification en = new Dialogs.ExceptionNotification("Error while searching MusicBrainz",
                        exc.Message, "File: " + track.MetaData.Title);
                    en.ShowDialog();
                    //TODO: not here
                }
            }
            return new List<test_MatchTableRow>();
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
