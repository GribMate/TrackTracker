using System;
using System.Collections.Generic;

using TrackTracker.Services.Interfaces;



namespace TrackTracker.Services
{
    public class TagLibSharpService : IMusicFileTaggingService
    {
        public Dictionary<string, string> Read(string path)
        {
            throw new NotImplementedException(); // TODO
        }

        public void Save(string path, Dictionary<string, string> metaTags)
        {
            throw new NotImplementedException(); // TODO
        }
    }
}
