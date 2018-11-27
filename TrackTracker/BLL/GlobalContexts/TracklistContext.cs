using System;
using System.Collections.ObjectModel;
using System.Linq;

using TrackTracker.BLL.Enums;
using TrackTracker.BLL.Model;



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



        public static void AddMusicFile(SupportedFileExtension type, string path)
        {
            if (path == null) throw new ArgumentNullException();
            if (path.Length < 3) throw new ArgumentException();

            try
            {
                TagLib.File audioFile = null;
                switch (type)
                {
                    case SupportedFileExtension.MP3:
                        audioFile = new TagLib.Mpeg.AudioFile(path);
                        break;
                    case SupportedFileExtension.FLAC:
                        audioFile = new TagLib.Flac.File(path);
                        break;
                    default:
                        audioFile = TagLib.File.Create(path);
                        break;
                }

                // We need two different object copies to store
                TrackLocal tT = new TrackLocal(new MusicFileProperties(path));
                TrackLocal tP = new TrackLocal(new MusicFileProperties(path));
                TracklistTracks.Add(tT);
                PlayzoneTracks.Add(tP);
            }
            catch (Exception) //TODO: more polished exception handling
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
    }
}
