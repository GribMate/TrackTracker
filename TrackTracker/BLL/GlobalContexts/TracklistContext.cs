using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using TrackTracker.BLL.Enums;
using TrackTracker.BLL.Model;
using TrackTracker.Services.Interfaces;



namespace TrackTracker.BLL.GlobalContexts
{
    /*
     * Provides a static single instance collection of app-wide data that cannot be tied to either one of the View tabs separately.
     * 
     * Tracklists.
     * 
     * TODO: rethink after MVVM
    */
    public static class TracklistContext
    {
        // Tracks that are on the local PC and displayed on Tracklist tab
        public static ObservableCollection<TrackLocal> TracklistTracks { get; set; } = new ObservableCollection<TrackLocal>();

        // Tracks that are on the local PC and can be mixed on Playzone tab
        public static ObservableCollection<TrackLocal> PlayzoneTracks { get; set; } = new ObservableCollection<TrackLocal>();



        public static void AddMusicFile(SupportedFileExtension ext, string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path), $"Cannot add new music file, since path is null.");
            if (path.Length < 8) // "C:\x.abc" is 8 characters long
                throw new ArgumentException("Cannot add new music file, path is too short.", nameof(path));

            try
            {
                ITaggingService taggingService = DependencyInjector.GetService<ITaggingService>();
                Dictionary<string, object> tagData = taggingService.Read(path, new List<string>() { ext.ToString() });
                MetaData trackData = FormatMetaData(tagData);

                // We need two different object copies to store
                TrackLocal tT = new TrackLocal(new MusicFileProperties(path), trackData);
                TrackLocal tP = new TrackLocal(new MusicFileProperties(path), trackData);
                TracklistTracks.Add(tT);
                PlayzoneTracks.Add(tP);
            }
            catch (Exception e) // TODO: more polished exception handling
            {
                ErrorHelper.ShowExceptionDialog(
                    "File reading error",
                    "File reading error happened while trying to parse a music file from local directory. This file will be omitted from Tracklist!",
                    $"File location: {path}");
            }
        }
        public static void RemoveMusicFile(string path)
        {
            TrackLocal itemToRemoveT = TracklistTracks.SingleOrDefault(t => t.MusicFileProperties.Path == path);
            if (itemToRemoveT != null)
            {
                TracklistTracks.Remove(itemToRemoveT);
            }
            TrackLocal itemToRemoveP = PlayzoneTracks.SingleOrDefault(t => t.MusicFileProperties.Path == path);
            if (itemToRemoveP != null)
            {
                PlayzoneTracks.Remove(itemToRemoveP);
            }
        }
        public static void ClearMusicFiles()
        {
            TracklistTracks.Clear();
            PlayzoneTracks.Clear();
        }



        private static MetaData FormatMetaData(Dictionary<string, object> source)
        {
            MetaData md = new MetaData();

            md.Genres.Value = (string[])source.Select(pair => pair).Where(pair => pair.Key.Equals("Genres")).First().Value;

            md.Title.Value = (string)source.Select(pair => pair).Where(pair => pair.Key.Equals("Title")).First().Value;
            md.Album.Value = (string)source.Select(pair => pair).Where(pair => pair.Key.Equals("Album")).First().Value;
            md.Copyright.Value = (string)source.Select(pair => pair).Where(pair => pair.Key.Equals("Copyright")).First().Value;
            md.AlbumArtists.Value = (string[])source.Select(pair => pair).Where(pair => pair.Key.Equals("AlbumArtists")).First().Value;
            md.AlbumArtistsSort.Value = (string[])source.Select(pair => pair).Where(pair => pair.Key.Equals("AlbumArtistsSort")).First().Value;
            md.BeatsPerMinute.Value = (int?)(uint)source.Select(pair => pair).Where(pair => pair.Key.Equals("BeatsPerMinute")).First().Value;
            md.Year.Value = (int?)(uint)source.Select(pair => pair).Where(pair => pair.Key.Equals("Year")).First().Value;
            md.Track.Value = (int?)(uint)source.Select(pair => pair).Where(pair => pair.Key.Equals("Track")).First().Value;
            md.TrackCount.Value = (int?)(uint)source.Select(pair => pair).Where(pair => pair.Key.Equals("TrackCount")).First().Value;
            md.Disc.Value = (int?)(uint)source.Select(pair => pair).Where(pair => pair.Key.Equals("Disc")).First().Value;
            md.DiscCount.Value = (int?)(uint)source.Select(pair => pair).Where(pair => pair.Key.Equals("DiscCount")).First().Value;

            md.AcoustID.Value = null; // TODO: Currently not obtained through tagging service, but can be added as a custom tag later

            // We have to null check each GUID
            string guid = (string)source.Select(pair => pair).Where(pair => pair.Key.Equals("MusicBrainzReleaseArtistId")).First().Value;
            if (String.IsNullOrWhiteSpace(guid))
                md.MusicBrainzReleaseArtistId.Value = null;
            else
                md.MusicBrainzReleaseArtistId.Value = new Guid(guid);

            guid = (string)source.Select(pair => pair).Where(pair => pair.Key.Equals("MusicBrainzTrackId")).First().Value;
            if (String.IsNullOrWhiteSpace(guid))
                md.MusicBrainzTrackId.Value = null;
            else
                md.MusicBrainzTrackId.Value = new Guid(guid);

            guid = (string)source.Select(pair => pair).Where(pair => pair.Key.Equals("MusicBrainzDiscId")).First().Value;
            if (String.IsNullOrWhiteSpace(guid))
                md.MusicBrainzDiscId.Value = null;
            else
                md.MusicBrainzDiscId.Value = new Guid(guid);

            guid = (string)source.Select(pair => pair).Where(pair => pair.Key.Equals("MusicBrainzReleaseId")).First().Value;
            if (String.IsNullOrWhiteSpace(guid))
                md.MusicBrainzReleaseId.Value = null;
            else
                md.MusicBrainzReleaseId.Value = new Guid(guid);

            guid = (string)source.Select(pair => pair).Where(pair => pair.Key.Equals("MusicBrainzArtistId")).First().Value;
            if (String.IsNullOrWhiteSpace(guid))
                md.MusicBrainzArtistId.Value = null;
            else
                md.MusicBrainzArtistId.Value = new Guid(guid);

            md.MusicBrainzReleaseStatus.Value = (string)source.Select(pair => pair).Where(pair => pair.Key.Equals("MusicBrainzReleaseStatus")).First().Value;
            md.MusicBrainzReleaseType.Value = (string)source.Select(pair => pair).Where(pair => pair.Key.Equals("MusicBrainzReleaseType")).First().Value;
            md.MusicBrainzReleaseCountry.Value = (string)source.Select(pair => pair).Where(pair => pair.Key.Equals("MusicBrainzReleaseCountry")).First().Value;

            return md;
        }
    }
}
