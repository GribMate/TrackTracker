using System;

using TrackTracker.BLL.Enums;
using TrackTracker.Services.Interfaces;



namespace TrackTracker.BLL.Model
{
    /*
     * Represents a track, that has various attributes, metadata and capabilities.
     * Child classes specify if it's a local offline copy, an online playable track or virtual set of metadata.
    */
    public class TrackBase
    {
        public MetaData MetaData { get; set; } // Basic set of metadata, that all tracks share

        public bool PlayableOnline { get; set; } // Is this track playable by some online service or not?
        public bool PlayableOffline { get; set; } // Is this track accessible on local computer and hence playable offline?
        public SupportedMediaPlayers SupportedMediaPlayers { get; set; } // If it is playable, this shows by what exactly

        public virtual void Save(IDatabaseService dbService)
        {
            throw new NotImplementedException(); //TODO
        }
    }
}