using System;
using System.Text;



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
        Electroswing = 1 << 22,
        Synthwave = 1 << 23,
        Nightcore = 1 << 24,

        OST = 1 << 25, // Marking Original SoundTrack (OST) files
        Special = 1 << 26, // Used to mark specific musics that are, by some measures, odd

        Unknown = 0 // Default
    }

    public static class MusicGenresConverter // Provides static methods to manipulate MusicGenres
    {
        public static string GetFormattedString(MusicGenres genre) // Returns a user-friendly string representation of a MusicGenres enum value
        {
            string[] genreFlags = genre.ToString().Split(',');
            StringBuilder formatted = new StringBuilder();

            foreach (string genreFlag in genreFlags)
            {
                string genreFlagTrimmed = genreFlag.Trim();

                // Handling two special cases where a "-" should be inserted
                if (MusicGenres.JPop.ToString().Equals(genreFlagTrimmed))
                {
                    formatted.Append("J-Pop, ");
                    continue;
                }
                else if (MusicGenres.KPop.ToString().Equals(genreFlagTrimmed))
                {
                    formatted.Append("K-Pop, ");
                    continue;
                }

                // Handling generic cases
                string step1 = genreFlagTrimmed.Replace("_and_", "&"); // D_and_B like values
                string step2 = step1.Replace('_', '-'); // If there wasn't "_and_" present, but there is "_", like in "Hip_hop"

                formatted.Append(step2 + ", ");
            }

            formatted.Remove(formatted.Length - 2, 2); // Removing unnecessary last ", " chars

            return formatted.ToString();
        }
    }
}
