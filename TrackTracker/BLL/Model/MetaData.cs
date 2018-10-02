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
        public MetaTagNumber TrackNumber { get; set; } // Number of track on the corresponding album (if any)
        public MetaTagNumber TrackCount { get; set; } // Number of total tracks on the corresponding album (if any)
        public MetaTagNumber DiscNumber { get; set; } // Number of disc of the corresponding album (if any)
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

        public virtual Dictionary<string, string> GetAllMetaTags() // Returns all metadata of the track
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
            allTags.Add(nameof(TrackNumber), TrackNumber.ToString());
            allTags.Add(nameof(TrackCount), TrackCount.ToString());
            allTags.Add(nameof(DiscNumber), DiscNumber.ToString());
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
        public virtual Dictionary<string, string> GetAllMetaTagsNonEmpty() // Returns non-empty metadata of the track
        {
            Dictionary<string, string> allTags = GetAllMetaTags();

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
    }
}