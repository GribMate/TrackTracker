using System;



namespace TrackTracker.BLL.Model
{
    /*
     * Represents a virtual track located online.
     * This might be an online playable track, like a Spotify record, or a completely virtual set of metadata about a track,
     * like a MusicBrainz potential match.
    */
    public class TrackVirtual : TrackBase
    {
        public bool IsOnlyMetaData { get; set; } // Is this track only a virtual set of metadata (like a MusicBrainz match)?
    }
}