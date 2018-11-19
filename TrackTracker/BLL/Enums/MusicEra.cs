using System;
using System.Linq;



namespace TrackTracker.BLL.Enums
{
    /*
     * Disjoint sets of spans of several years (decades or half decades mostly).
     * Used for statistics / profiling.
    */
    public enum MusicEra
    {
        Years_1940_1949 = 40,
        Years_1950_1959 = 50,
        Years_1960_1969 = 60,
        Years_1970_1979 = 70,
        Years_1980_1989 = 80,
        Years_1990_1999 = 90,

        Years_2000_2004 = 05,
        Years_2005_2009 = 10,
        Years_2010_2014 = 15,
        Years_2015_2019 = 20,
        Years_2020_2024 = 25,
        Years_2025_2029 = 30,
        Years_2030_2034 = 35,

        Unknown = -1
    }

    public static class MusicErasConverter // Provides static methods to manipulate MusicEras
    {
        public static string GetFormattedString(MusicEra era) // Returns a user-friendly string representation of a MusicEras enum value
        {
            if (era == MusicEra.Unknown) return "Unknown";

            // Gets the two boundary years of the era and replaces "_" with "-", for example: "2000-2004"
            return era.ToString().Remove(0, "Years_".Length).Replace('_', '-');
        }
        public static bool IsNewMillenium(MusicEra era) // Return true if the era is after 2000, and false if it's earlier
        {
            if (era == MusicEra.Unknown)
                throw new ArgumentOutOfRangeException(nameof(era), "Millenia cannot be determined about unknown era.");

            int eraValue = (int)era;
            if (eraValue < 40) return true;  // 1940 is the earliest supported date from 1900s
            else return false;
        }
        public static bool IsDateInEra(int year, MusicEra era) // Determines if a given year is in a given era
        {
            if (year < 1940 || year > 2034)
                throw new ArgumentOutOfRangeException(nameof(year), $"The date {year} is not in the supported range of eras.");
            if (era == MusicEra.Unknown)
                throw new ArgumentException("Cannot determine inclusion for unknown era.", nameof(era));

            string formattedEra = GetFormattedString(era);

            int minYear = Convert.ToInt32(formattedEra.Split('-')[0]);
            int maxYear = Convert.ToInt32(formattedEra.Split('-')[1]);

            return (minYear <= year && year <= maxYear); // minYear <= year <= maxYear
        }
        public static MusicEra ConvertFromYear(int year) // Determines in which era is the given year
        {
            if (year == 0)
                return MusicEra.Unknown;

            // Probably not the most resource efficient way, but it suffices for now
            // Might be better to implement some logic based on the (int) values of the enum and last 2 digits of year
            foreach (MusicEra era in Enum.GetValues(typeof(MusicEra)).Cast<MusicEra>()) // Casting to get typed iteration, just in case
            {
                if (era == MusicEra.Unknown)
                    continue; // Cannot determine inclusion

                if (IsDateInEra(year, era))
                    return era;
            }

            return MusicEra.Unknown;
        }
    }
}