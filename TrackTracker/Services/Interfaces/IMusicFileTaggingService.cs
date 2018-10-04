using System;
using System.Collections.Generic;



namespace TrackTracker.Services.Interfaces
{
    /*
     * Handles offline music file tagging.
    */
    public interface IMusicFileTaggingService
    {
        Dictionary<string, string> Read(string path); // Collects metadata into a general collection about a file
        void Save(string path, Dictionary<string, string> metaTags); // Saves new tagging information to an already existing file
    }
}
