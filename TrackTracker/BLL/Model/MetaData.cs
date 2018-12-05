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
        // A default constructor, that allocates each MetaTag, with given keys (names) and null values
        // Setting of each MetaTag's value is handled directly
        public MetaData()
        {
            Genres = new MetaTagCollection(nameof(Genres), MusicGenres.Unknown.ToString());

            Title = new MetaTagString(nameof(Title));
            Album = new MetaTagString(nameof(Album));
            Copyright = new MetaTagString(nameof(Copyright));
            AlbumArtists = new MetaTagCollection(nameof(AlbumArtists));
            AlbumArtistsSort = new MetaTagCollection(nameof(AlbumArtistsSort));
            BeatsPerMinute = new MetaTagNumber(nameof(BeatsPerMinute));
            Year = new MetaTagNumber(nameof(Year));
            Track = new MetaTagNumber(nameof(Track));
            TrackCount = new MetaTagNumber(nameof(TrackCount));
            Disc = new MetaTagNumber(nameof(Disc));
            DiscCount = new MetaTagNumber(nameof(DiscCount));

            AcoustID = new MetaTagGUID(nameof(AcoustID));
            MusicBrainzReleaseArtistId = new MetaTagGUID(nameof(MusicBrainzReleaseArtistId));
            MusicBrainzTrackId = new MetaTagGUID(nameof(MusicBrainzTrackId));
            MusicBrainzDiscId = new MetaTagGUID(nameof(MusicBrainzDiscId));
            MusicBrainzReleaseId = new MetaTagGUID(nameof(MusicBrainzReleaseId));
            MusicBrainzArtistId = new MetaTagGUID(nameof(MusicBrainzArtistId));

            MusicBrainzReleaseStatus = new MetaTagString(nameof(MusicBrainzReleaseStatus));
            MusicBrainzReleaseType = new MetaTagString(nameof(MusicBrainzReleaseType));
            MusicBrainzReleaseCountry = new MetaTagString(nameof(MusicBrainzReleaseCountry));
        }

        // Currently Supported ID3 tags
        // TODO: Review ID3 tags and finalize support
        #region MetaTagProperties
        
        public MetaTagCollection Genres { get; set; } // Multiple flag-type genres of the track
        // TODO: Maybe add custom attribute to mark which enum it supports

        public MetaTagString Title { get; set; } // Title of the track
        public MetaTagString Album { get; set; } // Album title of the track
        public MetaTagString Copyright { get; set; } // Copyright information about the track
        public MetaTagCollection AlbumArtists { get; set; } // All of the artist names, who contributed to the album
        public MetaTagCollection AlbumArtistsSort { get; set; } // All of the artist names in sort form, who contributed to the album
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

        public List<MetaTagBase> GetAllMetaTags()
        {
            List<MetaTagBase> allTags = new List<MetaTagBase>();

            allTags.Add(Title);
            allTags.Add(Album);
            allTags.Add(Copyright);
            allTags.Add(AlbumArtists);
            allTags.Add(AlbumArtistsSort);

            allTags.Add(Genres);

            allTags.Add(BeatsPerMinute);
            allTags.Add(Year);
            allTags.Add(Track);
            allTags.Add(TrackCount);
            allTags.Add(Disc);
            allTags.Add(DiscCount);

            allTags.Add(AcoustID);
            allTags.Add(MusicBrainzReleaseArtistId);
            allTags.Add(MusicBrainzTrackId);
            allTags.Add(MusicBrainzDiscId);
            allTags.Add(MusicBrainzReleaseId);
            allTags.Add(MusicBrainzArtistId);

            allTags.Add(MusicBrainzReleaseStatus);
            allTags.Add(MusicBrainzReleaseType);
            allTags.Add(MusicBrainzReleaseCountry);
            

            return allTags;
        }

        public Dictionary<string, string> GetAllMetaTagsDataFormatted() // Returns all metadata of the track in a formatted, displayable manner
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
        public Dictionary<string, object> GetAllMetaTagsDataNative() // Returns all metadata of the track in a rough, native, saveable manner
        {
            Dictionary<string, object> allTags = new Dictionary<string, object>();

            allTags.Add(nameof(Genres), Genres.Value);

            allTags.Add(nameof(Title), Title.Value);
            allTags.Add(nameof(Album), Album.Value);
            allTags.Add(nameof(Copyright), Copyright.Value);
            allTags.Add(nameof(AlbumArtists), AlbumArtists.Value);
            allTags.Add(nameof(AlbumArtistsSort), AlbumArtistsSort.Value);
            allTags.Add(nameof(BeatsPerMinute), BeatsPerMinute.Value);
            allTags.Add(nameof(Year), Year.Value);
            allTags.Add(nameof(Track), Track.Value);
            allTags.Add(nameof(TrackCount), TrackCount.Value);
            allTags.Add(nameof(Disc), Disc.Value);
            allTags.Add(nameof(DiscCount), DiscCount.Value);

            allTags.Add(nameof(MusicBrainzReleaseArtistId), MusicBrainzReleaseArtistId.Value.ToString());
            allTags.Add(nameof(MusicBrainzTrackId), MusicBrainzTrackId.Value.ToString());
            allTags.Add(nameof(MusicBrainzDiscId), MusicBrainzDiscId.Value.ToString());
            allTags.Add(nameof(MusicBrainzReleaseId), MusicBrainzReleaseId.Value.ToString());
            allTags.Add(nameof(MusicBrainzArtistId), MusicBrainzArtistId.Value.ToString());

            allTags.Add(nameof(MusicBrainzReleaseStatus), MusicBrainzReleaseStatus.Value);
            allTags.Add(nameof(MusicBrainzReleaseType), MusicBrainzReleaseType.Value);
            allTags.Add(nameof(MusicBrainzReleaseCountry), MusicBrainzReleaseCountry.Value);

            return allTags;
        }
        public Dictionary<string, string> GetAllMetaTagsDataFormattedNonEmpty() // Returns non-empty metadata of the track (formatted for display)
        {
            Dictionary<string, string> allTags = GetAllMetaTagsDataFormatted();

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
        public Dictionary<string, object> GetAllMetaTagsDataNativeNonEmpty() // Returns non-empty metadata of the track (native for save)
        {
            Dictionary<string, object> allTags = GetAllMetaTagsDataNative();

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