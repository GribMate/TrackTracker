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
        ENG = 0, // Default
        HUN = 1,
        GER = 2,
        FRE = 4,
        SPA = 8
    }
}
