using System;
using System.ComponentModel;

namespace Onlab
{
    /*
    Class: Track
    State: Under construction | DEBUG
    Description:
        Represents a music track, that has various attributes.
        Also can link to a physical file, so that file operations can be done if possible. (wraps around TagLib.File class).
    */
    public class Track : INotifyPropertyChanged
    {
        private bool isSelectedInGUI;
        private bool isOfflineAccessible;
        private TagLib.File fileHandle;
        private AudioMetaData metaData;

        public bool IsSelectedInGUI //true if the file is selected via Tracklist GUI
        {
            get => isSelectedInGUI;
            set
            {
                if (isSelectedInGUI != value)
                {
                    isSelectedInGUI = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelectedInGUI)));
                }
            }
        }
        public bool IsOfflineAccessible //true if file is downloaded on local machine or false if it's a virtual track
        {
            get => isOfflineAccessible;
            set
            {
                if (isOfflineAccessible != value)
                {
                    isOfflineAccessible = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsOfflineAccessible)));
                }
            }
        }
        public TagLib.File FileHandle //offline stored, physically accessible file handle, or null
        {
            get => fileHandle; //don't support on-the-fly changes
        }
        public AudioMetaData MetaData //ID3 tags of the track
        {
            get => metaData; //don't support on-the-fly changes
        }

        public event PropertyChangedEventHandler PropertyChanged; //Tracks are 1-1 represented in GUI

        public Track(TagLib.File fileHandle)
        {
            IsSelectedInGUI = false;
            IsOfflineAccessible = true; //beacuse we have a non null file handle
            this.fileHandle = fileHandle;
            this.metaData = new AudioMetaData(fileHandle);
        }
        public Track(AudioMetaData metaData)
        {
            IsSelectedInGUI = false;
            IsOfflineAccessible = false; //beacuse we do not have a file handle
            this.fileHandle = null;
            this.metaData = metaData;
        }
    }
}