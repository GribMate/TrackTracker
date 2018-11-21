using System;
using System.Collections.Generic;
using System.ComponentModel;



namespace TrackTracker.BLL
{
    /*
    Class: Track
    State: Under construction | DEBUG
    Description:
        Represents a music track, that has various attributes.
        Also can link to a physical file, so that file operations can be done if possible. (wraps around TagLib.File class).
    */
    public class Track : INotifyPropertyChanged
    {
        private bool isSelectedInGUI;
        private bool isOfflineAccessible;
        private List<Track> metaDataCandidates;
        private int activeCandidate;

        private string acoustID; //TODO: rework storing

        public bool IsSelectedInGUI //true if the file is selected via Tracklist GUI
        {
            get => isSelectedInGUI;
            set
            {
                if (isSelectedInGUI != value)
                {
                    isSelectedInGUI = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelectedInGUI)));
                }
            }
        }
        public bool IsOfflineAccessible //true if file is downloaded on local machine or false if it's a virtual track
        {
            get => isOfflineAccessible;
            set
            {
                if (isOfflineAccessible != value)
                {
                    isOfflineAccessible = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsOfflineAccessible)));
                }
            }
        }
        public TagLib.File FileHandle //offline stored, physically accessible file handle, or null
        {
            get; //don't support on-the-fly changes
        }
        public AudioMetaData MetaData //ID3 tags of the track
        {
            get; //don't support on-the-fly changes
        }
        public string AcoustID
        {
            get => acoustID;
            set
            {
                if (acoustID != value)
                {
                    acoustID = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AcoustID)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged; //Tracks are 1-1 represented in GUI

        public Track(TagLib.File fileHandle) //FOR ACTUAL MUSIC FILES THAT ARE STORED LOCALLY
        {
            IsSelectedInGUI = false;
            IsOfflineAccessible = true; //beacuse we have a non null file handle
            this.FileHandle = fileHandle;
            this.MetaData = new AudioMetaData(fileHandle);
            this.metaDataCandidates = new List<Track>();
            this.activeCandidate = -1;
        }
        public Track(AudioMetaData metaData) //FOR VIRTUAL TRACKS, CONSISTING ONLY OF METADATA
        {
            IsSelectedInGUI = false;
            IsOfflineAccessible = false; //beacuse we do not have a file handle
            this.FileHandle = null;
            this.MetaData = metaData;
            this.metaDataCandidates = null; //the current object is itself a candidate
            this.activeCandidate = -1;
        }

        public void AddCandidateTrack(Track candidate)
        {
            if (!IsOfflineAccessible || metaDataCandidates == null) throw new InvalidOperationException();
            if (candidate == null) throw new ArgumentNullException();

            metaDataCandidates.Add(candidate);
        }
        public bool SelectActiveCandidate(string MBID_Track)
        {
            if (!IsOfflineAccessible || metaDataCandidates == null) throw new InvalidOperationException();
            if (MBID_Track == null || MBID_Track.Length != 36) throw new ArgumentException(); //MBIDs are exactly 36 characters long

            int index = metaDataCandidates.FindIndex(c => c.MetaData.MusicBrainzTrackId == MBID_Track);
            if (index == -1) return false;
            else
            {
                activeCandidate = index;
                return true;
            }
        }
        public bool SetMetaDataFromActiveCandidate()
        {
            if (!IsOfflineAccessible || metaDataCandidates.Count < 1 || activeCandidate == -1 ||
                FileHandle == null) throw new InvalidOperationException();

            if (activeCandidate < 0 || activeCandidate >= metaDataCandidates.Count) return false;
            else
            {
                MetaData.Title = metaDataCandidates[activeCandidate].MetaData.Title;
                MetaData.Album = metaDataCandidates[activeCandidate].MetaData.Album;
                MetaData.JoinedAlbumArtists = metaDataCandidates[activeCandidate].MetaData.JoinedAlbumArtists;
                MetaData.JoinedAlbumArtistsSort = metaDataCandidates[activeCandidate].MetaData.JoinedAlbumArtistsSort;
                MetaData.JoinedGenres = metaDataCandidates[activeCandidate].MetaData.JoinedGenres;
                MetaData.BeatsPerMinute = metaDataCandidates[activeCandidate].MetaData.BeatsPerMinute;
                MetaData.Copyright = metaDataCandidates[activeCandidate].MetaData.Copyright;
                MetaData.Year = metaDataCandidates[activeCandidate].MetaData.Year;
                MetaData.Track = metaDataCandidates[activeCandidate].MetaData.Track;
                MetaData.TrackCount = metaDataCandidates[activeCandidate].MetaData.TrackCount;
                MetaData.Disc = metaDataCandidates[activeCandidate].MetaData.Disc;
                MetaData.DiscCount = metaDataCandidates[activeCandidate].MetaData.DiscCount;
                MetaData.MusicBrainzReleaseArtistId = metaDataCandidates[activeCandidate].MetaData.MusicBrainzReleaseArtistId;
                MetaData.MusicBrainzTrackId = metaDataCandidates[activeCandidate].MetaData.MusicBrainzTrackId;
                MetaData.MusicBrainzDiscId = metaDataCandidates[activeCandidate].MetaData.MusicBrainzDiscId;
                MetaData.MusicBrainzReleaseStatus = metaDataCandidates[activeCandidate].MetaData.MusicBrainzReleaseStatus;
                MetaData.MusicBrainzReleaseType = metaDataCandidates[activeCandidate].MetaData.MusicBrainzReleaseType;
                MetaData.MusicBrainzReleaseCountry = metaDataCandidates[activeCandidate].MetaData.MusicBrainzReleaseCountry;
                MetaData.MusicBrainzReleaseId = metaDataCandidates[activeCandidate].MetaData.MusicBrainzReleaseId;
                MetaData.MusicBrainzArtistId = metaDataCandidates[activeCandidate].MetaData.MusicBrainzArtistId;

                return true;
            }
        }
        public List<MetaTag> GetTags()
        {
            if (!IsOfflineAccessible) throw new InvalidOperationException();

            List<MetaTag> tags = new List<MetaTag>();

            tags.Add(new MetaTag("AcoustID", acoustID ?? "", acoustID ?? "", this)); //TODO: rework storing

            if (activeCandidate != -1 && metaDataCandidates.Count > 0)
            {
                tags.Add(new MetaTag("Title",                           MetaData.Title ?? "",                         metaDataCandidates[activeCandidate].MetaData.Title ?? "", this));
                tags.Add(new MetaTag("Album",                           MetaData.Album ?? "",                         metaDataCandidates[activeCandidate].MetaData.Album ?? "", this));
                tags.Add(new MetaTag("Album artists",                   MetaData.JoinedAlbumArtists ?? "",            metaDataCandidates[activeCandidate].MetaData.JoinedAlbumArtists ?? "", this));
                tags.Add(new MetaTag("Album artists sort order",        MetaData.JoinedAlbumArtistsSort ?? "",        metaDataCandidates[activeCandidate].MetaData.JoinedAlbumArtistsSort ?? "", this));
                tags.Add(new MetaTag("Genres",                          MetaData.JoinedGenres ?? "",                  metaDataCandidates[activeCandidate].MetaData.JoinedGenres ?? "", this));
                tags.Add(new MetaTag("Beats per Minute",                MetaData.BeatsPerMinute.ToString() ?? "",     metaDataCandidates[activeCandidate].MetaData.BeatsPerMinute.ToString() ?? "", this));
                tags.Add(new MetaTag("Copyright",                       MetaData.Copyright ?? "",                     metaDataCandidates[activeCandidate].MetaData.Copyright ?? "", this));
                tags.Add(new MetaTag("Year",                            MetaData.Year.ToString() ?? "",               metaDataCandidates[activeCandidate].MetaData.Year.ToString() ?? "", this));
                tags.Add(new MetaTag("Track",                           MetaData.Track.ToString() ?? "",              metaDataCandidates[activeCandidate].MetaData.Track.ToString() ?? "", this));
                tags.Add(new MetaTag("Album track count",               MetaData.TrackCount.ToString() ?? "",         metaDataCandidates[activeCandidate].MetaData.TrackCount.ToString() ?? "", this));
                tags.Add(new MetaTag("Disc",                            MetaData.Disc.ToString() ?? "",               metaDataCandidates[activeCandidate].MetaData.Disc.ToString() ?? "", this));
                tags.Add(new MetaTag("Album disc count",                MetaData.DiscCount.ToString() ?? "",          metaDataCandidates[activeCandidate].MetaData.DiscCount.ToString() ?? "", this));
                tags.Add(new MetaTag("MusicBrainz Release Artist ID",   MetaData.MusicBrainzReleaseArtistId ?? "",    metaDataCandidates[activeCandidate].MetaData.MusicBrainzReleaseArtistId ?? "", this));
                tags.Add(new MetaTag("MusicBrainz Track ID",            MetaData.MusicBrainzTrackId ?? "",            metaDataCandidates[activeCandidate].MetaData.MusicBrainzTrackId ?? "", this));
                tags.Add(new MetaTag("MusicBrainz Disc ID",             MetaData.MusicBrainzDiscId ?? "",             metaDataCandidates[activeCandidate].MetaData.MusicBrainzDiscId ?? "", this));
                tags.Add(new MetaTag("MusicBrainz Release Status",      MetaData.MusicBrainzReleaseStatus ?? "",      metaDataCandidates[activeCandidate].MetaData.MusicBrainzReleaseStatus ?? "", this));
                tags.Add(new MetaTag("MusicBrainz Release Type",        MetaData.MusicBrainzReleaseType ?? "",        metaDataCandidates[activeCandidate].MetaData.MusicBrainzReleaseType ?? "", this));
                tags.Add(new MetaTag("MusicBrainz Release Country",     MetaData.MusicBrainzReleaseCountry ?? "",     metaDataCandidates[activeCandidate].MetaData.MusicBrainzReleaseCountry ?? "", this));
                tags.Add(new MetaTag("MusicBrainz Release ID",          MetaData.MusicBrainzReleaseId ?? "",          metaDataCandidates[activeCandidate].MetaData.MusicBrainzReleaseId ?? "", this));
                tags.Add(new MetaTag("MusicBrainz Artist ID",           MetaData.MusicBrainzArtistId ?? "",           metaDataCandidates[activeCandidate].MetaData.MusicBrainzArtistId ?? "", this));
            }
            else
            {
                tags.Add(new MetaTag("Title",                           MetaData.Title ?? "",                         "", this));
                tags.Add(new MetaTag("Album",                           MetaData.Album ?? "",                         "", this));
                tags.Add(new MetaTag("Album artists",                   MetaData.JoinedAlbumArtists ?? "",            "", this));
                tags.Add(new MetaTag("Album artists sort order",        MetaData.JoinedAlbumArtistsSort ?? "",        "", this));
                tags.Add(new MetaTag("Genres",                          MetaData.JoinedGenres ?? "",                  "", this));
                tags.Add(new MetaTag("Beats per Minute",                MetaData.BeatsPerMinute.ToString() ?? "",     "", this));
                tags.Add(new MetaTag("Copyright",                       MetaData.Copyright ?? "",                     "", this));
                tags.Add(new MetaTag("Year",                            MetaData.Year.ToString() ?? "",               "", this));
                tags.Add(new MetaTag("Track",                           MetaData.Track.ToString() ?? "",              "", this));
                tags.Add(new MetaTag("Album track count",               MetaData.TrackCount.ToString() ?? "",         "", this));
                tags.Add(new MetaTag("Disc",                            MetaData.Disc.ToString() ?? "",               "", this));
                tags.Add(new MetaTag("Album disc count",                MetaData.DiscCount.ToString() ?? "",          "", this));
                tags.Add(new MetaTag("MusicBrainz Release Artist ID",   MetaData.MusicBrainzReleaseArtistId ?? "",    "", this));
                tags.Add(new MetaTag("MusicBrainz Track ID",            MetaData.MusicBrainzTrackId ?? "",            "", this));
                tags.Add(new MetaTag("MusicBrainz Disc ID",             MetaData.MusicBrainzDiscId ?? "",             "", this));
                tags.Add(new MetaTag("MusicBrainz Release Status",      MetaData.MusicBrainzReleaseStatus ?? "",      "", this));
                tags.Add(new MetaTag("MusicBrainz Release Type",        MetaData.MusicBrainzReleaseType ?? "",        "", this));
                tags.Add(new MetaTag("MusicBrainz Release Country",     MetaData.MusicBrainzReleaseCountry ?? "",     "", this));
                tags.Add(new MetaTag("MusicBrainz Release ID",          MetaData.MusicBrainzReleaseId ?? "",          "", this));
                tags.Add(new MetaTag("MusicBrainz Artist ID",           MetaData.MusicBrainzArtistId ?? "",           "", this));
            }

            return tags;
        }
    }
}