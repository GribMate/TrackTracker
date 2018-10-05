using System.Threading.Tasks;
using System.Collections.Generic;

using TrackTracker.BLL; //TODO Remove



namespace TrackTracker.Services.Interfaces
{
    /*
     * Handles internet service calls from a music metadata service.
    */
    public interface IMetadataService
    {       
        Task<AudioMetaData> GetRecordingByMBID(string MBID); // Returns exactly one recording by it's ID
        Task<List<AudioMetaData>> GetRecordingsByMetaData(string title, string artist = null, string album = null, int? limit = null); // Returns all the recordings which correspond to the parameters
    }
}
