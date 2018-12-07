using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

using TrackTracker.Services.Interfaces;
using TrackTracker.BLL;
using TrackTracker.BLL.Enums;
using TrackTracker.BLL.Model;
using TrackTracker.BLL.GlobalContexts;
using TrackTracker.GUI.ViewModels.Base;



namespace TrackTracker.GUI.ViewModels
{
    public class PlayzoneViewModel : ViewModelBase
    {
        private ISpotifyService spotifyService;
        private IFoobarService foobarService;

        private PlayzoneBackgroundWorker player;

        private string localSelectedArtist;
        private string localSelectedAlbum;
        private string localSelectedGenre;

        private string onlineSelectedArtist;
        private string onlineSelectedAlbum;
        private string onlineSelectedGenre;

        public ObservableCollection<string> LocalSupportedPlayers { get; set; }
        public ObservableCollection<string> OnlineSupportedPlayers { get; set; }
        public ObservableCollection<TrackBase> MixList { get; set; }

        public ICommand LocalSelectAllCommand { get; }
        public ICommand LocalSelectReverseCommand { get; }
        public ICommand LocalManageFiltersCommand { get; }
        public ICommand LocalAddToMixCommand { get; }

        public ICommand OnlineSelectAllCommand { get; }
        public ICommand OnlineSelectReverseCommand { get; }
        public ICommand OnlineManageFiltersCommand { get; }
        public ICommand OnlineAddToMixCommand { get; }

        public ICommand PlayCommand { get; }
        public ICommand PauseCommand { get; }
        public ICommand RemoveCommand { get; }

        public PlayzoneViewModel() : base()
        {
            spotifyService = DependencyInjector.GetService<ISpotifyService>();
            foobarService = DependencyInjector.GetService<IFoobarService>();

            player = new PlayzoneBackgroundWorker();

            localSelectedArtist = null;
            localSelectedAlbum = null;
            localSelectedGenre = null;

            onlineSelectedArtist = null;
            onlineSelectedAlbum = null;
            onlineSelectedGenre = null;

        LocalSupportedPlayers = GetLocalSupportedPlayers();
            OnlineSupportedPlayers = GetOnlineSupportedPlayers();
            MixList = new ObservableCollection<TrackBase>();

            LocalSelectAllCommand = new RelayCommand<object>(exe => ExecuteLocalSelectAll(), can => CanExecuteLocalSelectAll);
            LocalSelectReverseCommand = new RelayCommand<object>(exe => ExecuteLocalSelectReverse(), can => CanExecuteLocalSelectReverse);
            LocalManageFiltersCommand = new RelayCommand<object>(exe => ExecuteLocalManageFilters(), can => CanExecuteLocalManageFilters);
            LocalAddToMixCommand = new RelayCommand<object>(exe => ExecuteLocalAddToMix(), can => CanExecuteLocalAddToMix);

            OnlineSelectAllCommand = new RelayCommand<object>(exe => ExecuteOnlineSelectAll(), can => CanExecuteOnlineSelectAll);
            OnlineSelectReverseCommand = new RelayCommand<object>(exe => ExecuteOnlineSelectReverse(), can => CanExecuteOnlineSelectReverse);
            OnlineManageFiltersCommand = new RelayCommand<object>(exe => ExecuteOnlineManageFilters(), can => CanExecuteOnlineManageFilters);
            OnlineAddToMixCommand = new RelayCommand<object>(exe => ExecuteOnlineAddToMix(), can => CanExecuteOnlineAddToMix);

            PlayCommand = new RelayCommand<object>(exe => ExecutePlay(), can => CanExecutePlay);
            PauseCommand = new RelayCommand<object>(exe => ExecutePause(), can => CanExecutePause);
            RemoveCommand = new RelayCommand<object>(exe => ExecuteRemove(), can => CanExecuteRemove);
        }



        public string LocalTitle
        {
            get
            {
                if (localSelectedArtist == null && localSelectedAlbum == null && localSelectedGenre == null)
                    return "Local music files";
                else
                    return "Local music files (filtered)";
            }
        }

        public string OnlineTitle
        {
            get
            {
                if (onlineSelectedArtist == null && onlineSelectedAlbum == null && onlineSelectedGenre == null)
                    return "Online music files";
                else
                    return "Online music files (filtered)";
            }
        }

        private SupportedMediaPlayers? selectedLocalPlayer;
        public SupportedMediaPlayers? SelectedLocalPlayer
        {
            get => selectedLocalPlayer;
            set
            {
                SetProperty(ref selectedLocalPlayer, value);

                switch (value)
                {
                    case SupportedMediaPlayers.Foobar2000:
                        string path = foobarService.DetectFoobarPath();
                        if (path.Length >= 8) // "C:\x.exe" is 8 characters long
                        {
                            SettingsContext.AddMediaPlayerPath(value.Value, path); // "value" is not null
                            LocalPlayerInfo = "Foobar linked!";
                        }
                        else
                        {
                            LocalPlayerInfo = "Cannot find player.";
                            UtilityHelper.ShowExceptionDialog(
                                "Foobar2000 is not installed",
                                "Foobar2000 is not installed on this computer. As the only currently supported local player, it is needed for this feature to work.",
                                "You can download Foobar2000 here: https://www.foobar2000.org/download",
                                null);
                        }
                        break;
                }

                NotifyPropertyChanged(nameof(CanExecutePlay));
                NotifyPropertyChanged(nameof(CanExecutePause));
            }
        }

        private SupportedMediaPlayers? selectedOnlinePlayer;
        public SupportedMediaPlayers? SelectedOnlinePlayer
        {
            get => selectedOnlinePlayer;
            set
            {
                SetProperty(ref selectedOnlinePlayer, value);

                // TODO: check if Spotify is linked

                NotifyPropertyChanged(nameof(CanExecutePlay));
                NotifyPropertyChanged(nameof(CanExecutePause));
            }
        }

        private string localPlayerInfo;
        public string LocalPlayerInfo
        {
            get => localPlayerInfo;
            set { SetProperty(ref localPlayerInfo, value); }
        }



        private bool CanExecuteLocalSelectAll
        {
            get => PlayzoneContext.LocalTracks.Count > 0;
        }
        private void ExecuteLocalSelectAll()
        {
            foreach (TrackLocal track in PlayzoneContext.LocalTracks)
            {
                track.IsSelected = true;
            }
            NotifyPropertyChanged(nameof(CanExecuteLocalAddToMix));
        }

        public bool CanExecuteLocalSelectReverse
        {
            get => PlayzoneContext.LocalTracks.Count > 0;
        }
        public void ExecuteLocalSelectReverse()
        {
            foreach (TrackLocal track in PlayzoneContext.LocalTracks)
            {
                if (track.IsSelected)
                    track.IsSelected = false;
                else
                    track.IsSelected = true;
            }
            NotifyPropertyChanged(nameof(CanExecuteLocalAddToMix));
        }

        public bool CanExecuteLocalManageFilters
        {
            get => PlayzoneContext.LocalTracks.Count > 0;
        }
        public void ExecuteLocalManageFilters()
        {
            Views.Dialogs.ManageFilters mfDialog = new Views.Dialogs.ManageFilters();
            mfDialog.Owner = System.Windows.Application.Current.MainWindow;

            Dialogs.ManageFiltersViewModel vm = new Dialogs.ManageFiltersViewModel()
            {
                AvailableArtists = GetLocalAvailableArtists(),
                AvailableAlbums = GetLocalAvailableAlbums(),
                AvailableGenres = GetLocalAvailableGenres(),

                SelectedArtist = localSelectedArtist,
                SelectedAlbum = localSelectedAlbum,
                SelectedGenre = localSelectedGenre
            };

            mfDialog.DataContext = vm;

            mfDialog.ShowDialog();

            localSelectedArtist = vm.SelectedArtist;
            localSelectedAlbum = vm.SelectedAlbum;
            localSelectedGenre = vm.SelectedGenre;

            FilterLocalList();
        }

        public bool CanExecuteLocalAddToMix
        {
            get
            {
                if (SelectedLocalPlayer != null)
                {
                    foreach (TrackLocal track in PlayzoneContext.LocalTracks)
                    {
                        if (track.IsSelected)
                            return true; // An offline player is selected as target and we have selected track(s) to add
                    }
                }

                return false;
            }
        }
        public void ExecuteLocalAddToMix()
        {
            foreach (TrackLocal track in PlayzoneContext.LocalTracks)
            {
                if (track.IsSelected)
                {
                    TrackBase newTrack = UtilityHelper.CopyTrack(track);
                    newTrack.IsSelected = true;
                    MixList.Add(newTrack);
                    player.AddPlayableTrack(newTrack);
                }
            }

            NotifyPropertyChanged(nameof(MixList));
        }



        private bool CanExecuteOnlineSelectAll
        {
            get => PlayzoneContext.SpotifyTracks.Count > 0;
        }
        private void ExecuteOnlineSelectAll()
        {
            foreach (TrackVirtual track in PlayzoneContext.SpotifyTracks)
            {
                track.IsSelected = true;
            }
            NotifyPropertyChanged(nameof(CanExecuteOnlineAddToMix));
        }

        public bool CanExecuteOnlineSelectReverse
        {
            get => PlayzoneContext.SpotifyTracks.Count > 0;
        }
        public void ExecuteOnlineSelectReverse()
        {
            foreach (TrackVirtual track in PlayzoneContext.SpotifyTracks)
            {
                if (track.IsSelected)
                    track.IsSelected = false;
                else
                    track.IsSelected = true;
            }
            NotifyPropertyChanged(nameof(CanExecuteOnlineAddToMix));
        }

        public bool CanExecuteOnlineManageFilters
        {
            get => PlayzoneContext.SpotifyTracks.Count > 0;
        }
        public void ExecuteOnlineManageFilters()
        {
            Views.Dialogs.ManageFilters mfDialog = new Views.Dialogs.ManageFilters();
            mfDialog.Owner = System.Windows.Application.Current.MainWindow;

            Dialogs.ManageFiltersViewModel vm = new Dialogs.ManageFiltersViewModel()
            {
                AvailableArtists = GetOnlineAvailableArtists(),
                AvailableAlbums = GetOnlineAvailableAlbums(),
                AvailableGenres = GetOnlineAvailableGenres(),

                SelectedArtist = onlineSelectedArtist,
                SelectedAlbum = onlineSelectedAlbum,
                SelectedGenre = onlineSelectedGenre
            };

            mfDialog.DataContext = vm;

            mfDialog.ShowDialog();

            onlineSelectedArtist = vm.SelectedArtist;
            onlineSelectedAlbum = vm.SelectedAlbum;
            onlineSelectedGenre = vm.SelectedGenre;

            FilterOnlineList();
        }

        public bool CanExecuteOnlineAddToMix
        {
            get
            {
                if (SelectedOnlinePlayer != null)
                {
                    foreach (TrackVirtual track in PlayzoneContext.SpotifyTracks)
                    {
                        if (track.IsSelected)
                            return true; // An online player is selected as target and we have selected track(s) to add
                    }
                }

                return false;
            }
        }
        public void ExecuteOnlineAddToMix()
        {
            foreach (TrackVirtual track in PlayzoneContext.SpotifyTracks)
            {
                if (track.IsSelected)
                {
                    TrackBase newTrack = UtilityHelper.CopyTrack(track);
                    newTrack.IsSelected = true;
                    MixList.Add(newTrack);
                    player.AddPlayableTrack(newTrack);
                }
            }

            NotifyPropertyChanged(nameof(MixList));
        }



        public bool CanExecutePlay
        {
            get => MixList.Count > 0;
        }
        public void ExecutePlay()
        {
            player.Start();
        }

        public bool CanExecutePause
        {
            get => MixList.Count > 0;
        }
        public void ExecutePause()
        {
            player.Pause();
        }

        public bool CanExecuteRemove
        {
            get => MixList.Count > 0;
        }
        public void ExecuteRemove()
        {
            List<TrackBase> toRemove = new List<TrackBase>();

            // First, we collect tracks to remove
            foreach (TrackBase track in MixList)
            {
                if (track.IsSelected)
                    toRemove.Add(track);
            }

            // It is necessary to iterate on a "foreign" collection while we modify the target collection
            foreach (TrackBase track in toRemove)
            {
                MixList.Remove(track);
            }

            NotifyPropertyChanged(nameof(MixList));
        }



        private ObservableCollection<string> GetOnlineSupportedPlayers() // Load up the player type selection box with the currently supported values from SupportedMediaPlayers instead of burning values in
        {
            //TODO: after functional refactor this will not work this way
            List<string> players = new List<string>();

            SupportedMediaPlayers supportedOnline = SupportedMediaPlayersConverter.GetAllOnlinePlayers();
            foreach (SupportedMediaPlayers player in Enum.GetValues(typeof(SupportedMediaPlayers)).Cast<SupportedMediaPlayers>()) // Casting to get typed iteration, just in case
            {
                if (supportedOnline.HasFlag(player))
                    players.Add(player.ToString());
            }

            return new ObservableCollection<string>(players);
        }

        private ObservableCollection<string> GetLocalSupportedPlayers() // Load up the player type selection box with the currently supported values from SupportedMediaPlayers instead of burning values in
        {
            //TODO: after functional refactor this will not work this way
            List<string> players = new List<string>();

            SupportedMediaPlayers supportedOnline = SupportedMediaPlayersConverter.GetAllOfflinePlayers();
            foreach (SupportedMediaPlayers player in Enum.GetValues(typeof(SupportedMediaPlayers)).Cast<SupportedMediaPlayers>()) // Casting to get typed iteration, just in case
            {
                if (supportedOnline.HasFlag(player))
                    players.Add(player.ToString());
            }

            return new ObservableCollection<string>(players);
        }

        private ObservableCollection<string> GetLocalAvailableArtists()
        {
            List<string> artists = new List<string>();

            foreach (TrackLocal item in PlayzoneContext.LocalTracks)
            {
                if (artists.Contains(item.MetaData.AlbumArtists.JoinedValue) == false)
                    artists.Add(item.MetaData.AlbumArtists.JoinedValue);
            }

            return new ObservableCollection<string>(artists);
        }

        private ObservableCollection<string> GetLocalAvailableAlbums()
        {
            List<string> albums = new List<string>();

            foreach (TrackLocal item in PlayzoneContext.LocalTracks)
            {
                if (albums.Contains(item.MetaData.Album.Value) == false)
                    albums.Add(item.MetaData.Album.Value);
            }

            return new ObservableCollection<string>(albums);
        }

        private ObservableCollection<string> GetLocalAvailableGenres()
        {
            List<string> genres = new List<string>();

            foreach (TrackLocal item in PlayzoneContext.LocalTracks)
            {
                if (genres.Contains(item.MetaData.Genres.JoinedValue) == false)
                    genres.Add(item.MetaData.Genres.JoinedValue);
            }

            return new ObservableCollection<string>(genres);
        }

        private ObservableCollection<string> GetOnlineAvailableArtists()
        {
            List<string> artists = new List<string>();

            foreach (TrackVirtual item in PlayzoneContext.SpotifyTracks)
            {
                if (artists.Contains(item.MetaData.AlbumArtists.JoinedValue) == false)
                    artists.Add(item.MetaData.AlbumArtists.JoinedValue);
            }

            return new ObservableCollection<string>(artists);
        }

        private ObservableCollection<string> GetOnlineAvailableAlbums()
        {
            List<string> albums = new List<string>();

            foreach (TrackVirtual item in PlayzoneContext.SpotifyTracks)
            {
                if (albums.Contains(item.MetaData.Album.Value) == false)
                    albums.Add(item.MetaData.Album.Value);
            }

            return new ObservableCollection<string>(albums);
        }

        private ObservableCollection<string> GetOnlineAvailableGenres()
        {
            List<string> genres = new List<string>();

            foreach (TrackVirtual item in PlayzoneContext.SpotifyTracks)
            {
                if (genres.Contains(item.MetaData.Genres.JoinedValue) == false)
                    genres.Add(item.MetaData.Genres.JoinedValue);
            }

            return new ObservableCollection<string>(genres);
        }

        private void FilterLocalList()
        {
            PlayzoneContext.FilteredLocalTracks.Clear();

            foreach (TrackLocal track in PlayzoneContext.LocalTracks)
            {
                if (localSelectedArtist == null || (track.MetaData.AlbumArtists.JoinedValue != null && track.MetaData.AlbumArtists.JoinedValue.Equals(localSelectedArtist)))
                {
                    if (localSelectedAlbum == null || (track.MetaData.Album.Value != null && track.MetaData.Album.Value.Equals(localSelectedAlbum)))
                    {
                        if (localSelectedGenre == null || (track.MetaData.Genres.JoinedValue != null && track.MetaData.Genres.JoinedValue.Equals(localSelectedGenre)))
                        {
                            PlayzoneContext.FilteredLocalTracks.Add(track);
                        }
                    }
                }
            }

            NotifyPropertyChanged(nameof(LocalTitle));
        }

        private void FilterOnlineList()
        {
            PlayzoneContext.FilteredSpotifyTracks.Clear();

            foreach (TrackVirtual track in PlayzoneContext.SpotifyTracks)
            {
                if (onlineSelectedArtist == null || (track.MetaData.AlbumArtists.JoinedValue != null && track.MetaData.AlbumArtists.JoinedValue.Equals(onlineSelectedArtist)))
                {
                    if (onlineSelectedAlbum == null || (track.MetaData.Album.Value != null && track.MetaData.Album.Value.Equals(onlineSelectedAlbum)))
                    {
                        if (onlineSelectedGenre == null || (track.MetaData.Genres.JoinedValue != null && track.MetaData.Genres.JoinedValue.Equals(onlineSelectedGenre)))
                        {
                            PlayzoneContext.FilteredSpotifyTracks.Add(track);
                        }
                    }
                }
            }

            NotifyPropertyChanged(nameof(OnlineTitle));
        }
    }
}