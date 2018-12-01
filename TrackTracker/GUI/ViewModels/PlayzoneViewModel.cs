using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private IEnvironmentService environmentService;
        private ISpotifyService spotifyService;

        public ObservableCollection<string> SupportedPlayers { get; set; }

        public ICommand SelectAllCommand { get; }
        public ICommand SelectReverseCommand { get; }
        public ICommand AddToMixCommand { get; }
        public ICommand SpotifyPlayPauseCommand { get; }

        public PlayzoneViewModel() : base()
        {
            environmentService = DependencyInjector.GetService<IEnvironmentService>();
            spotifyService = DependencyInjector.GetService<ISpotifyService>();

            SupportedPlayers = GetSupportedMediaPlayers();

            SelectAllCommand = new RelayCommand<object>(exe => ExecuteSelectAll(), can => CanExecuteSelectAll);
            SelectReverseCommand = new RelayCommand<object>(exe => ExecuteSelectReverse(), can => CanExecuteSelectReverse);
            AddToMixCommand = new RelayCommand<object>(exe => ExecuteAddToMix(), can => CanExecuteAddToMix);
            SpotifyPlayPauseCommand = new RelayCommand<object>(exe => ExecuteSpotifyPlayPause(), can => CanExecuteSpotifyPlayPause);
        }



        private SupportedMediaPlayers selectedSupportedMediaPlayer;
        public SupportedMediaPlayers SelectedSupportedMediaPlayer
        {
            get => selectedSupportedMediaPlayer;
            set
            {
                SetProperty(ref selectedSupportedMediaPlayer, value);

                switch (value)
                {
                    case SupportedMediaPlayers.Foobar2000:
                        string path = environmentService.TryFindFoobar();
                        if (path.Length > 2)
                        {
                            SettingsContext.AddMediaPlayerPath(value, path);
                            PlayerLocation = path + " | PLAYER LINKED!";
                        }
                        break;
                }

                NotifyPropertyChanged(nameof(CanExecuteAddToMix));
                NotifyPropertyChanged(nameof(CanExecuteSpotifyPlayPause));
            }
        }

        private string playerLocation;
        public string PlayerLocation
        {
            get => playerLocation;
            set
            {
                SetProperty(ref playerLocation, value);
            }
        }



        private bool CanExecuteSelectAll
        {
            get => TracklistContext.PlayzoneTracks.Count > 0;
        }
        private void ExecuteSelectAll()
        {
            foreach (TrackLocal track in TracklistContext.PlayzoneTracks)
            {
                track.IsSelected = true;
            }
            NotifyPropertyChanged(nameof(CanExecuteAddToMix));
        }

        public bool CanExecuteSelectReverse
        {
            get => TracklistContext.PlayzoneTracks.Count > 0;
        }
        public void ExecuteSelectReverse()
        {
            foreach (TrackLocal track in TracklistContext.PlayzoneTracks)
            {
                if (track.IsSelected) track.IsSelected = false;
                else track.IsSelected = true;
            }
            NotifyPropertyChanged(nameof(CanExecuteAddToMix));
        }

        public bool CanExecuteAddToMix
        {
            get
            {
                if (SelectedSupportedMediaPlayer == SupportedMediaPlayers.Foobar2000)
                {
                    foreach (TrackLocal track in TracklistContext.PlayzoneTracks)
                    {
                        if (track.IsSelected)
                            return true; // Foobar2000 is selected as target and we have selected track(s) to add
                    }
                }

                return false;
            }
        }
        public void ExecuteAddToMix()
        {
            string playlistPath = AppDomain.CurrentDomain.BaseDirectory + "_playzone_mix.m3u";

            System.IO.StreamWriter sw = new System.IO.StreamWriter(playlistPath);
            foreach (TrackLocal track in TracklistContext.PlayzoneTracks)
            {
                if (track.IsSelected) sw.WriteLine(track.MusicFileProperties.Path);
            }
            sw.Flush();
            sw.Close();
            sw.Dispose();
            System.Diagnostics.Process.Start(playlistPath);
        }

        public bool CanExecuteSpotifyPlayPause
        {
            get => SelectedSupportedMediaPlayer == SupportedMediaPlayers.Spotify;
        }
        public async void ExecuteSpotifyPlayPause()
        {
            PlayerLocation = await spotifyService.TEST_PLAYING();

            spotifyService.TEST_PLAY_PAUSE();
        }



        private ObservableCollection<string> GetSupportedMediaPlayers()
        {
            //TODO: after functional refactor this will not work this way
            List<string> players = new List<string>();
            
            // Load up the player type selection box with the currently supported values from MediaPlayerType instead of burning values in
            foreach (SupportedMediaPlayers player in Enum.GetValues(typeof(SupportedMediaPlayers)).Cast<SupportedMediaPlayers>()) // Casting to get typed iteration, just in case
            {
                players.Add(player.ToString());
            }

            return new ObservableCollection<string>(players);
        }
    }
}