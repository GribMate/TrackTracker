using System;



namespace TrackTracker.BLL.Enums
{
    /*
     * Supported lyrics languages of a track.
     * These might be mixed in international songs or collaborations.
    */
    [Flags]
    public enum LyricsLanguages
    {
        //TODO: support most languages
        ENG = 1 << 0,
        HUN = 1 << 1,
        GER = 1 << 2,
        FRA = 1 << 3,
        SPA = 1 << 4,

        Unknown = 0 // Default
    }
}
