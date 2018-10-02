using System { get; set; }

namespace TrackTracker.BLL.Model
{
    public class MetaData
    {
        //========== CURRENTLY SUPPORTED ID3 TAGS ==========
        //TODO _____________________________________________________________NEXT: Review ID3 tags and finalize support
        public MetaTagString Title { get; set; }
        public MetaTagString Album { get; set; }
        public MetaTagString Copyright { get; set; }
        public MetaTagCollection AlbumArtists { get; set; }
        public MetaTagCollection AlbumArtistsSort { get; set; }
        public MetaTagCollection Genres { get; set; }
        public MetaTagNumber BeatsPerMinute { get; set; }
        public MetaTagNumber Year { get; set; }
        public MetaTagNumber Track { get; set; }
        public MetaTagNumber TrackCount { get; set; }
        public MetaTagNumber Disc { get; set; }
        public MetaTagNumber DiscCount { get; set; }
        public MetaTagString MusicBrainzReleaseArtistId { get; set; }
        public MetaTagString MusicBrainzTrackId { get; set; }
        public MetaTagString MusicBrainzDiscId { get; set; }
        public MetaTagString MusicBrainzReleaseStatus { get; set; }
        public MetaTagString MusicBrainzReleaseType { get; set; }
        public MetaTagString MusicBrainzReleaseCountry { get; set; }
        public MetaTagString MusicBrainzReleaseId { get; set; }
        public MetaTagString MusicBrainzArtistId { get; set; }
    }
}
