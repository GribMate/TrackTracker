using System;
using System.Collections.Generic;

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
    Enum: MusicGenre
    State: Under construction | DEBUG
    Description:
        Currently handled genre of tracks.
        TODO: may need to be deleted after MusicBrainz integration
    */
    [Flags]
    public enum MusicGenre : ushort
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
        TODO: may need to be deleted after MusicBrainz integration
    */
    [Flags]
    public enum MusicLanguage : ushort
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
        public sealed class ConfigClass
        {
            public string LocalMediaPath;
            public string FoobarPath;

            public ConfigClass()
            {
                LocalMediaPath = null;
                FoobarPath = null;
            }
        }
        public sealed class OfflineDataClass
        {

        }
        public sealed class GUIDataClass
        {

        }

        public static ConfigClass Config = new ConfigClass();
        public static List<TagLib.File> MusicFiles = new List<TagLib.File>(1000); //1000 for an approximate offline music collection

        public static void AddMusicFile(string path)
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