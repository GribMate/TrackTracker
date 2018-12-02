using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

using TrackTracker.Services.Interfaces;
using TrackTracker.BLL;
using TrackTracker.BLL.Model;
using TrackTracker.BLL.GlobalContexts;
using TrackTracker.GUI.ViewModels.Base;



namespace TrackTracker.GUI.ViewModels
{
    public class TracklistViewModel : ViewModelBase
    {
        private IFileService fileService;
        private IMetadataService metadataService;
        private IFingerprintService fingerprintService;

        public ObservableCollection<MetaTagBase> TagList { get; set; }
        public ObservableCollection<TrackVirtual> MatchList { get; set; }

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

            TagList = new ObservableCollection<MetaTagBase>();
            MatchList = new ObservableCollection<TrackVirtual>();

            SelectAllCommand = new RelayCommand<object>(exe => ExecuteSelectAll(), can => CanExecuteSelectAll);
            SelectReverseCommand = new RelayCommand<object>(exe => ExecuteSelectReverse(), can => CanExecuteSelectReverse);
            ManageSourcesCommand = new RelayCommand<object>(exe => ExecuteManageSources(), can => CanExecuteManageSources);
            UpdateTagsCommand = new RelayCommand<object>(exe => ExecuteUpdateTags(), can => CanExecuteUpdateTags);
            GetFingerprintCommand = new RelayCommand<object>(exe => ExecuteGetFingerprint(), can => CanExecuteGetFingerprint);
            SearchMusicBrainzCommand = new RelayCommand<object>(exe => ExecuteSearchMusicBrainz(), can => CanExecuteSearchMusicBrainz);
        }



        private TrackLocal selectedTrack;
        public TrackLocal SelectedTrack
        {
            get => selectedTrack;
            set
            {
                SetProperty(ref selectedTrack, value);
                TagList = new ObservableCollection<MetaTagBase>(value.MetaData.GetAllMetaTags());
                NotifyPropertyChanged(nameof(TagList));
            }
        }

        private TrackVirtual selectedMatch;
        public TrackVirtual SelectedMatch
        {
            get => selectedMatch;
            set
            {
                SetProperty(ref selectedMatch, value);

                if (SelectedTrack != null)
                {
                    SelectedTrack.MatchCandidates.Add(value);
                    SelectedTrack.ActiveCandidateMBTrackID = value.MetaData.MusicBrainzTrackId;
                }
            }
        }

        private bool autoSelect;
        public bool AutoSelect
        {
            get => autoSelect;
            set { SetProperty(ref autoSelect, value); }
        }




        private bool CanExecuteSelectAll
        {
            get => TracklistContext.TracklistTracks.Count > 0;
        }
        private void ExecuteSelectAll()
        {
            foreach (TrackLocal track in TracklistContext.TracklistTracks)
            {
                track.IsSelected = true;
            }
        }

        public bool CanExecuteSelectReverse
        {
            get => TracklistContext.TracklistTracks.Count > 0;
        }
        public void ExecuteSelectReverse()
        {
            foreach (TrackLocal track in TracklistContext.TracklistTracks)
            {
                if (track.IsSelected)
                    track.IsSelected = false;
                else
                    track.IsSelected = true;
            }
        }

        public bool CanExecuteManageSources
        {
            get => true;
        }
        public void ExecuteManageSources()
        {
            Views.Dialogs.ManageTracklistSources mts = new Views.Dialogs.ManageTracklistSources();
            //mts.Owner = this; //enables center-screen display  //TODO
            mts.ShowDialog();
        }

        public bool CanExecuteUpdateTags
        {
            get => true;
        }
        public async void ExecuteUpdateTags()
        {
            foreach (TrackLocal track in TracklistContext.TracklistTracks)
            {
                if (track.IsSelected)
                {
                    if (track.MatchCandidates.Count < 1 || track.ActiveCandidateMBTrackID.Value.HasValue == false)
                        throw new InvalidOperationException($"Cannot set new metatag data for file {track.MusicFileProperties.FileName}, because of an internal error.");

                    TrackVirtual sourceTrack = null;
                    foreach (TrackVirtual trackCandidate in track.MatchCandidates)
                    {
                        if (trackCandidate.MetaData.MusicBrainzTrackId.Value.Value == track.ActiveCandidateMBTrackID.Value.Value)
                        {
                            sourceTrack = trackCandidate;
                            break;
                        }
                    }

                    track.MetaData.Title = sourceTrack.MetaData.Title;
                    track.MetaData.Album = sourceTrack.MetaData.Album;
                    track.MetaData.AlbumArtists = sourceTrack.MetaData.AlbumArtists;
                    track.MetaData.AlbumArtistsSort = sourceTrack.MetaData.AlbumArtistsSort;
                    track.MetaData.Genres = sourceTrack.MetaData.Genres;
                    track.MetaData.BeatsPerMinute = sourceTrack.MetaData.BeatsPerMinute;
                    track.MetaData.Copyright = sourceTrack.MetaData.Copyright;
                    track.MetaData.Year = sourceTrack.MetaData.Year;
                    track.MetaData.Track = sourceTrack.MetaData.Track;
                    track.MetaData.TrackCount = sourceTrack.MetaData.TrackCount;
                    track.MetaData.Disc = sourceTrack.MetaData.Disc;
                    track.MetaData.DiscCount = sourceTrack.MetaData.DiscCount;
                    track.MetaData.AcoustID = sourceTrack.MetaData.AcoustID;
                    track.MetaData.MusicBrainzReleaseArtistId = sourceTrack.MetaData.MusicBrainzReleaseArtistId;
                    track.MetaData.MusicBrainzTrackId = sourceTrack.MetaData.MusicBrainzTrackId;
                    track.MetaData.MusicBrainzDiscId = sourceTrack.MetaData.MusicBrainzDiscId;
                    track.MetaData.MusicBrainzReleaseStatus = sourceTrack.MetaData.MusicBrainzReleaseStatus;
                    track.MetaData.MusicBrainzReleaseType = sourceTrack.MetaData.MusicBrainzReleaseType;
                    track.MetaData.MusicBrainzReleaseCountry = sourceTrack.MetaData.MusicBrainzReleaseCountry;
                    track.MetaData.MusicBrainzReleaseId = sourceTrack.MetaData.MusicBrainzReleaseId;
                    track.MetaData.MusicBrainzArtistId = sourceTrack.MetaData.MusicBrainzArtistId;

                    // TODO: TODO
                    string trial_ID = await fingerprintService.GetIDByFingerprint(track.MusicFileProperties.Fingerprint, track.MusicFileProperties.Duration);
                    track.MetaData.AcoustID.Value = new Guid(trial_ID);
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
            foreach (TrackLocal track in TracklistContext.TracklistTracks)
            {
                if (track.IsSelected)
                {
                    fingerprintService.GetFingerprint(track.MusicFileProperties.Path, new Services.AcoustIDService.FingerPrintCallback(callback_fv));
                }
            }
        }

        private void callback_fv(string path, string fingerprint, int duration, Services.NAudioDecoder decoder)
        {
            //SetProgressBarValue(75, "Generating fingerprints for" + path); //TODO
            foreach (TrackLocal track in TracklistContext.TracklistTracks)
            {
                if (track.MusicFileProperties.Path == path)
                {
                    track.MusicFileProperties.Fingerprint = fingerprint;
                    track.MusicFileProperties.Duration = duration;
                    //string trial_ID = GlobalContext.AcoustIDProvider.GetIDByFingerprint(fingerprint, duration);
                    //TODO: other non-editable metadata
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
            foreach (TrackLocal track in TracklistContext.TracklistTracks)
            {
                if (track.IsSelected)
                {
                    List<TrackVirtual> results = new List<TrackVirtual>();
                    //SetProgressBarValue(25, "Querying MusicBrainz API for " + GlobalContext.FileService.GetFileNameFromFilePath(track.FileHandle.Name));  //TODO
                    results = await GetMatchesForTrack(track);
                    MatchList = new ObservableCollection<TrackVirtual>(results);
                    //SetProgressBarValue(75, "Querying MusicBrainz API " + GlobalContext.FileService.GetFileNameFromFilePath(track.FileHandle.Name));  //TODO
                    System.Threading.Thread.Sleep(100);
                    //SetProgressBarValue(0, " ");  //TODO

                    if (AutoSelect && results.Count > 0)
                    {
                        //TODO: implement magic
                        //_id = GlobalContext.AcoustIDProvider.GetIDByFingerprint(_fingerprint, _duration).Result;
                        //track.AddCandidateTrack(results[0]);
                        //track.SelectActiveCandidate(results[0].MetaData.MusicBrainzTrackId);
                    }
                }
                NotifyPropertyChanged(nameof(MatchList));
            }
        }



        private async Task<List<TrackVirtual>> GetMatchesForTrack(TrackLocal track)
        {
            List<TrackVirtual> matches = new List<TrackVirtual>();

            if (track.MetaData.MusicBrainzTrackId.Value.HasValue) // Track has an MBID, we need a lookup
            {
                TrackVirtual result = await GetMatchByMBID(track.MetaData.MusicBrainzTrackId.Value.Value);
                matches.Add(result);
            }
            else if (!String.IsNullOrEmpty(track.MetaData.Title.Value)) //we don't have MBID, we need a search by some metadata
            {
                matches = await GetMatchesByMetaData(track.MetaData.Title.Value,
                    track.MetaData.AlbumArtists.JoinedValue.Split(';').First(), // Can be null
                    track.MetaData.Album.Value); // Can be null
            }
            else if (!String.IsNullOrEmpty(track.MusicFileProperties.FileName)) // We don't have MBID, nor a title, but we can try some magic from the file name
            {
                string trackName = fileService.GetFileNameFromFilePath(track.MusicFileProperties.FileName);
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
                UtilityHelper.ShowExceptionDialog(
                   "Track matching error",
                    "Cannot query this track against MusicBrainz, since it has no relevant metadata.",
                    "Try to use fingerprinting!");
            }

            return matches;
        }
        private async Task<TrackVirtual> GetMatchByMBID(Guid MBID)
        {
            Dictionary<string, object> result = await metadataService.GetRecordingByMBID(MBID);

            //return new Track(result);
            return new TrackVirtual(new MetaData(), true);
        }
        private async Task<List<TrackVirtual>> GetMatchesByMetaData(string title, string artist = null, string album = null)
        {
            List<Dictionary<string, object>> results = await metadataService.GetRecordingsByMetaData(title, artist, album);

            List<TrackVirtual> toReturn = new List<TrackVirtual>();

            //foreach (Dictionary<string, object> result in results)
            //{
            //    toReturn.Add(new TrackLocal(result));
            //}

            return toReturn;
        }
    }
}