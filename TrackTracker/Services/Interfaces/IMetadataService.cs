using System;
using System.Threading.Tasks;
using System.Collections.Generic;



namespace TrackTracker.Services.Interfaces
{
    /*
     * Handles internet service calls from a music metadata service.
    */
    public interface IMetadataService
    {       
        Task<Dictionary<string, object>> GetRecordingByMBID(Guid MBID); // Returns exactly one recording's metadata by it's ID
        Task<List<Dictionary<string, object>>> GetRecordingsByMetaData(string title, string artist = null, string album = null, int? limit = null); // Returns all the recordings' metadata which correspond to the parameters
    }
}
