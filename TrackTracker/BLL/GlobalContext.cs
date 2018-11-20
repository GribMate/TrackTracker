using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using TrackTracker.BLL.Enums;



namespace TrackTracker.BLL
{
    /*
     * Provides a static single instance collection of app-wide data that cannot be tied to either one of the View tabs separately.
     * 
     * TODO: rethink after MVVM
    */
    public static class GlobalContext
    {
        // Persistent settings through the whole application
        public static AppConfig AppConfig { get; set; } = new AppConfig();

        // LMP container
        public static LocalMediaPackContainer LocalMediaPackContainer { get; set; } = new LocalMediaPackContainer();





        public static ObservableCollection<Track> TracklistTracks { get; set; } = new ObservableCollection<Track>();
        
        public static ObservableCollection<Track> PlayzoneTracks { get; set; } = new ObservableCollection<Track>();



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
                Track tT = new Track(audioFile);
                Track tP = new Track(audioFile);
                TracklistTracks.Add(tT);
                PlayzoneTracks.Add(tP);
            }
            catch (Exception) //TODO: more polished exception handling
            {
                Dialogs.ExceptionNotification en = new Dialogs.ExceptionNotification("File reading error",
                    "File reading error happened while trying to parse a music file from local directory. This file will be omitted from Tracklist!",
                    $"File location: {path}");
            }
        }
        public static void RemoveMusicFile(string path)
        {
            Track itemToRemoveT = TracklistTracks.SingleOrDefault(t => t.FileHandle.Name == path);
            if (itemToRemoveT != null)
            {
                TracklistTracks.Remove(itemToRemoveT);
            }
            Track itemToRemoveP = PlayzoneTracks.SingleOrDefault(t => t.FileHandle.Name == path);
            if (itemToRemoveP != null)
            {
                PlayzoneTracks.Remove(itemToRemoveP);
            }
        }
        public static void Clear()
        {
            TracklistTracks.Clear();
            PlayzoneTracks.Clear();
        }
    }
}
