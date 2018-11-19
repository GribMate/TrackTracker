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

            SelectAllCommand = new RelayCommand(exe => ExecuteSelectAll(), can => CanExecuteSelectAll);
            SelectReverseCommand = new RelayCommand(exe => ExecuteSelectReverse(), can => CanExecuteSelectReverse);
            AddToMixCommand = new RelayCommand(exe => ExecuteAddToMix(), can => CanExecuteAddToMix);
            SpotifyPlayPauseCommand = new RelayCommand(exe => ExecuteSpotifyPlayPause(), can => CanExecuteSpotifyPlayPause);
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
                            GlobalContext.AppConfig.AddMediaPlayerPath(value, path);
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
            get => GlobalContext.PlayzoneTracks.Count > 0;
        }
        private void ExecuteSelectAll()
        {
            foreach (Track track in GlobalContext.PlayzoneTracks)
            {
                track.IsSelectedInGUI = true;
            }
            NotifyPropertyChanged(nameof(CanExecuteAddToMix));
        }

        public bool CanExecuteSelectReverse
        {
            get => GlobalContext.PlayzoneTracks.Count > 0;
        }
        public void ExecuteSelectReverse()
        {
            foreach (Track track in GlobalContext.PlayzoneTracks)
            {
                if (track.IsSelectedInGUI) track.IsSelectedInGUI = false;
                else track.IsSelectedInGUI = true;
            }
            NotifyPropertyChanged(nameof(CanExecuteAddToMix));
        }

        public bool CanExecuteAddToMix
        {
            get
            {
                if (SelectedSupportedMediaPlayer == SupportedMediaPlayers.Foobar2000)
                {
                    foreach (Track track in GlobalContext.PlayzoneTracks)
                    {
                        if (track.IsSelectedInGUI)
                            return true; // Foobar2000 is selected as target and we have selected track(s) to add
                    }
                }

                return false;
            }
        }
        public void ExecuteAddToMix()
        {
            System.IO.StreamWriter sw = new System.IO.StreamWriter("D:\\testplaylist.m3u");
            foreach (Track track in GlobalContext.PlayzoneTracks)
            {
                if (track.IsSelectedInGUI) sw.WriteLine(track.FileHandle.Name);
            }
            sw.Flush();
            sw.Close();
            sw.Dispose();
            System.Diagnostics.Process.Start("D:\\testplaylist.m3u");
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