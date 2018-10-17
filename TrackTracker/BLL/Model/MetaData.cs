using System;
using System.Collections.Generic;

using TrackTracker.BLL.Enums;



namespace TrackTracker.BLL.Model
{
    /*
     * Groups all currently supported metadata about a particular track (be it offline, playable or virtual).
    */
    public class MetaData
    {
        // Currently Supported ID3 tags
        // TODO: Review ID3 tags and finalize support
        #region MetaTagProperties
        
        public MetaTagString Title { get; set; } // Title of the track
        public MetaTagString Album { get; set; } // Album title of the track
        public MetaTagString Copyright { get; set; } // Copyright information about the track
        public MetaTagCollection AlbumArtists { get; set; } // All of the artist names, who contributed to the album
        public MetaTagCollection AlbumArtistsSort { get; set; } // All of the artist names in sort form, who contributed to the album
        public MusicGenres Genres { get; set; } // Multiple flag-type genres of the track
        public MetaTagNumber BeatsPerMinute { get; set; } // BPM
        public MetaTagNumber Year { get; set; } // Release year of the track (or album) TODO: further polish
        public MetaTagNumber Track { get; set; } // Number of track on the corresponding album (if any)
        public MetaTagNumber TrackCount { get; set; } // Number of total tracks on the corresponding album (if any)
        public MetaTagNumber Disc { get; set; } // Number of disc of the corresponding album (if any)
        public MetaTagNumber DiscCount { get; set; } // Number of total discs of the corresponding album (if any)

        public MetaTagGUID AcoustID { get; set; } // AID of the track
        public MetaTagGUID MusicBrainzReleaseArtistId { get; set; } // MBID of release artist
        public MetaTagGUID MusicBrainzTrackId { get; set; } // MBID of track
        public MetaTagGUID MusicBrainzDiscId { get; set; } // MBID of disc
        public MetaTagGUID MusicBrainzReleaseId { get; set; } // MBID of release
        public MetaTagGUID MusicBrainzArtistId { get; set; } // MBID of artist

        public MetaTagString MusicBrainzReleaseStatus { get; set; } // MB release status
        public MetaTagString MusicBrainzReleaseType { get; set; } // MB release type
        public MetaTagString MusicBrainzReleaseCountry { get; set; } // MB country of the release of the corresponding album

        #endregion

        public Dictionary<string, string> GetAllMetaTagsFormatted() // Returns all metadata of the track in a formatted, displayable manner
        {
            Dictionary<string, string> allTags = new Dictionary<string, string>();

            allTags.Add(nameof(Title), Title.ToString());
            allTags.Add(nameof(Album), Album.ToString());
            allTags.Add(nameof(Copyright), Copyright.ToString());
            allTags.Add(nameof(AlbumArtists), AlbumArtists.ToString());
            allTags.Add(nameof(AlbumArtistsSort), AlbumArtistsSort.ToString());
            allTags.Add(nameof(Genres), Genres.ToString());
            allTags.Add(nameof(BeatsPerMinute), BeatsPerMinute.ToString());
            allTags.Add(nameof(Year), Year.ToString());
            allTags.Add(nameof(Track), Track.ToString());
            allTags.Add(nameof(TrackCount), TrackCount.ToString());
            allTags.Add(nameof(Disc), Disc.ToString());
            allTags.Add(nameof(DiscCount), DiscCount.ToString());
            allTags.Add(nameof(MusicBrainzReleaseArtistId), MusicBrainzReleaseArtistId.ToString());
            allTags.Add(nameof(MusicBrainzTrackId), MusicBrainzTrackId.ToString());
            allTags.Add(nameof(MusicBrainzDiscId), MusicBrainzDiscId.ToString());
            allTags.Add(nameof(MusicBrainzReleaseStatus), MusicBrainzReleaseStatus.ToString());
            allTags.Add(nameof(MusicBrainzReleaseType), MusicBrainzReleaseType.ToString());
            allTags.Add(nameof(MusicBrainzReleaseCountry), MusicBrainzReleaseCountry.ToString());
            allTags.Add(nameof(MusicBrainzReleaseId), MusicBrainzReleaseId.ToString());
            allTags.Add(nameof(MusicBrainzArtistId), MusicBrainzArtistId.ToString());

            return allTags;
        }
        public Dictionary<string, object> GetAllMetaTagsNative() // Returns all metadata of the track in a rough, native, saveable manner
        {
            Dictionary<string, object> allTags = new Dictionary<string, object>();

            allTags.Add(nameof(Title), Title);
            allTags.Add(nameof(Album), Album);
            allTags.Add(nameof(Copyright), Copyright);
            allTags.Add(nameof(AlbumArtists), AlbumArtists);
            allTags.Add(nameof(AlbumArtistsSort), AlbumArtistsSort);
            allTags.Add(nameof(Genres), Genres);
            allTags.Add(nameof(BeatsPerMinute), BeatsPerMinute);
            allTags.Add(nameof(Year), Year);
            allTags.Add(nameof(Track), Track);
            allTags.Add(nameof(TrackCount), TrackCount);
            allTags.Add(nameof(Disc), Disc);
            allTags.Add(nameof(DiscCount), DiscCount);
            allTags.Add(nameof(MusicBrainzReleaseArtistId), MusicBrainzReleaseArtistId);
            allTags.Add(nameof(MusicBrainzTrackId), MusicBrainzTrackId);
            allTags.Add(nameof(MusicBrainzDiscId), MusicBrainzDiscId);
            allTags.Add(nameof(MusicBrainzReleaseStatus), MusicBrainzReleaseStatus);
            allTags.Add(nameof(MusicBrainzReleaseType), MusicBrainzReleaseType);
            allTags.Add(nameof(MusicBrainzReleaseCountry), MusicBrainzReleaseCountry);
            allTags.Add(nameof(MusicBrainzReleaseId), MusicBrainzReleaseId);
            allTags.Add(nameof(MusicBrainzArtistId), MusicBrainzArtistId);

            return allTags;
        }
        public Dictionary<string, string> GetAllMetaTagsFormattedNonEmpty() // Returns non-empty metadata of the track (formatted for display)
        {
            Dictionary<string, string> allTags = GetAllMetaTagsFormatted();

            // Filtering out null or empty tag values from all collection
            foreach (string key in allTags.Keys)
            {
                string value = null;
                allTags.TryGetValue(key, out value); // There must exist a corresponding value since we iterate valid keys
                if (String.IsNullOrWhiteSpace(value))
                {
                    allTags.Remove(key);
                }
            }

            return allTags;
        }
        public Dictionary<string, object> GetAllMetaTagsNativeNonEmpty() // Returns non-empty metadata of the track (native for save)
        {
            Dictionary<string, object> allTags = GetAllMetaTagsNative();

            // Filtering out null or empty tag values from all collection
            foreach (string key in allTags.Keys)
            {
                object value = null;
                allTags.TryGetValue(key, out value); // There must exist a corresponding value since we iterate valid keys
                if (value == null || String.IsNullOrWhiteSpace(value.ToString()))
                {
                    allTags.Remove(key);
                }
            }

            return allTags;
        }
    }
}