using System;

using Onlab.Services;
using Onlab.Services.Interfaces;
//TODO
using TrackTracker.Services;
using TrackTracker.Services.Interfaces;



namespace Onlab.BLL
{
    /*
    Class: GlobalVariables
    Description:
        Provides static and global properties and variables for the application.
        Used mainly for utility data during running.
        Provides data for GlobalAlgorithms class.
    */
    public static class GlobalVariables
    {
        public static IDatabaseService DatabaseService = new SQLiteService();
        public static IFileService FileService = new FileService();
        public static IEnvironmentService EnvironmentService = new WindowsEnvironmentService();
        public static IMetadataService MetadataService = new MusicBrainzService();
        public static IFingerprintService FingerprintService = new AcoustIDService();
        public static ISpotifyService SpotifyService = new SpotifyService();

        public static AppConfig AppConfig = new AppConfig();
        public static LocalMediaPackContainer LocalMediaPackContainer = new LocalMediaPackContainer(); //persistent settings through the whole application
        public static TracklistData TracklistData = new TracklistData(); //dynamic wrapper of data currently represented @ Tracklist tab table
        public static PlayzoneData PlayzoneData = new PlayzoneData(); //dynamic wrapper of data currently represented @ Playzone tab table
        //TODO: MVVM and DI
    }
}