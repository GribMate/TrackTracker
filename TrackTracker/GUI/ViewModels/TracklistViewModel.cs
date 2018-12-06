using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TrackTracker.BLL;
using TrackTracker.BLL.GlobalContexts;
using TrackTracker.BLL.Model;
using TrackTracker.GUI.ViewModels.Base;
using TrackTracker.Services.Interfaces;



namespace TrackTracker.GUI.ViewModels
{
    public class TracklistViewModel : ViewModelBase
    {
        private IFileService fileService;
        private ITaggingService taggingService;
        private IMetadataService metadataService;
        private IFingerprintService fingerprintService;

        public ObservableCollection<MetaTagDisplayable> TagList { get; set; }
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
            taggingService = DependencyInjector.GetService<ITaggingService>();
            metadataService = DependencyInjector.GetService<IMetadataService>();
            fingerprintService = DependencyInjector.GetService<IFingerprintService>();

            TagList = new ObservableCollection<MetaTagDisplayable>();
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

                if (SelectedTrack != null)
                {
                    MatchList = new ObservableCollection<TrackVirtual>(selectedTrack.MatchCandidates);

                    RefreshTagList(false);

                    NotifyPropertyChanged(nameof(MatchList));
                }
            }
        }

        private TrackVirtual selectedMatch;
        public TrackVirtual SelectedMatch
        {
            get => selectedMatch;
            set
            {
                SetProperty(ref selectedMatch, value);

                if (SelectedMatch != null && SelectedTrack != null && AutoSelect == false)
                {
                    SelectedTrack.ActiveCandidateMBTrackID = value.MetaData.MusicBrainzTrackId;

                    RefreshTagList(true);
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
            get => LMPContext.StoredLocalMediaPacks.Count > 0 || LMPContext.ActiveLocalMediaPacks.Count > 0;
        }
        public void ExecuteManageSources()
        {
            Views.Dialogs.ManageTracklistSources mts = new Views.Dialogs.ManageTracklistSources();
            mts.Owner = System.Windows.Application.Current.MainWindow;
            mts.ShowDialog();
        }

        public bool CanExecuteUpdateTags
        {
            get => TracklistContext.TracklistTracks.Count > 0;
        }
        public void ExecuteUpdateTags()
        {
            foreach (TrackLocal track in TracklistContext.TracklistTracks)
            {
                if (track.IsSelected)
                {
                    if (track.MatchCandidates.Count > 0 && track.ActiveCandidateMBTrackID.Value.HasValue)
                    {

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

                        taggingService.Save(track.MusicFileProperties.Path, track.MetaData.GetAllMetaTagsDataNative()); // Persisting to local file metadata
                    }
                    else
                    {
                        UtilityHelper.ShowExceptionDialog(
                            "No matches for selected track",
                            $"There are no possible matches for selected track (path: {track.MusicFileProperties.Path}).",
                            "Please unselect this track or perform another search on it!",
                            System.Windows.Application.Current.MainWindow);
                    }
                }
            }
        }

        public bool CanExecuteGetFingerprint
        {
            get => TracklistContext.TracklistTracks.Count > 0;
        }
        public async void ExecuteGetFingerprint()
        {
            //SetProgressBarValue(25, "Generating fingerprints...");  //TODO
            foreach (TrackLocal track in TracklistContext.TracklistTracks)
            {
                if (track.IsSelected)
                {
                    await fingerprintService.RunFingerprinting(track.MusicFileProperties.Path);

                    Dictionary<string, object> results = fingerprintService.GetDataOfLastRun();
                    UtilityHelper.RegisterFingerprintData(track, results);



                    Dictionary<string, Guid> IDs = await fingerprintService.GetIDsByFingerprint(track.MusicFileProperties.Fingerprint, track.MusicFileProperties.Duration);

                    Guid musicBrainzTrackId = IDs.Select(id => id).Where(id => id.Key.Equals("MusicBrainzTrackId")).First().Value;
                    Guid acoustID = IDs.Select(id => id).Where(id => id.Key.Equals("AcoustID")).First().Value;

                    track.MetaData.MusicBrainzTrackId.Value = musicBrainzTrackId;
                    track.MetaData.AcoustID.Value = acoustID;

                    taggingService.Save(track.MusicFileProperties.Path, track.MetaData.GetAllMetaTagsDataNative()); // Persisting to local file metadata
                }
            }
        }

        public bool CanExecuteSearchMusicBrainz
        {
            get => TracklistContext.TracklistTracks.Count > 0;
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
                    track.MatchCandidates.AddRange(results);
                    //SetProgressBarValue(75, "Querying MusicBrainz API for " + GlobalContext.FileService.GetFileNameFromFilePath(track.FileHandle.Name));  //TODO
                    //System.Threading.Thread.Sleep(100);
                    //SetProgressBarValue(0, " ");  //TODO

                    if (AutoSelect && results.Count > 0)
                    {
                        int maxPoints = 0;
                        TrackVirtual selected = null;

                        foreach (TrackVirtual result in results)
                        {
                            int currentPoints = 0;

                            currentPoints += result.MetaData.GetAllMetaTagsDataNativeNonEmpty().Count;

                            if (result.MetaData.MusicBrainzReleaseCountry.Value != null)
                            {
                                if (result.MetaData.MusicBrainzReleaseCountry.Value.Equals("XW"))
                                    currentPoints += 10;
                                else if (result.MetaData.MusicBrainzReleaseCountry.Value.Equals("XE"))
                                    currentPoints += 5;
                            }

                            if (result.MetaData.MusicBrainzReleaseStatus.Value != null)
                            {
                                if (result.MetaData.MusicBrainzReleaseStatus.Value.Equals("Official"))
                                    currentPoints += 3;
                            }

                            if (currentPoints > maxPoints)
                            {
                                maxPoints = currentPoints;
                                selected = result;
                            }
                        }
                        track.ActiveCandidateMBTrackID = new MetaTagGUID("MusicBrainzTrackId", selected.MetaData.MusicBrainzTrackId.Value);
                    }
                }
            }
        }



        private void RefreshTagList(bool isFromMatchSelect)
        {
            List<MetaTagDisplayable> tagList = new List<MetaTagDisplayable>();

            if (isFromMatchSelect) // We know that a track and a match are both selected and AutoSelect = false
            {
                List<MetaTagBase> newTags = SelectedMatch.MetaData.GetAllMetaTags();

                foreach (MetaTagBase oldTag in SelectedTrack.MetaData.GetAllMetaTags())
                {
                    MetaTagDisplayable displayable = GetDisplayableMetaTag(oldTag);

                    string matchValue = newTags.Select(matchTag => matchTag).Where(matchTag => matchTag.Key.Equals(oldTag.Key)).First().ToString();
                    displayable.NewValue = (matchValue == null) ? oldTag.ToString() : matchValue.ToString();

                    tagList.Add(displayable);
                }
            }
            else // A track is selected, but a match might not be and the state of AutoSelect is unknown
            {
                if (SelectedTrack.MatchCandidates.Count == 0) // The track has no matches yet, we only want to display it's own tags
                {
                    foreach (MetaTagBase oldTag in SelectedTrack.MetaData.GetAllMetaTags())
                    {
                        MetaTagDisplayable displayable = GetDisplayableMetaTag(oldTag);
                        displayable.NewValue = " - (no matches found yet)";

                        tagList.Add(displayable);
                    }
                }
                else if (SelectedTrack.ActiveCandidateMBTrackID != null) // The selected track has some matches available and AutoSelect = true
                {
                    TrackVirtual activeMatch = null;
                    foreach (TrackVirtual candidate in SelectedTrack.MatchCandidates)
                    {
                        if (candidate.MetaData.MusicBrainzTrackId.Value == SelectedTrack.ActiveCandidateMBTrackID.Value)
                        {
                            activeMatch = candidate;
                            break;
                        }
                    }

                    List<MetaTagBase> newTags = activeMatch.MetaData.GetAllMetaTags();

                    foreach (MetaTagBase oldTag in SelectedTrack.MetaData.GetAllMetaTags())
                    {
                        MetaTagDisplayable displayable = GetDisplayableMetaTag(oldTag);

                        string matchValue = newTags.Select(matchTag => matchTag).Where(matchTag => matchTag.Key.Equals(oldTag.Key)).First().ToString();
                        displayable.NewValue = (matchValue == null) ? oldTag.ToString() : matchValue.ToString();

                        tagList.Add(displayable);
                    }
                }
                else // The selected track has some matches available, but AutoSelect = false, so we need to tell the user to select a match as well
                {
                    foreach (MetaTagBase oldTag in SelectedTrack.MetaData.GetAllMetaTags())
                    {
                        MetaTagDisplayable displayable = GetDisplayableMetaTag(oldTag);
                        displayable.NewValue = "Please select a match from the list!";

                        tagList.Add(displayable);
                    }
                }
            }

            TagList = new ObservableCollection<MetaTagDisplayable>(tagList);
            NotifyPropertyChanged(nameof(TagList));
        }

        private MetaTagDisplayable GetDisplayableMetaTag(MetaTagBase source)
        {
            MetaTagDisplayable displayable = new MetaTagDisplayable()
            {
                TagName = source.Key,
                CurrentValue = source.ToString()
            };

            return displayable;
        }

        private async Task<List<TrackVirtual>> GetMatchesForTrack(TrackLocal track)
        {
            List<TrackVirtual> matches = new List<TrackVirtual>();

            if (track.MetaData.MusicBrainzTrackId.Value.HasValue) // Track has an MBID, we need a lookup
            {
                TrackVirtual result = await GetMatchByMBID(track.MetaData.MusicBrainzTrackId.Value.Value);
                matches.Add(result);
            }
            else if (track.MusicFileProperties.IsFingerprinted) // Track is fingerprinted already, we can get an MBID
            {
                TrackVirtual result = await GetMatchByFingerprint(track.MusicFileProperties.Fingerprint, track.MusicFileProperties.Duration);
                matches.Add(result);
            }
            else if (!String.IsNullOrEmpty(track.MetaData.Title.Value)) // We don't have MBID, we need a search by some metadata
            {
                matches = await GetMatchesByMetaData(track.MetaData.Title.Value, // We require at least a title for accurate-enough queries
                    track.MetaData.AlbumArtists.Value.FirstOrDefault(), // Can be null
                    track.MetaData.Album.Value); // Can be null
            }
            else if (!String.IsNullOrEmpty(track.MusicFileProperties.FileName)) // We don't have MBID, nor a title, but we can try some magic from the file name
            {
                if (track.MusicFileProperties.FileName.Contains("-"))
                {
                    string[] splitted = track.MusicFileProperties.FileName.Split('-');
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

            MetaData tags = UtilityHelper.FormatMetaData(result);

            return new TrackVirtual(tags, true);
        }
        private async Task<TrackVirtual> GetMatchByFingerprint(string fingerprint, int duration)
        {
            Dictionary<string, Guid> IDs = await fingerprintService.GetIDsByFingerprint(fingerprint, duration);

            Guid musicBrainzTrackId = IDs.Select(id => id).Where(id => id.Key.Equals("MusicBrainzTrackId")).First().Value;
            Guid acoustID = IDs.Select(id => id).Where(id => id.Key.Equals("AcoustID")).First().Value;

            TrackVirtual result = await GetMatchByMBID(musicBrainzTrackId);

            result.MetaData.AcoustID.Value = acoustID; // Adding manually

            return result;
        }
        private async Task<List<TrackVirtual>> GetMatchesByMetaData(string title, string artist = null, string album = null)
        {
            List<Dictionary<string, object>> results = await metadataService.GetRecordingsByMetaData(title, artist, album);

            List<TrackVirtual> toReturn = new List<TrackVirtual>();

            foreach (Dictionary<string, object> result in results)
            {
                MetaData resultTags = UtilityHelper.FormatMetaData(result);
                toReturn.Add(new TrackVirtual(resultTags, true));
            }

            return toReturn;
        }
    }
}