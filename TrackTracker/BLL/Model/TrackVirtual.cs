using System;

using TrackTracker.BLL.Enums;



namespace TrackTracker.BLL.Model
{
    /*
     * Represents a virtual track located online.
     * This might be an online playable track, like a Spotify record, or a completely virtual set of metadata about a track,
     * like a MusicBrainz potential match.
    */
    public class TrackVirtual : TrackBase
    {
        public TrackVirtual(MetaData metaData, bool isOnlyMetaData) : base (metaData)
        {
            IsOnlyMetaData = isOnlyMetaData;

            PlayableOnline = !IsOnlyMetaData;
            PlayableOffline = false;
            SupportedMediaPlayers = SupportedMediaPlayersConverter.GetDefaultOnlinePlayer();
        }

        public bool IsOnlyMetaData { get; set; } // Is this track only a virtual set of metadata (like a MusicBrainz match)?
    }
}