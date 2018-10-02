using System;
using TrackTracker.BLL.Enums;

namespace Onlab.BLL
{
    /*
    Class: PlayTrack
    State: Under construction | DEBUG
    Description:
        Represents a music track, that can be played based on the Track class. Used in the mix feature.
    */
    class PlayTrack : Track
    {
        //These variables are more customized for TrackTracker features (do not correspond 1-1 to ID3 tags, but are derived from them)
        public MusicGenres TypedGenre { get; set; } //predefined genres supported, but can be mixed as flags
        public LyricsLanguages Language { get; set; } //can be mixed as well (rare case though)

        public PlayTrack(TagLib.File fileHandle, bool generateCustomizedTags = true) : base(fileHandle)
        {
            if (generateCustomizedTags) GenerateCustomizedTags();
        }

        private void GenerateCustomizedTags()
        {
            throw new NotImplementedException(); //TODO: Implement custom tags
        }
    }
}
