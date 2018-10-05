using System;
using System.Collections.Generic;



namespace TrackTracker.BLL
{
    /*
    Enum: Decades
    Description:
        Custom enumeration of decades for various tracks. For statistical grouping purposes.
    */
    public enum Decades
    {
        Unknown,
        Years_1950_1959,
        Years_1960_1969,
        Years_1970_1979,
        Years_1980_1989,
        Years_1990_1999,
        Years_2000_2009,
        Years_2010_2019,
        Years_2020_2029,
    }

    /*
    Class: Statistics
    Description:
        Generated each time the Tracklist changes and holds all currently generated statistical values of it.
    */
    public class Statistics
    {
        private int totalCount;
        private int properlyTagged;
        private Dictionary<string, uint> countsByArtist;
        private Dictionary<string, uint> countsByAlbum;
        private Dictionary<string, uint> countsByGenre;
        private Dictionary<Decades, uint> countsByDecade;

        public int TotalCount { get => totalCount; }
        public int ProperlyTagged { get => properlyTagged; }
        public Dictionary<string, uint> CountsByArtist { get => new Dictionary<string, uint>(countsByArtist); } //copying to avoid modifications from outside
        public Dictionary<string, uint> CountsByAlbum { get => new Dictionary<string, uint>(countsByAlbum); } //copying to avoid modifications from outside
        public Dictionary<string, uint> CountsByGenre { get => new Dictionary<string, uint>(countsByGenre); } //copying to avoid modifications from outside
        public Dictionary<Decades, uint> CountsByDecade { get => new Dictionary<Decades, uint>(countsByDecade); } //copying to avoid modifications from outside

        public Statistics()
        {
            this.totalCount = -1;
            this.properlyTagged = -1;
            this.countsByArtist = new Dictionary<string, uint>();
            this.countsByAlbum = new Dictionary<string, uint>();
            this.countsByGenre = new Dictionary<string, uint>();
            this.countsByDecade = new Dictionary<Decades, uint>();
        }

        public void GenerateStatistics(List<Track> tracks,
            bool totalCount, bool properlyTagged, bool countsByArtist,
            bool countsByAlbum, bool countsByGenre, bool countsByDecade)
        {
            if (totalCount) this.totalCount = tracks.Count; //does not require own method
            if (properlyTagged) this.properlyTagged = CountProperlyTagged(tracks);
            if (countsByArtist) this.countsByArtist = CalculateCountsByArtist(tracks);
            if (countsByAlbum) this.countsByAlbum = CalculateCountsByAlbum(tracks);
            if (countsByGenre) this.countsByGenre = CalculateCountsByGenre(tracks);
            if (countsByDecade) this.countsByDecade = CalculateCountsByDecade(tracks);
        }

        public bool GetMostFrequentArtist(out string artist, out uint count)
        {
            if (countsByArtist.Count > 0)
            {
                uint maxCount = 0;
                string maxArtist = "";
                foreach (KeyValuePair<string, uint> entry in countsByArtist)
                {
                    if (entry.Value > maxCount)
                    {
                        maxCount = entry.Value;
                        maxArtist = entry.Key;
                    }
                }
                artist = maxArtist;
                count = maxCount;
                return true;
            }
            else
            {
                artist = null;
                count = 0;
                return false;
            }
        }
        public bool GetMostFrequentAlbum(out string album, out uint count)
        {
            if (countsByAlbum.Count > 0)
            {
                uint maxCount = 0;
                string maxAlbum = "";
                foreach (KeyValuePair<string, uint> entry in countsByAlbum)
                {
                    if (entry.Value > maxCount)
                    {
                        maxCount = entry.Value;
                        maxAlbum = entry.Key;
                    }
                }
                album = maxAlbum;
                count = maxCount;
                return true;
            }
            else
            {
                album = null;
                count = 0;
                return false;
            }
        }
        public bool GetMostFrequentGenre(out string genre, out uint count)
        {
            if (countsByGenre.Count > 0)
            {
                uint maxCount = 0;
                string maxGenre = "";
                foreach (KeyValuePair<string, uint> entry in countsByGenre)
                {
                    if (entry.Value > maxCount)
                    {
                        maxCount = entry.Value;
                        maxGenre = entry.Key;
                    }
                }
                genre = maxGenre;
                count = maxCount;
                return true;
            }
            else
            {
                genre = null;
                count = 0;
                return false;
            }
        }
        public bool GetMostFrequentDecade(out Decades decade, out uint count)
        {
            if (countsByDecade.Count > 0)
            {
                uint maxCount = 0;
                Decades maxDecade = Decades.Unknown;
                foreach (KeyValuePair<Decades, uint> entry in countsByDecade)
                {
                    if (entry.Value > maxCount)
                    {
                        maxCount = entry.Value;
                        maxDecade = entry.Key;
                    }
                }
                decade = maxDecade;
                count = maxCount;
                return true;
            }
            else
            {
                decade = Decades.Unknown;
                count = 0;
                return false;
            }
        }

        public List<StatisticalData> GetCountsByArtist()
        {
            List<StatisticalData> data = new List<StatisticalData>();
            foreach (KeyValuePair<string, uint> pair in countsByArtist)
            {
                data.Add(new StatisticalData() { Name = pair.Key, Count = pair.Value });
            }
            return data;
        }

        private int CountProperlyTagged(List<Track> tracks)
        {
            int count = 0;
            foreach (Track track in tracks)
            {
                //we assume that if MBID for the track / release was properly set, then every other metadata were too
                //it's rather safe to do and avoids exhausting metadata check (furthermore doesn't raise the question: when do we recognize a set of metadata as complete)
                if (!String.IsNullOrEmpty(track.MetaData.MusicBrainzReleaseId) || !String.IsNullOrEmpty(track.MetaData.MusicBrainzTrackId)) count++;
            }
            return count;
        }
        private Dictionary<string, uint> CalculateCountsByArtist(List<Track> tracks)
        {
            Dictionary<string, uint> counts = new Dictionary<string, uint>();
            foreach (Track track in tracks)
            {
                if (!String.IsNullOrEmpty(track.MetaData.JoinedAlbumArtists)) //we can determine artist from metadata
                {
                    if (!counts.ContainsKey(track.MetaData.JoinedAlbumArtists)) //it is the first time we encounter this particular artist
                    {
                        counts.Add(track.MetaData.JoinedAlbumArtists, 1); //adding occurence of 1, since it's the first
                    }
                    else //we have a pre-existing count about this particular artist
                    {
                        uint originalCount;
                        counts.TryGetValue(track.MetaData.JoinedAlbumArtists, out originalCount); //getting the original counter
                        counts.Remove(track.MetaData.JoinedAlbumArtists); //we will increment the counter's value so we remove
                        uint newCount = originalCount + 1; //incrementing by 1
                        counts.Add(track.MetaData.JoinedAlbumArtists, newCount); //inserting artist with new value
                    }
                }
                else //we cannot determine artist, so we list it under "Unknown"
                {
                    if (!counts.ContainsKey("Unknown")) //it is the first undetermined artist
                    {
                        counts.Add("Unknown", 1); //adding occurence of 1, since it's the first
                    }
                    else //we already have undetermined artists
                    {
                        uint originalCount;
                        counts.TryGetValue("Unknown", out originalCount); //getting the original counter
                        counts.Remove("Unknown"); //we will increment the counter's value so we remove
                        uint newCount = originalCount + 1; //incrementing by 1
                        counts.Add("Unknown", newCount); //inserting unkown artists with new value
                    }
                }
            }
            return counts;
        }
        private Dictionary<string, uint> CalculateCountsByAlbum(List<Track> tracks)
        {
            Dictionary<string, uint> counts = new Dictionary<string, uint>();
            foreach (Track track in tracks)
            {
                if (!String.IsNullOrEmpty(track.MetaData.Album)) //we can determine album from metadata
                {
                    if (!counts.ContainsKey(track.MetaData.Album)) //it is the first time we encounter this particular album
                    {
                        counts.Add(track.MetaData.Album, 1); //adding occurence of 1, since it's the first
                    }
                    else //we have a pre-existing count about this particular album
                    {
                        uint originalCount;
                        counts.TryGetValue(track.MetaData.Album, out originalCount); //getting the original counter
                        counts.Remove(track.MetaData.Album); //we will increment the counter's value so we remove
                        uint newCount = originalCount + 1; //incrementing by 1
                        counts.Add(track.MetaData.Album, newCount); //inserting album with new value
                    }
                }
                else //we cannot determine album, so we list it under "Unknown"
                {
                    if (!counts.ContainsKey("Unknown")) //it is the first undetermined album
                    {
                        counts.Add("Unknown", 1); //adding occurence of 1, since it's the first
                    }
                    else //we already have undetermined albums
                    {
                        uint originalCount;
                        counts.TryGetValue("Unknown", out originalCount); //getting the original counter
                        counts.Remove("Unknown"); //we will increment the counter's value so we remove
                        uint newCount = originalCount + 1; //incrementing by 1
                        counts.Add("Unknown", newCount); //inserting unkown albums with new value
                    }
                }
            }
            return counts;
        }
        private Dictionary<string, uint> CalculateCountsByGenre(List<Track> tracks)
        {
            Dictionary<string, uint> counts = new Dictionary<string, uint>();
            foreach (Track track in tracks)
            {
                if (!String.IsNullOrEmpty(track.MetaData.JoinedGenres)) //we can determine genre from metadata
                {
                    if (!counts.ContainsKey(track.MetaData.JoinedGenres)) //it is the first time we encounter this particular genre
                    {
                        counts.Add(track.MetaData.JoinedGenres, 1); //adding occurence of 1, since it's the first
                    }
                    else //we have a pre-existing count about this particular genre
                    {
                        uint originalCount;
                        counts.TryGetValue(track.MetaData.JoinedGenres, out originalCount); //getting the original counter
                        counts.Remove(track.MetaData.JoinedGenres); //we will increment the counter's value so we remove
                        uint newCount = originalCount + 1; //incrementing by 1
                        counts.Add(track.MetaData.JoinedGenres, newCount); //inserting genre with new value
                    }
                }
                else //we cannot determine genre, so we list it under "Unknown"
                {
                    if (!counts.ContainsKey("Unknown")) //it is the first undetermined genre
                    {
                        counts.Add("Unknown", 1); //adding occurence of 1, since it's the first
                    }
                    else //we already have undetermined genres
                    {
                        uint originalCount;
                        counts.TryGetValue("Unknown", out originalCount); //getting the original counter
                        counts.Remove("Unknown"); //we will increment the counter's value so we remove
                        uint newCount = originalCount + 1; //incrementing by 1
                        counts.Add("Unknown", newCount); //inserting unkown genres with new value
                    }
                }
            }
            return counts;
        }
        private Dictionary<Decades, uint> CalculateCountsByDecade(List<Track> tracks)
        {
            Dictionary<Decades, uint> counts = new Dictionary<Decades, uint>();
            foreach (Track track in tracks)
            {
                Decades decade = GetDecadeFromYear(track.MetaData.Year); //we do not have to worry about value check, since it will "unknown" if not recognized

                if (!counts.ContainsKey(decade)) //it is the first time we encounter this particular decade
                {
                    counts.Add(decade, 1); //adding occurence of 1, since it's the first
                }
                else //we have a pre-existing count about this particular decade
                {
                    uint originalCount;
                    counts.TryGetValue(decade, out originalCount); //getting the original counter
                    counts.Remove(decade); //we will increment the counter's value so we remove
                    uint newCount = originalCount + 1; //incrementing by 1
                    counts.Add(decade, newCount); //inserting decade with new value
                }
            }
            return counts;
        }
        private Decades GetDecadeFromYear(uint year)
        {
                 if (year >= 1950 && year <= 1959) return Decades.Years_1950_1959;
            else if (year >= 1960 && year <= 1969) return Decades.Years_1960_1969;
            else if (year >= 1970 && year <= 1979) return Decades.Years_1970_1979;
            else if (year >= 1980 && year <= 1989) return Decades.Years_1980_1989;
            else if (year >= 1990 && year <= 1999) return Decades.Years_1990_1999;
            else if (year >= 2000 && year <= 2009) return Decades.Years_2000_2009;
            else if (year >= 2010 && year <= 2019) return Decades.Years_2010_2019;
            else if (year >= 2020 && year <= 2029) return Decades.Years_2020_2029;
            else return Decades.Unknown;
        }
    }
}
