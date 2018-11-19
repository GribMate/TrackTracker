using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

using WinForms = System.Windows.Forms;

using TrackTracker.Services.Interfaces;
using TrackTracker.BLL;
using TrackTracker.BLL.Enums;



namespace TrackTracker.GUI.ViewModels
{
    public class StatisticsViewModel : ViewModelBase
    {
        public ICommand RefreshCommand { get; }

        private Statistics statistics;

        public StatisticsViewModel() : base()
        {
            RefreshCommand = new RelayCommand(exe => ExecuteRefresh(), can => CanExecuteRefresh);

            statistics = new Statistics();
        }



        private string artistName;
        public string ArtistName
        {
            get => artistName;
            set
            {
                SetProperty(ref artistName, value);
                NotifyPropertyChanged(nameof(ArtistName));
            }
        }

        private uint artistCount;
        public uint ArtistCount
        {
            get => artistCount;
            set
            {
                SetProperty(ref artistCount, value);
                NotifyPropertyChanged(nameof(ArtistCount));
            }
        }

        private string albumName;
        public string AlbumName
        {
            get => albumName;
            set
            {
                SetProperty(ref albumName, value);
                NotifyPropertyChanged(nameof(AlbumName));
            }
        }

        private uint albumCount;
        public uint AlbumCount
        {
            get => albumCount;
            set
            {
                SetProperty(ref albumCount, value);
                NotifyPropertyChanged(nameof(AlbumCount));
            }
        }

        private string genreName;
        public string GenreName
        {
            get => genreName;
            set
            {
                SetProperty(ref genreName, value);
                NotifyPropertyChanged(nameof(GenreName));
            }
        }

        private uint genreCount;
        public uint GenreCount
        {
            get => genreCount;
            set
            {
                SetProperty(ref genreCount, value);
                NotifyPropertyChanged(nameof(GenreCount));
            }
        }

        private string decadeName;
        public string DecadeName
        {
            get => decadeName;
            set
            {
                SetProperty(ref decadeName, value);
                NotifyPropertyChanged(nameof(DecadeName));
            }
        }

        private uint decadeCount;
        public uint DecadeCount
        {
            get => decadeCount;
            set
            {
                SetProperty(ref decadeCount, value);
                NotifyPropertyChanged(nameof(DecadeCount));
            }
        }

        private uint totalCount;
        public uint TotalCount
        {
            get => totalCount;
            set
            {
                SetProperty(ref totalCount, value);
                NotifyPropertyChanged(nameof(TotalCount));
            }
        }

        private uint properlyTaggedCount;
        public uint ProperlyTaggedCount
        {
            get => properlyTaggedCount;
            set
            {
                SetProperty(ref properlyTaggedCount, value);
                NotifyPropertyChanged(nameof(ProperlyTaggedCount));
            }
        }

        private string recommendedGenre;
        public string RecommendedGenre
        {
            get => recommendedGenre;
            set
            {
                SetProperty(ref recommendedGenre, value);
                NotifyPropertyChanged(nameof(RecommendedGenre));
            }
        }



        private bool CanExecuteRefresh
        {
            get => GlobalContext.TracklistTracks.Count > 0;
        }
        private void ExecuteRefresh()
        {
            statistics.GenerateStatistics(GlobalContext.TracklistTracks.ToList<Track>(), true, true, true, true, true, true);

            var TODO = new Dictionary<string, uint>(statistics.GetCountsByArtist()); // TODO: TODO

            TotalCount = statistics.TotalCount;
            ProperlyTaggedCount = statistics.ProperlyTaggedCount;

            if (statistics.GetMostFrequentArtist(out string artistName, out uint artistCount))
            {
                ArtistName = artistName;
                ArtistCount = artistCount;
            }
            else
                ArtistName = "-";

            if (statistics.GetMostFrequentAlbum(out string albumName, out uint albumCount))
            {
                AlbumName = albumName;
                AlbumCount = albumCount;
            }
            else
                AlbumName = "-";

            if (statistics.GetMostFrequentGenre(out string genreName, out uint genreCount))
            {
                GenreName = genreName;
                GenreCount = genreCount;
            }
            else
                GenreName = "-";

            if (statistics.GetMostFrequentDecade(out MusicEra decadeName, out uint decadeCount))
            {
                DecadeName = decadeName.ToString(); // TODO: convert properly
                DecadeCount = decadeCount;
            }
            else
                DecadeName = "-";

            if (statistics.TotalCount != 0)
            {
                string recommendedBand = "";
                if (GenreName == "Rock") recommendedBand = "Hollywood Undead";
                else if (GenreName == "Punk") recommendedBand = "Good Charlotte";

                RecommendedGenre = "Hmm... it seems you rather like " + GenreName + "! You might want to check out " + recommendedBand + " for a change!";
            }
        }
    }
}