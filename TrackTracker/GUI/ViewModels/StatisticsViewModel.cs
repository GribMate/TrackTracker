using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

using TrackTracker.BLL;
using TrackTracker.BLL.Enums;
using TrackTracker.BLL.Model;
using TrackTracker.BLL.GlobalContexts;
using TrackTracker.GUI.ViewModels.Base;



namespace TrackTracker.GUI.ViewModels
{
    public class StatisticsViewModel : ViewModelBase
    {
        public StatisticsViewModel() : base()
        {
            TotalCount = 0;
            ProperlyTaggedCount = 0;
            CountsByArtist = new Dictionary<string, uint>();
            CountsByAlbum = new Dictionary<string, uint>();
            CountsByGenre = new Dictionary<string, uint>();
            CountsByDecade = new Dictionary<MusicEra, uint>();

            RefreshCommand = new RelayCommand<object>(exe => ExecuteRefresh(), can => CanExecuteRefresh);
        }

        public ICommand RefreshCommand { get; }

        public Dictionary<string, uint> CountsByArtist { get; private set; }
        public Dictionary<string, uint> CountsByAlbum { get; private set; }
        public Dictionary<string, uint> CountsByGenre { get; private set; }
        public Dictionary<MusicEra, uint> CountsByDecade { get; private set; }



        private string artistName;
        public string ArtistName
        {
            get => artistName;
            set
            {
                SetProperty(ref artistName, value);
            }
        }

        private uint artistCount;
        public uint ArtistCount
        {
            get => artistCount;
            set
            {
                SetProperty(ref artistCount, value);
            }
        }

        private string albumName;
        public string AlbumName
        {
            get => albumName;
            set
            {
                SetProperty(ref albumName, value);
            }
        }

        private uint albumCount;
        public uint AlbumCount
        {
            get => albumCount;
            set
            {
                SetProperty(ref albumCount, value);
            }
        }

        private string genreName;
        public string GenreName
        {
            get => genreName;
            set
            {
                SetProperty(ref genreName, value);
            }
        }

        private uint genreCount;
        public uint GenreCount
        {
            get => genreCount;
            set
            {
                SetProperty(ref genreCount, value);
            }
        }

        private string decadeName;
        public string DecadeName
        {
            get => decadeName;
            set
            {
                SetProperty(ref decadeName, value);
            }
        }

        private uint decadeCount;
        public uint DecadeCount
        {
            get => decadeCount;
            set
            {
                SetProperty(ref decadeCount, value);
            }
        }

        private uint totalCount;
        public uint TotalCount
        {
            get => totalCount;
            set
            {
                SetProperty(ref totalCount, value);
            }
        }

        private uint properlyTaggedCount;
        public uint ProperlyTaggedCount
        {
            get => properlyTaggedCount;
            set
            {
                SetProperty(ref properlyTaggedCount, value);
            }
        }

        private string recommendedGenre;
        public string RecommendedGenre
        {
            get => recommendedGenre;
            set
            {
                SetProperty(ref recommendedGenre, value);
            }
        }

        // TODO: CleanCode

        private bool CanExecuteRefresh
        {
            get => TracklistContext.TracklistTracks.Count > 0 || PlayzoneContext.SpotifyTracks.Count > 0;
        }
        private void ExecuteRefresh()
        {
            TotalCount = (uint)TracklistContext.TracklistTracks.Count; // Does not require own method
            ProperlyTaggedCount = (uint)CountProperlyTagged();
            CountsByArtist = CalculateCountsByArtist();
            CountsByAlbum = CalculateCountsByAlbum();
            CountsByGenre = CalculateCountsByGenre();
            CountsByDecade = CalculateCountsByDecade();

            if (StatisticsContext.CountsByArtist.Count != CountsByArtist.Count)
            {
                StatisticsContext.CountsByArtist.Clear();
                foreach (PieChartableData item in UtilityHelper.ConvertStatistics(CountsByArtist))
                {
                    StatisticsContext.CountsByArtist.Add(item);
                }
            }

            if (StatisticsContext.CountsByAlbum.Count != CountsByAlbum.Count)
            {
                StatisticsContext.CountsByAlbum.Clear();
                foreach (PieChartableData item in UtilityHelper.ConvertStatistics(CountsByAlbum))
                {
                    StatisticsContext.CountsByAlbum.Add(item);
                }
            }

            if (StatisticsContext.CountsByGenre.Count != CountsByGenre.Count)
            {
                StatisticsContext.CountsByGenre.Clear();
                foreach (PieChartableData item in UtilityHelper.ConvertStatistics(CountsByGenre))
                {
                    StatisticsContext.CountsByGenre.Add(item);
                }
            }

            if (StatisticsContext.CountsByDecade.Count != CountsByDecade.Count)
            {
                StatisticsContext.CountsByDecade.Clear();
                foreach (PieChartableData item in UtilityHelper.ConvertStatistics(CountsByDecade))
                {
                    StatisticsContext.CountsByDecade.Add(item);
                }
            }

            if (GetMostFrequentArtist(out string artistName, out uint artistCount))
            {
                ArtistName = artistName;
                ArtistCount = artistCount;
            }
            else
                ArtistName = "-";

            if (GetMostFrequentAlbum(out string albumName, out uint albumCount))
            {
                AlbumName = albumName;
                AlbumCount = albumCount;
            }
            else
                AlbumName = "-";

            if (GetMostFrequentGenre(out string genreName, out uint genreCount))
            {
                GenreName = genreName;
                GenreCount = genreCount;
            }
            else
                GenreName = "-";

            if (GetMostFrequentDecade(out MusicEra decadeName, out uint decadeCount))
            {
                DecadeName = decadeName.ToString(); // TODO: convert properly
                DecadeCount = decadeCount;
            }
            else
                DecadeName = "-";

            if (TotalCount != 0)
            {
                RecommendedGenre = "Hmm... it seems you rather like " + GenreName + "! You might want to check out " + GetRecommendedBand() + " for a change!";
            }
        }



        private string GetRecommendedBand() // Just a trial feature for fun
        {
            string recommendedBand = "Linkin Park"; // Default - R.I.P. Chester

            if (GenreName == "Hip hop")
                if (ArtistName != "Hollywood Undead")
                    recommendedBand = "Hollywood Undead";
                else
                    recommendedBand = "Gorillaz";

            if (GenreName == "Punk")
                if (ArtistName != "Good Charlotte")
                    recommendedBand = "Good Charlotte";
                else
                    recommendedBand = "Green Day";

            if (GenreName == "Rock")
                if (ArtistName != "Linkin Park")
                    recommendedBand = "Linkin Park";
                else
                    recommendedBand = "AC/DC";

            if (GenreName == "Rap")
                if (ArtistName != "Eminem")
                    recommendedBand = "Eminem";
                else
                    recommendedBand = "Macklemore";

            if (GenreName == "Pop")
                if (ArtistName != "Lady Gaga")
                    recommendedBand = "Lady Gaga";
                else
                    recommendedBand = "Ke$ha";

            if (GenreName == "Dubstep")
                if (ArtistName != "Skrillex")
                    recommendedBand = "Skrillex";
                else
                    recommendedBand = "Example";

            return recommendedBand;
        }

        private int CountProperlyTagged()
        {
            int count = 0;
            foreach (TrackLocal track in TracklistContext.TracklistTracks)
            {
                //we assume that if MBID for the track / release was properly set, then every other metadata were too
                //it's rather safe to do and avoids exhausting metadata check (furthermore doesn't raise the question: when do we recognize a set of metadata as complete)
                if (!String.IsNullOrEmpty(track.MetaData.MusicBrainzReleaseId.ToString()) || !String.IsNullOrEmpty(track.MetaData.MusicBrainzTrackId.ToString())) count++;
            }
            return count;
        }

        private bool GetMostFrequentArtist(out string artist, out uint count)
        {
            if (CountsByArtist.Count > 0)
            {
                uint maxCount = 0;
                string maxArtist = "";
                foreach (KeyValuePair<string, uint> entry in CountsByArtist)
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
        private bool GetMostFrequentAlbum(out string album, out uint count)
        {
            if (CountsByAlbum.Count > 0)
            {
                uint maxCount = 0;
                string maxAlbum = "";
                foreach (KeyValuePair<string, uint> entry in CountsByAlbum)
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
        private bool GetMostFrequentGenre(out string genre, out uint count)
        {
            if (CountsByGenre.Count > 0)
            {
                uint maxCount = 0;
                string maxGenre = "";
                foreach (KeyValuePair<string, uint> entry in CountsByGenre)
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
        private bool GetMostFrequentDecade(out MusicEra decade, out uint count)
        {
            if (CountsByDecade.Count > 0)
            {
                uint maxCount = 0;
                MusicEra maxDecade = MusicEra.Unknown;
                foreach (KeyValuePair<MusicEra, uint> entry in CountsByDecade)
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
                decade = MusicEra.Unknown;
                count = 0;
                return false;
            }
        }


        private Dictionary<string, uint> CalculateCountsByArtist()
        {
            Dictionary<string, uint> counts = new Dictionary<string, uint>();

            foreach (TrackLocal track in TracklistContext.TracklistTracks)
            {
                if (!String.IsNullOrEmpty(track.MetaData.AlbumArtists.JoinedValue)) //we can determine artist from metadata
                {
                    if (!counts.ContainsKey(track.MetaData.AlbumArtists.JoinedValue)) //it is the first time we encounter this particular artist
                    {
                        counts.Add(track.MetaData.AlbumArtists.JoinedValue, 1); //adding occurence of 1, since it's the first
                    }
                    else //we have a pre-existing count about this particular artist
                    {
                        uint originalCount;
                        counts.TryGetValue(track.MetaData.AlbumArtists.JoinedValue, out originalCount); //getting the original counter
                        counts.Remove(track.MetaData.AlbumArtists.JoinedValue); //we will increment the counter's value so we remove
                        uint newCount = originalCount + 1; //incrementing by 1
                        counts.Add(track.MetaData.AlbumArtists.JoinedValue, newCount); //inserting artist with new value
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

            foreach (TrackVirtual track in PlayzoneContext.SpotifyTracks)
            {
                if (!String.IsNullOrEmpty(track.MetaData.AlbumArtists.JoinedValue)) //we can determine artist from metadata
                {
                    if (!counts.ContainsKey(track.MetaData.AlbumArtists.JoinedValue)) //it is the first time we encounter this particular artist
                    {
                        counts.Add(track.MetaData.AlbumArtists.JoinedValue, 1); //adding occurence of 1, since it's the first
                    }
                    else //we have a pre-existing count about this particular artist
                    {
                        uint originalCount;
                        counts.TryGetValue(track.MetaData.AlbumArtists.JoinedValue, out originalCount); //getting the original counter
                        counts.Remove(track.MetaData.AlbumArtists.JoinedValue); //we will increment the counter's value so we remove
                        uint newCount = originalCount + 1; //incrementing by 1
                        counts.Add(track.MetaData.AlbumArtists.JoinedValue, newCount); //inserting artist with new value
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
        private Dictionary<string, uint> CalculateCountsByAlbum()
        {
            Dictionary<string, uint> counts = new Dictionary<string, uint>();
            foreach (TrackLocal track in TracklistContext.TracklistTracks)
            {
                if (!String.IsNullOrEmpty(track.MetaData.Album.ToString())) //we can determine album from metadata
                {
                    if (!counts.ContainsKey(track.MetaData.Album.ToString())) //it is the first time we encounter this particular album
                    {
                        counts.Add(track.MetaData.Album.ToString(), 1); //adding occurence of 1, since it's the first
                    }
                    else //we have a pre-existing count about this particular album
                    {
                        uint originalCount;
                        counts.TryGetValue(track.MetaData.Album.ToString(), out originalCount); //getting the original counter
                        counts.Remove(track.MetaData.Album.ToString()); //we will increment the counter's value so we remove
                        uint newCount = originalCount + 1; //incrementing by 1
                        counts.Add(track.MetaData.Album.ToString(), newCount); //inserting album with new value
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

            foreach (TrackVirtual track in PlayzoneContext.SpotifyTracks)
            {
                if (!String.IsNullOrEmpty(track.MetaData.Album.ToString())) //we can determine album from metadata
                {
                    if (!counts.ContainsKey(track.MetaData.Album.ToString())) //it is the first time we encounter this particular album
                    {
                        counts.Add(track.MetaData.Album.ToString(), 1); //adding occurence of 1, since it's the first
                    }
                    else //we have a pre-existing count about this particular album
                    {
                        uint originalCount;
                        counts.TryGetValue(track.MetaData.Album.ToString(), out originalCount); //getting the original counter
                        counts.Remove(track.MetaData.Album.ToString()); //we will increment the counter's value so we remove
                        uint newCount = originalCount + 1; //incrementing by 1
                        counts.Add(track.MetaData.Album.ToString(), newCount); //inserting album with new value
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
        private Dictionary<string, uint> CalculateCountsByGenre()
        {
            Dictionary<string, uint> counts = new Dictionary<string, uint>();
            foreach (TrackLocal track in TracklistContext.TracklistTracks)
            {
                if (track.MetaData.Genres.Value != null && track.MetaData.Genres.Value.Length > 0) //we can determine genre from metadata
                {
                    foreach (string genre in track.MetaData.Genres.Value)
                    {
                        if (!counts.ContainsKey(genre)) //it is the first time we encounter this particular genre
                        {
                            counts.Add(genre, 1); //adding occurence of 1, since it's the first
                        }
                        else //we have a pre-existing count about this particular genre
                        {
                            uint originalCount;
                            counts.TryGetValue(genre, out originalCount); //getting the original counter
                            counts.Remove(genre); //we will increment the counter's value so we remove
                            uint newCount = originalCount + 1; //incrementing by 1
                            counts.Add(genre, newCount); //inserting genre with new value
                        }
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

            foreach (TrackVirtual track in PlayzoneContext.SpotifyTracks)
            {
                if (track.MetaData.Genres.Value != null && track.MetaData.Genres.Value.Length > 0) //we can determine genre from metadata
                {
                    foreach (string genre in track.MetaData.Genres.Value)
                    {
                        if (!counts.ContainsKey(genre)) //it is the first time we encounter this particular genre
                        {
                            counts.Add(genre, 1); //adding occurence of 1, since it's the first
                        }
                        else //we have a pre-existing count about this particular genre
                        {
                            uint originalCount;
                            counts.TryGetValue(genre, out originalCount); //getting the original counter
                            counts.Remove(genre); //we will increment the counter's value so we remove
                            uint newCount = originalCount + 1; //incrementing by 1
                            counts.Add(genre, newCount); //inserting genre with new value
                        }
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
        private Dictionary<MusicEra, uint> CalculateCountsByDecade()
        {
            Dictionary<MusicEra, uint> counts = new Dictionary<MusicEra, uint>();
            foreach (TrackLocal track in TracklistContext.TracklistTracks)
            {
                MusicEra decade = MusicEra.Unknown;

                if (track.MetaData.Year.Value.HasValue)
                    decade = MusicErasConverter.ConvertFromYear(track.MetaData.Year.Value.Value); //we do not have to worry about value check, since it will "unknown" if not recognized

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

            foreach (TrackVirtual track in PlayzoneContext.SpotifyTracks)
            {
                MusicEra decade = MusicEra.Unknown;

                if (track.MetaData.Year.Value.HasValue)
                    decade = MusicErasConverter.ConvertFromYear(track.MetaData.Year.Value.Value); //we do not have to worry about value check, since it will "unknown" if not recognized

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
    }
}