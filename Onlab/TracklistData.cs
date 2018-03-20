using System;
using System.Collections.Generic;

namespace Onlab
{
    public class TracklistData
    {
        public List<Track> Tracks; 

        public TracklistData(int capacity = 1000) //1000 for an approximate offline music collection
        {
            Tracks = new List<Track>(capacity);
        }

        public void AddMusicFile(string path)
        {
            if (path == null) throw new ArgumentNullException();
            if (path.Length < 3) throw new ArgumentException();

            try
            {
                Track t = new Track(TagLib.File.Create(path));
                Tracks.Add(t);
            }
            catch (Exception e) //TODO: more polished exception handling
            {
                Dialogs.ExceptionNotification en = new Dialogs.ExceptionNotification("File reading error",
                    "File reading error happened while trying to parse a music file from local directory. This file will be omitted from Tracklist!",
                    $"File location: {path}");
            }
        }
    }
}
