using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Onlab
{
    public class TracklistData
    {
        public ObservableCollection<Track> Tracks; 

        public TracklistData()
        {
            Tracks = new ObservableCollection<Track>();
        }

        public void AddMusicFile(ExtensionType type, string path)
        {
            if (path == null) throw new ArgumentNullException();
            if (path.Length < 3) throw new ArgumentException();

            try
            {
                TagLib.File audioFile = null;
                switch (type)
                {
                    case ExtensionType.MP3:
                        audioFile = new TagLib.Mpeg.AudioFile(path);
                        break;
                    case ExtensionType.FLAC:
                        audioFile = new TagLib.Flac.File(path);
                        break;
                    default:
                        audioFile = TagLib.File.Create(path);
                        break;
                }

                Track t = new Track(audioFile);
                Tracks.Add(t);
            }
            catch (Exception) //TODO: more polished exception handling
            {
                Dialogs.ExceptionNotification en = new Dialogs.ExceptionNotification("File reading error",
                    "File reading error happened while trying to parse a music file from local directory. This file will be omitted from Tracklist!",
                    $"File location: {path}");
            }
        }
        public void RemoveMusicFile(string path)
        {
            Track itemToRemove = Tracks.SingleOrDefault(t => t.FileHandle.Name == path);
            if (itemToRemove != null) Tracks.Remove(itemToRemove);
        }
    }
}
