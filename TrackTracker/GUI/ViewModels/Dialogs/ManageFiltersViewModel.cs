using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

using TrackTracker.GUI.Interfaces;
using TrackTracker.GUI.ViewModels.Base;



namespace TrackTracker.GUI.ViewModels.Dialogs
{
    public class ManageFiltersViewModel : ViewModelBase
    {
        public ManageFiltersViewModel()
        {
            AvailableArtists = new ObservableCollection<string>();
            AvailableAlbums = new ObservableCollection<string>();
            AvailableGenres = new ObservableCollection<string>();

            ClearCommand = new RelayCommand<object>(exe => ExecuteClear(), can => CanExecuteClear);
            CloseCommand = new RelayCommand<IClosable>(exe => ExecuteClose(exe), can => CanExecuteClose);
        }


        public ObservableCollection<string> AvailableArtists { get; set; }
        public ObservableCollection<string> AvailableAlbums { get; set; }
        public ObservableCollection<string> AvailableGenres { get; set; }

        public ICommand ClearCommand { get; }
        public ICommand CloseCommand { get; }



        private string selectedArtist;
        public string SelectedArtist
        {
            get => selectedArtist;
            set { SetProperty(ref selectedArtist, value); }
        }

        private string selectedAlbum;
        public string SelectedAlbum
        {
            get => selectedAlbum;
            set { SetProperty(ref selectedAlbum, value); }
        }

        private string selectedGenre;
        public string SelectedGenre
        {
            get => selectedGenre;
            set { SetProperty(ref selectedGenre, value); }
        }



        public bool CanExecuteClear
        {
            get => SelectedArtist != null || SelectedAlbum != null || SelectedGenre != null;
        }
        private void ExecuteClear()
        {
            SelectedArtist = null;
            SelectedAlbum = null;
            SelectedGenre = null;
        }

        public bool CanExecuteClose
        {
            get => true;
        }
        private void ExecuteClose(IClosable window)
        {
            if (window != null)
                window.Close();
        }
    }
}
