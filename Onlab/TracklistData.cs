using System;
using System.Collections.Generic;

namespace Onlab
{
    public class TracklistData
    {
        public List<TagLib.File> MusicFiles; 

        public TracklistData(int capacity = 1000) //1000 for an approximate offline music collection
        {
            MusicFiles = new List<TagLib.File>(capacity);
        }

        public void AddMusicFile(string path)
        {
            if (path == null) throw new ArgumentNullException();
            if (path.Length < 3) throw new ArgumentException();

            try
            {
                TagLib.File tf = TagLib.File.Create(path);
                MusicFiles.Add(tf);
            }
            catch (Exception e)
            {
                //TODO: exception handling in-app for files that cannot be read by TagLib#
            }
        }
    }
}
