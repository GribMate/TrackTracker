using System;

namespace Onlab
{
    /*
    Enum: ExtensionType
    State: Under construction | DEBUG
    Description:
        Extension of music files for which proper tagging and meta-data is supported.
    */
    public enum ExtensionType : ushort
    {
        MP3 = 0, //default
        FLAC = 1
    }

    /*
    Enum: MediaPlayerType
    State: Under construction | DEBUG
    Description:
        Identifier of media players that are supported for tracklist mixing.
    */
    public enum MediaPlayerType : ushort
    {
        Foobar2000 = 0 //default
    }

    /*
    Enum: MusicGenre
    State: Under construction | DEBUG
    Description:
        Currently handled genre of tracks.
    */
    [Flags]
    public enum MusicGenre : ushort //TODO: may need to be deleted after MusicBrainz integration
    {
        Rock = 1,
        Pop = 2,
        Rap = 4,
        Metal = 8,
        Dubstep = 16,
        Classical = 32,
        Xtreme = 64
    }

    /*
    Enum: MusicLanguage
    State: Under construction | DEBUG
    Description:
        Currently handled language of tracks.
    */
    [Flags]
    public enum MusicLanguage : ushort //TODO: may need to be deleted after MusicBrainz integration
    {
        ENG = 1,
        HUN = 2,
        GER = 4,
        FRE = 8,
        SPA = 16
    }

    /*
    Class: GlobalVariables
    State: Under construction | DEBUG
    Description:
        Provides static and global properties and functions for the application.
        Used mainly for utility data during running.
    */
    public static class GlobalVariables
    {
        public static AppConfig Config; //persistent settings through the whole application
        public static TracklistData TracklistData; //dynamic wrapper of data currently represented @ Tracklist tab table
        public static PlayzoneData PlayzoneData; //dynamic wrapper of data currently represented @ Playzone tab table
        //TODO: generalize data classes via base class or interface

        public static void Initialize()
        {
            Config = new AppConfig(); //TODO: make persisent loading from config file stored locally on file system
            TracklistData = new TracklistData();
            PlayzoneData = new PlayzoneData();
        }
    }
}