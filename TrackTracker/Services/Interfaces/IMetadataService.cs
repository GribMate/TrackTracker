using System.Threading.Tasks;
using System.Collections.Generic;
using Onlab.BLL;

namespace Onlab.Services.Interfaces
{
    /*
    Interface: IMetadataService
    Description:
        Handles internet service calls from MusicBrainz API.
    */
    public interface IMetadataService
    {       
        Task<AudioMetaData> GetRecordingByMBID(string MBID); //returns exactly one recording by it's GUID MBID
        Task<List<AudioMetaData>> GetRecordingsByMetaData(string title, int limit, string artist = null, string album = null); //returns all the recordings which correspond to the parameters
    }
}
