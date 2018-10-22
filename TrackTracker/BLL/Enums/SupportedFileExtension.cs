using System;



namespace TrackTracker.BLL.Enums
{
    /*
     * Currently supported extensions of music files by TrackTracker.
     * Note: WAV or APE support might be the next reasonable update.
    */
    public enum SupportedFileExtension
    {
        MP3 = 1,
        FLAC = 2,

        All = 0, // Means that all of the supported formats should be handled

        Unknown = -1
    }
}
