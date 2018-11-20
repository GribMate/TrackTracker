using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

using WinForms = System.Windows.Forms;

using TrackTracker.Services.Interfaces;
using TrackTracker.BLL;
using TrackTracker.BLL.Enums;
using System.Threading.Tasks;

namespace TrackTracker.GUI.ViewModels
{
    public class TracklistViewModel : ViewModelBase
    {
        private IFileService fileService;
        private IMetadataService metadataService;
        private IFingerprintService fingerprintService;

        private int todo_duration; // TODO

        public ObservableCollection<MetaTag> TagList { get; set; }
        public ObservableCollection<Track> MatchList { get; set; }

        public ICommand SelectAllCommand { get; }
        public ICommand SelectReverseCommand { get; }
        public ICommand ManageSourcesCommand { get; }
        public ICommand UpdateTagsCommand { get; }
        public ICommand GetFingerprintCommand { get; }
        public ICommand SearchMusicBrainzCommand { get; }

        public TracklistViewModel() : base()
        {
            fileService = DependencyInjector.GetService<IFileService>();
            metadataService = DependencyInjector.GetService<IMetadataService>();
            fingerprintService = DependencyInjector.GetService<IFingerprintService>();

            todo_duration = -1;

            TagList = new ObservableCollection<MetaTag>();
            MatchList = new ObservableCollection<Track>();

            SelectAllCommand = new RelayCommand(exe => ExecuteSelectAll(), can => CanExecuteSelectAll);
            SelectReverseCommand = new RelayCommand(exe => ExecuteSelectReverse(), can => CanExecuteSelectReverse);
            ManageSourcesCommand = new RelayCommand(exe => ExecuteManageSources(), can => CanExecuteManageSources);
            UpdateTagsCommand = new RelayCommand(exe => ExecuteUpdateTags(), can => CanExecuteUpdateTags);
            GetFingerprintCommand = new RelayCommand(exe => ExecuteGetFingerprint(), can => CanExecuteGetFingerprint);
            SearchMusicBrainzCommand = new RelayCommand(exe => ExecuteSearchMusicBrainz(), can => CanExecuteSearchMusicBrainz);
        }



        private Track selectedTrack;
        public Track SelectedTrack
        {
            get => selectedTrack;
            set
            {
                SetProperty(ref selectedTrack, value);
                TagList = new ObservableCollection<MetaTag>(value.GetTags());
                NotifyPropertyChanged(nameof(TagList));
            }
        }

        private Track selectedMatch;
        public Track SelectedMatch
        {
            get => selectedMatch;
            set
            {
                SetProperty(ref selectedMatch, value);

                if (SelectedTrack != null)
                {
                    SelectedTrack.AddCandidateTrack(value);
                    SelectedTrack.SelectActiveCandidate(value.MetaData.MusicBrainzTrackId);

                    //FOR DEBUG ONLY
                    Dialogs.ExceptionNotification en = new Dialogs.ExceptionNotification("For debug", "Active candidate selected!");
                    //en.Owner = this;  //TODO
                    en.ShowDialog();
                }
            }
        }

        private bool autoSelect;
        public bool AutoSelect
        {
            get => autoSelect;
            set
            {
                SetProperty(ref autoSelect, value);
            }
        }




        private bool CanExecuteSelectAll
        {
            get => GlobalContext.TracklistTracks.Count > 0;
        }
        private void ExecuteSelectAll()
        {
            foreach (Track track in GlobalContext.TracklistTracks)
            {
                track.IsSelectedInGUI = true;
            }
        }

        public bool CanExecuteSelectReverse
        {
            get => GlobalContext.TracklistTracks.Count > 0;
        }
        public void ExecuteSelectReverse()
        {
            foreach (Track track in GlobalContext.TracklistTracks)
            {
                if (track.IsSelectedInGUI) track.IsSelectedInGUI = false;
                else track.IsSelectedInGUI = true;
            }
        }

        public bool CanExecuteManageSources
        {
            get => true;
        }
        public void ExecuteManageSources()
        {
            Dialogs.ManageTracklistSources mts = new Dialogs.ManageTracklistSources();
            //mts.Owner = this; //enables center-screen display  //TODO
            mts.ShowDialog();
        }

        public bool CanExecuteUpdateTags
        {
            get => true;
        }
        public async void ExecuteUpdateTags()
        {
            foreach (Track track in GlobalContext.TracklistTracks)
            {
                if (track.IsSelectedInGUI)
                {
                    //track.SetMetaDataFromActiveCandidate();
                    string trial_ID = await fingerprintService.GetIDByFingerprint(track.AcoustID, todo_duration);
                    track.MetaData.Copyright = trial_ID;
                }
            }
        }

        public bool CanExecuteGetFingerprint
        {
            get => true;
        }
        public void ExecuteGetFingerprint()
        {
            //SetProgressBarValue(25, "Generating fingerprints...");  //TODO
            foreach (Track track in GlobalContext.TracklistTracks)
            {
                if (track.IsSelectedInGUI)
                {
                    fingerprintService.GetFingerprint(track.FileHandle.Name, new Services.AcoustIDService.FingerPrintCallback(callback_fv));
                }
            }
        }

        private void callback_fv(string path, string fingerprint, int duration, Services.NAudioDecoder decoder)
        {
            //SetProgressBarValue(75, "Generating fingerprints for" + path); //TODO
            foreach (Track track in GlobalContext.TracklistTracks)
            {
                if (track.FileHandle.Name == path)
                {
                    track.AcoustID = fingerprint;
                    todo_duration = duration;
                    //string trial_ID = GlobalContext.AcoustIDProvider.GetIDByFingerprint(fingerprint, duration);
                    //TODO: duration and other non-editable metadata
                    break; //1 match
                }
            }
            //SetProgressBarValue(0, " ");  //TODO
            decoder.Dispose();
        }

        public bool CanExecuteSearchMusicBrainz
        {
            get => true;
        }
        public async void ExecuteSearchMusicBrainz()
        {
            foreach (Track track in GlobalContext.TracklistTracks)
            {
                if (track.IsSelectedInGUI)
                {
                    List<Track> results = new List<Track>();
                    //SetProgressBarValue(25, "Querying MusicBrainz API for " + GlobalContext.FileService.GetFileNameFromFilePath(track.FileHandle.Name));  //TODO
                    results = await GetMatchesForTrack(track);
                    MatchList = new ObservableCollection<Track>(results);
                    //SetProgressBarValue(75, "Querying MusicBrainz API " + GlobalContext.FileService.GetFileNameFromFilePath(track.FileHandle.Name));  //TODO
                    System.Threading.Thread.Sleep(100);
                    //SetProgressBarValue(0, " ");  //TODO

                    if (AutoSelect && results.Count > 0)
                    {
                        //TODO: implement magic
                        //_id = GlobalContext.AcoustIDProvider.GetIDByFingerprint(_fingerprint, _duration).Result;
                        track.AddCandidateTrack(results[0]);
                        track.SelectActiveCandidate(results[0].MetaData.MusicBrainzTrackId);
                    }
                }
                NotifyPropertyChanged(nameof(MatchList));
            }
        }



        private async Task<List<Track>> GetMatchesForTrack(Track track)
        {
            List<Track> matches = new List<Track>();

            if (!String.IsNullOrEmpty(track.MetaData.MusicBrainzTrackId)) //track has an MBID, we need a lookup
            {
                Track result = await GetMatchByMBID(track.MetaData.MusicBrainzTrackId);
                matches.Add(result);
            }
            else if (!String.IsNullOrEmpty(track.MetaData.Title)) //we don't have MBID, we need a search by some metadata
            {
                matches = await GetMatchesByMetaData(track.MetaData.Title,
                    track.MetaData.JoinedAlbumArtists.Split(';').First(), //can be null
                    track.MetaData.Album); //can be null
            }
            else if (!String.IsNullOrEmpty(track.FileHandle.Name)) //we don't have MBID, nor a title, but we can try some magic from the file name
            {
                string trackName = fileService.GetFileNameFromFilePath(track.FileHandle.Name);
                if (trackName.Contains("-"))
                {
                    string[] splitted = trackName.Split('-');
                    if (splitted.Length == 2)
                    {
                        if (splitted[0].Length > 0 && splitted[1].Length > 0)
                        {
                            string supposedArtist = splitted[0].Trim();
                            string supposedTitle = splitted[1].Trim();
                            matches = await GetMatchesByMetaData(supposedTitle, supposedArtist);
                        }
                    }
                }
            }
            else
            {
                Dialogs.ExceptionNotification en = new Dialogs.ExceptionNotification(
                    "Track matching error",
                    "Cannot query this track against MusicBrainz, since it has no relevant metadata.",
                    "Try to use fingerprinting!");
                en.ShowDialog();
            }

            return matches;
        }
        private async Task<Track> GetMatchByMBID(string MBID)
        {
            AudioMetaData result = await metadataService.GetRecordingByMBID(MBID);

            return new Track(result);
        }
        private async Task<List<Track>> GetMatchesByMetaData(string title, string artist = null, string album = null)
        {
            List<AudioMetaData> results = await metadataService.GetRecordingsByMetaData(title, artist, album);

            List<Track> toReturn = new List<Track>();

            foreach (AudioMetaData result in results)
            {
                toReturn.Add(new Track(result));
            }

            return toReturn;
        }
    }
}