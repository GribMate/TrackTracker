using System;

namespace Onlab
{
    /*
    Class: Track
    State: Under construction | DEBUG
    Description:
        Represents a music track, that can be played and has various attributes.
        Also can link to a physical file, so that file operations can be done if possible. (wraps around TagLib.File class).
        Cannot participate in inheritance.
    */
    public sealed class Track
    {
        public bool IsSelectedInGUI { get; set; } //true if the file is selected via Tracklist GUI
        public bool IsOfflineAccessible { get; set; } //true if file is downloaded on local machine or false if it's a virtual track
        public TagLib.File FileHandle { get; set; } //offline stored, physically accessible file handle, or null
        public AudioMetaData MetaData { get; set; } //ID3 tags of the track

        public Track(TagLib.File fileHandle, bool generateCustomizedTags = false)
        {
            IsSelectedInGUI = false;
            IsOfflineAccessible = true; //beacuse we have a non null file handle
            FileHandle = fileHandle;
            MetaData = new AudioMetaData(fileHandle, generateCustomizedTags);
        }
        public Track(AudioMetaData metaData)
        {
            IsSelectedInGUI = false;
            IsOfflineAccessible = false; //beacuse we do not have a file handle
            FileHandle = null;
            MetaData = metaData;
        }
    }
}