using System;



namespace TrackTracker.BLL
{
    /*
     * TODO: remove for MVVM
    */
    public static class GlobalData
    {
        public static AppConfig AppConfig { get; set; } = new AppConfig();

        public static LocalMediaPackContainer LocalMediaPackContainer { get; set; } = new LocalMediaPackContainer(); //persistent settings through the whole application

        public static TracklistData TracklistData { get; set; } = new TracklistData(); //dynamic wrapper of data currently represented @ Tracklist tab table

        public static PlayzoneData PlayzoneData { get; set; } = new PlayzoneData(); //dynamic wrapper of data currently represented @ Playzone tab table
    }
}
