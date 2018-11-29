using System;

using TrackTracker.BLL.Enums;
using TrackTracker.BLL.Model.Base;



namespace TrackTracker.BLL.Model
{
    /*
     * Provides extended metadata information about a local music file.
     * These are more proprietary, TrackTracker related infos, then globally recognised.
    */
    public class MetaDataExtended : ModelObjectBase
    {
        public MetaDataExtended(MusicMoods? musicMoods = null, string lyrics = null, LyricsLanguages? lyricsLanguages = null, bool isSupported = false)
        {
            if (musicMoods.HasValue)
                MusicMoods = (MusicMoods)musicMoods;
            else
                MusicMoods = MusicMoods.Unknown;

            Lyrics = lyrics;

            if (lyricsLanguages.HasValue)
                LyricsLanguages = (LyricsLanguages)lyricsLanguages;
            else
                LyricsLanguages = LyricsLanguages.Unknown;

            IsSupported = isSupported;
        }

        public bool IsSupported { get; set; } // Is extended metadata info supported for the current track?
        public MusicMoods MusicMoods { get; set; } // A flag set of user-provided moods that apply for this track
        public string Lyrics { get; set; } // The lyrics of the track for future usage
        public LyricsLanguages LyricsLanguages { get; set; } // A flag set of lyrics languages of the music (typically one of them)
    }
}
