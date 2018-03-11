using System;

namespace Onlab
{
    /*
    Class: Track
    State: Under construction | DEBUG
    Description:
        Represents a music track, that can be played and has various attributes.
        Also can link to a physical file, so that file operations can be done if possible. (wraps around JAudioTags.AudioFile class).
        Cannot participate in inheritance.
    */
    public sealed class Track
    {
        private bool isOfflineAccessible; //true if file is downloaded on local machine or false if it's a virtual track
        private TagLib.File fileHandle; //offline stored, physically accessible file handle, or null
        private AudioMetaData metaData; //ID3 tags of the track

        public Track(TagLib.File fileHandle, bool generateCustomizedTags = false)
        {
            this.isOfflineAccessible = true; //beacuse we have a non null file handle
            this.fileHandle = fileHandle;
            this.metaData = new AudioMetaData(fileHandle, generateCustomizedTags);
        }
        public Track(AudioMetaData metaData)
        {
            this.isOfflineAccessible = false; //beacuse we do not have a file handle
            this.fileHandle = null;
            this.metaData = metaData;
        }

        public bool IsOfflineAccessible
        {
            get { return this.isOfflineAccessible; }
        }
        public TagLib.File FileHandle //can return null
        {
            get { return this.fileHandle; }
        }
        public AudioMetaData MetaData
        {
            get { return this.metaData; }
        }
    }
}