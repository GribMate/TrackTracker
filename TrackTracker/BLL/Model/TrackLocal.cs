using System;
using System.Collections.Generic;

using TrackTracker.BLL.Enums;



namespace TrackTracker.BLL.Model
{
    /*
     * Represents an offline, local physical music file on the user's computer.
    */
    public class TrackLocal : TrackBase
    {
        public TrackLocal(MusicFileProperties musicFileProperties, MetaData metaData = null, MetaDataExtended metaDataExtended = null) : base(metaData)
        {
            if (metaDataExtended != null)
                MetaDataExtended = metaDataExtended;
            else
                MetaDataExtended = new MetaDataExtended();

            MusicFileProperties = musicFileProperties;

            PlayableOnline = false;
            PlayableOffline = true;
            SupportedMediaPlayers = SupportedMediaPlayersConverter.GetOfflinePlayersWhichSupportFormat(MusicFileProperties.FileExtension);

            MatchCandidates = new List<TrackVirtual>();
            ActiveCandidateMBTrackID = null;
        }
        public TrackLocal(string filePath, MetaData metaData = null, MetaDataExtended metaDataExtended = null) : this(new MusicFileProperties(filePath), metaData, metaDataExtended)
        {
            if (String.IsNullOrWhiteSpace(filePath))
                throw new ArgumentNullException(nameof(filePath), $"Cannot instantiate new offline track, since file path is null or empty");
        }

        public MetaDataExtended MetaDataExtended { get; set; } // Provides extended, TrackTracker proprietary information
        public MusicFileProperties MusicFileProperties { get; set; } // Local music file related data

        public List<TrackVirtual> MatchCandidates { get; set; } // All the candidate tracks, that MBAPI returned
        public MetaTagGUID ActiveCandidateMBTrackID { get; set; } // Currently selected match to sync metadata from, identified by its MB Track ID
    }
}