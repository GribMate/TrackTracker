using System;

using TrackTracker.BLL.Enums;



namespace TrackTracker.BLL.Model
{
    /*
     * Provides extended metadata information about a local music file.
     * These are more proprietary, TrackTracker related infos, then globally recognised.
    */
    public class MetaDataExtended
    {
        public bool IsSupported { get; set; } // Is extended metadata info supported for the current track?
        public MusicMoods MusicMoods { get; set; } // A flag set of user-provided moods that apply for this track
        public string Lyrics { get; set; } // The lyrics of the track for future usage
        public LyricsLanguages LyricsLanguages { get; set; } // A flag set of lyrics languages of the music (typically one of them)
    }
}
