namespace Onlab.DAL.Interfaces
{
    /*
    Interface: IMusicBrainzProvider
    Description:
        Handles internet service calls from MusicBrainz API.
    */
    public interface IMusicBrainzProvider
    {
        void GetResultsByMBID(string MBID);
        void GetResultsByAcoustID(string acoustID);
        void GetResultsByMetaData(string title, string artist = null, string album = null);
    }
}
