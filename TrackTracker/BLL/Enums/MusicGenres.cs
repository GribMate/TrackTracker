using System;

namespace TrackTracker.BLL.Enums
{
    /*
     * Genres that describe a track. It is usual for a track to have a mix of these flags.
    */
    [Flags]
    public enum MusicGenres
    {
        // Basic, more general genres
        Blues = 1 << 0,
        Classical = 1 << 1,
        Jazz = 1 << 2,
        Reggae = 1 << 3,
        Country = 1 << 4,
        Folk = 1 << 5,
        R_and_B = 1 << 6,
        Electronic = 1 << 7,
        Rock = 1 << 8,
        Metal = 1 << 9,
        Hip_hop = 1 << 10,
        Pop = 1 << 11,

        // Specific genres, usually coming in pair with a general one from above
        Rap = 1 << 12,  
        D_and_B = 1 << 13,
        Dubstep = 1 << 14,
        House = 1 << 15,
        Techno = 1 << 16,
        Trap = 1 << 17,
        Alternative = 1 << 18,
        Dance = 1 << 19,
        KPop = 1 << 20,
        JPop = 1 << 21,
        ElectroSwing = 1 << 22,
        Synthwave = 1 << 23,
        Nightcore = 1 << 24,

        OST = 1 << 25, // Marking Original SoundTrack (OST) files
        Special = 1 << 26, // Used to mark specific musics that are, by some measures, odd

        Unknown = 0 // Default
    }
}
