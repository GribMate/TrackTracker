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
     * Offline tracklist.
     * 
     * TODO: rethink after MVVM
    */
    public static class TracklistContext
    {
        // Tracks that are on the local PC and displayed on Tracklist tab
        public static ObservableCollection<TrackLocal> TracklistTracks { get; set; } = new ObservableCollection<TrackLocal>();



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
                MetaData trackData = UtilityHelper.FormatMetaData(tagData);

                TrackLocal track = new TrackLocal(new MusicFileProperties(path), trackData);
                TracklistTracks.Add(track);
            }
            catch (Exception) // TODO: more polished exception handling
            {
                UtilityHelper.ShowExceptionDialog(
                    "File reading error",
                    "File reading error happened while trying to parse a music file from local directory. This file will be omitted from Tracklist!",
                    $"File location: {path}");
            }
        }
        public static void RemoveMusicFile(string path)
        {
            TrackLocal itemToRemove = TracklistTracks.SingleOrDefault(t => t.MusicFileProperties.Path == path);
            if (itemToRemove != null)
            {
                TracklistTracks.Remove(itemToRemove);
            }
        }
        public static void ClearMusicFiles()
        {
            TracklistTracks.Clear();
        }
    }
}
