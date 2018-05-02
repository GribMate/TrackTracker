using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;

namespace Onlab.BLL
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
        private TagLib.File fileHandle;
        private AudioMetaData metaData;
        private List<Track> metaDataCandidates;
        private int activeCandidate;

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
            get => fileHandle; //don't support on-the-fly changes
        }
        public AudioMetaData MetaData //ID3 tags of the track
        {
            get => metaData; //don't support on-the-fly changes
        }

        public event PropertyChangedEventHandler PropertyChanged; //Tracks are 1-1 represented in GUI

        public Track(TagLib.File fileHandle) //FOR ACTUAL MUSIC FILES THAT ARE STORED LOCALLY
        {
            IsSelectedInGUI = false;
            IsOfflineAccessible = true; //beacuse we have a non null file handle
            this.fileHandle = fileHandle;
            this.metaData = new AudioMetaData(fileHandle);
            this.metaDataCandidates = new List<Track>();
            this.activeCandidate = -1;
        }
        public Track(AudioMetaData metaData) //FOR VIRTUAL TRACKS, CONSISTING ONLY OF METADATA
        {
            IsSelectedInGUI = false;
            IsOfflineAccessible = false; //beacuse we do not have a file handle
            this.fileHandle = null;
            this.metaData = metaData;
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
        public void SetMetaDataFromActiveCandidate()
        {
            if (!IsOfflineAccessible || metaDataCandidates.Count < 1 || activeCandidate == -1 ||
                fileHandle == null) throw new InvalidOperationException();
            if (activeCandidate < 0 || activeCandidate >= metaDataCandidates.Count) throw new ArgumentOutOfRangeException();

            metaData.Title = metaDataCandidates[activeCandidate].MetaData.Title;
            metaData.JoinedAlbumArtists = metaDataCandidates[activeCandidate].MetaData.JoinedAlbumArtists;
            metaData.MusicBrainzTrackId = metaDataCandidates[activeCandidate].MetaData.MusicBrainzTrackId;
        }
        public List<MetaTag> GetTags()
        {
            if (!IsOfflineAccessible) throw new InvalidOperationException();

            List<MetaTag> tags = new List<MetaTag>();
            if (activeCandidate != -1 && metaDataCandidates.Count > 0)
            {
                tags.Add(new MetaTag("Title",                           metaData.Title ?? "",                         metaDataCandidates[activeCandidate].MetaData.Title ?? ""));
                tags.Add(new MetaTag("Album",                           metaData.Album ?? "",                         metaDataCandidates[activeCandidate].MetaData.Album ?? ""));
                tags.Add(new MetaTag("Album artists",                   metaData.JoinedAlbumArtists ?? "",            metaDataCandidates[activeCandidate].MetaData.JoinedAlbumArtists ?? ""));
                tags.Add(new MetaTag("Genres",                          metaData.JoinedGenres ?? "",                  metaDataCandidates[activeCandidate].MetaData.JoinedGenres ?? ""));
                tags.Add(new MetaTag("Beats per Minute",                metaData.BeatsPerMinute.ToString() ?? "",     metaDataCandidates[activeCandidate].MetaData.BeatsPerMinute.ToString() ?? ""));
                tags.Add(new MetaTag("Copyright",                       metaData.Copyright ?? "",                     metaDataCandidates[activeCandidate].MetaData.Copyright ?? ""));
                tags.Add(new MetaTag("Year",                            metaData.Year.ToString() ?? "",               metaDataCandidates[activeCandidate].MetaData.Year.ToString() ?? ""));
                tags.Add(new MetaTag("Track",                           metaData.Track.ToString() ?? "",              metaDataCandidates[activeCandidate].MetaData.Track.ToString() ?? ""));
                tags.Add(new MetaTag("Album track count",               metaData.TrackCount.ToString() ?? "",         metaDataCandidates[activeCandidate].MetaData.TrackCount.ToString() ?? ""));
                tags.Add(new MetaTag("Disc",                            metaData.Disc.ToString() ?? "",               metaDataCandidates[activeCandidate].MetaData.Disc.ToString() ?? ""));
                tags.Add(new MetaTag("Album disc count",                metaData.DiscCount.ToString() ?? "",          metaDataCandidates[activeCandidate].MetaData.DiscCount.ToString() ?? ""));
                tags.Add(new MetaTag("MusicBrainz Release Artist ID",   metaData.MusicBrainzReleaseArtistId ?? "",    metaDataCandidates[activeCandidate].MetaData.MusicBrainzReleaseArtistId ?? ""));
                tags.Add(new MetaTag("MusicBrainz Track ID",            metaData.MusicBrainzTrackId ?? "",            metaDataCandidates[activeCandidate].MetaData.MusicBrainzTrackId ?? ""));
                tags.Add(new MetaTag("MusicBrainz Disc ID",             metaData.MusicBrainzDiscId ?? "",             metaDataCandidates[activeCandidate].MetaData.MusicBrainzDiscId ?? ""));
                tags.Add(new MetaTag("MusicBrainz Release Status",      metaData.MusicBrainzReleaseStatus ?? "",      metaDataCandidates[activeCandidate].MetaData.MusicBrainzReleaseStatus ?? ""));
                tags.Add(new MetaTag("MusicBrainz Release Type",        metaData.MusicBrainzReleaseType ?? "",        metaDataCandidates[activeCandidate].MetaData.MusicBrainzReleaseType ?? ""));
                tags.Add(new MetaTag("MusicBrainz Release Country",     metaData.MusicBrainzReleaseCountry ?? "",     metaDataCandidates[activeCandidate].MetaData.MusicBrainzReleaseCountry ?? ""));
                tags.Add(new MetaTag("MusicBrainz Release ID",          metaData.MusicBrainzReleaseId ?? "",          metaDataCandidates[activeCandidate].MetaData.MusicBrainzReleaseId ?? ""));
                tags.Add(new MetaTag("MusicBrainz Artist ID",           metaData.MusicBrainzArtistId ?? "",           metaDataCandidates[activeCandidate].MetaData.MusicBrainzArtistId ?? ""));
            }
            else
            {
                tags.Add(new MetaTag("Title",                           metaData.Title ?? "",                         ""));
                tags.Add(new MetaTag("Album",                           metaData.Album ?? "",                         ""));
                tags.Add(new MetaTag("Album artists",                   metaData.JoinedAlbumArtists ?? "",            ""));
                tags.Add(new MetaTag("Genres",                          metaData.JoinedGenres ?? "",                  ""));
                tags.Add(new MetaTag("Beats per Minute",                metaData.BeatsPerMinute.ToString() ?? "",     ""));
                tags.Add(new MetaTag("Copyright",                       metaData.Copyright ?? "",                     ""));
                tags.Add(new MetaTag("Year",                            metaData.Year.ToString() ?? "",               ""));
                tags.Add(new MetaTag("Track",                           metaData.Track.ToString() ?? "",              ""));
                tags.Add(new MetaTag("Album track count",               metaData.TrackCount.ToString() ?? "",         ""));
                tags.Add(new MetaTag("Disc",                            metaData.Disc.ToString() ?? "",               ""));
                tags.Add(new MetaTag("Album disc count",                metaData.DiscCount.ToString() ?? "",          ""));
                tags.Add(new MetaTag("MusicBrainz Release Artist ID",   metaData.MusicBrainzReleaseArtistId ?? "",    ""));
                tags.Add(new MetaTag("MusicBrainz Track ID",            metaData.MusicBrainzTrackId ?? "",            ""));
                tags.Add(new MetaTag("MusicBrainz Disc ID",             metaData.MusicBrainzDiscId ?? "",             ""));
                tags.Add(new MetaTag("MusicBrainz Release Status",      metaData.MusicBrainzReleaseStatus ?? "",      ""));
                tags.Add(new MetaTag("MusicBrainz Release Type",        metaData.MusicBrainzReleaseType ?? "",        ""));
                tags.Add(new MetaTag("MusicBrainz Release Country",     metaData.MusicBrainzReleaseCountry ?? "",     ""));
                tags.Add(new MetaTag("MusicBrainz Release ID",          metaData.MusicBrainzReleaseId ?? "",          ""));
                tags.Add(new MetaTag("MusicBrainz Artist ID",           metaData.MusicBrainzArtistId ?? "",           ""));
            }

            return tags;
        }
    }
}