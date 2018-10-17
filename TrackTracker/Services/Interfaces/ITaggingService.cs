using System;
using System.Collections.Generic;



namespace TrackTracker.Services.Interfaces
{
    /*
     * Handles offline music file tagging.
    */
    public interface ITaggingService
    {
        Dictionary<string, object> Read(string path, List<string> allowedExtensions); // Collects metadata into a general collection about a file
        void Save(string path, Dictionary<string, object> metaTags); // Saves new tagging information to an already existing file
    }
}
