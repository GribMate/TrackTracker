using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using TrackTracker.BLL.Enums;
using TrackTracker.BLL.Model;
using TrackTracker.Services.Interfaces;



namespace TrackTracker.BLL.GlobalContexts
{
    /*
     * Provides a static single instance collection of app-wide data that cannot be tied to either one of the View tabs separately.
     * 
     * Playzone tracklists.
     * 
     * TODO: rethink after MVVM
    */
    public static class PlayzoneContext
    {
        // Tracks that are on the local PC and can be mixed
        public static ObservableCollection<TrackLocal> LocalTracks { get; set; } = new ObservableCollection<TrackLocal>();

        // Tracks that are on Spotify and can be mixed
        public static ObservableCollection<TrackVirtual> SpotifyTracks { get; set; } = new ObservableCollection<TrackVirtual>();



        public static void AddMusicFileLocal(SupportedFileExtension ext, string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path), $"Cannot add new music file, since path is null.");
            if (path.Length < 8) // "C:\x.abc" is 8 characters long
                throw new ArgumentException("Cannot add new music file, path is too short.", nameof(path));

            try
            {
                ITaggingService taggingService = DependencyInjector.GetService<ITaggingService>();
                Dictionary<string, object> tagData = taggingService.Read(path, new List<string>() { ext.ToString() });
                MetaData trackData = UtilityHelper.FormatMetaData(tagData);

                TrackLocal track = new TrackLocal(new MusicFileProperties(path), trackData);
                LocalTracks.Add(track);
            }
            catch (Exception) // TODO: more polished exception handling
            {
                UtilityHelper.ShowExceptionDialog(
                    "File reading error",
                    "File reading error happened while trying to parse a music file from local directory. This file will be omitted from Tracklist!",
                    $"File location: {path}");
            }
        }
        public async static void AddMusicFileSpotify(string trackID)
        {
            if (String.IsNullOrWhiteSpace(trackID))
                throw new ArgumentException($"Cannot add new music file, since its ID is null or empty.", nameof(trackID));

            try
            {
                ISpotifyService spotifyService = DependencyInjector.GetService<ISpotifyService>();
                Dictionary<string, string> tagData = await spotifyService.GetTrackData(trackID);
                MetaData trackData = UtilityHelper.FormatMetaDataSpotify(tagData, out string trackURI);

                TrackVirtual track = new TrackVirtual(trackData, false);
                track.SpotifyID = trackID;
                track.SpotifyURI = trackURI;
                SpotifyTracks.Add(track);
            }
            catch (Exception) // TODO: more polished exception handling
            {
                UtilityHelper.ShowExceptionDialog(
                    "Track parsing error",
                    "Track parsing error happened while trying to parse a music file from Spotify. This file will be omitted from Playzone!",
                    $"Track URI: {trackID}");
            }
        }
        public static void RemoveMusicFileLocal(string path)
        {
            TrackLocal itemToRemove = LocalTracks.SingleOrDefault(t => t.MusicFileProperties.Path == path);
            if (itemToRemove != null)
            {
                LocalTracks.Remove(itemToRemove);
            }
        }
        public static void RemoveMusicFileSpotify(string trackID)
        {
            TrackVirtual itemToRemove = SpotifyTracks.SingleOrDefault(t => t.SpotifyID == trackID);
            if (itemToRemove != null)
            {
                SpotifyTracks.Remove(itemToRemove);
            }
        }
        public static void ClearMusicFilesAll()
        {
            LocalTracks.Clear();
            SpotifyTracks.Clear();
        }
    }
}
