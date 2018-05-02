using System.Threading.Tasks;
using System.Collections.Generic;



namespace Onlab.DAL.Interfaces
{
    /*
    Interface: IMusicBrainzProvider
    Description:
        Handles internet service calls from MusicBrainz API.
    */
    public interface IMusicBrainzProvider
    {
        Task<Dictionary<string, string>> GetRecordingByMBID(string MBID); //returns exactly one recording by it's GUID MBID
        Task<List<Dictionary<string, string>>> GetRecordingsByMetaData(string title, int limit, string artist = null, string album = null); //returns all the recordings which correspond to the parameters
    }
}
