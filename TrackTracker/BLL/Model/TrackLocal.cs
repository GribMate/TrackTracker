using System;
using System.Collections.Generic;

namespace TrackTracker.BLL.Model
{
    /*
     * Represents an offline, local physical music file on the user's computer.
    */
    public class TrackLocal : TrackBase
    {
        public MetaDataExtended MetaDataExtended { get; set; } // Provides extended, TrackTracker proprietary information
        public MusicFileProperties MusicFileProperties { get; set; } // Local music file related data

        public List<TrackVirtual> MatchCandidates { get; set; } // All the candidate tracks, that MBAPI returned
        public int ActiveCandidateIndex { get; set; } // Currently selected match to sync metadata from
    }
}