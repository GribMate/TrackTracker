using System;



namespace TrackTracker.BLL.Enums
{
    /*
     * Moods that correspond to a track according to a given user.
     * It is usual for a track to have a mix of these flags and wildly differ from user to user.
    */
    [Flags]
    public enum MusicMoods
    {
        Party = 1 << 0,
        Chill = 1 << 1,
        Studying_or_Working = 1 << 2,
        WorkingOut = 1 << 3,
        Hype = 1 << 4,
        MorningStarter = 1 << 5,
        DreamBringer = 1 << 6,
        BinauralBeats = 1 << 7,
        ASMR = 1 << 8,
        Romantic = 1 << 9,
        Heartbroken = 1 << 10,
        Depressing = 1 << 11,
        Uplifting = 1 << 12,
        Motivating = 1 << 13,
        Thinker = 1 << 14,
        Travelling = 1 << 15,
        Gangster = 1 << 16,

        Unknown = 0 // Default
    }
}
