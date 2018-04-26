using MetaBrainz.MusicBrainz.Interfaces.Browses;
using MetaBrainz.MusicBrainz.Interfaces.Entities;
using MetaBrainz.MusicBrainz.Interfaces.Searches;
using MetaBrainz.MusicBrainz.Objects.Submissions;

using Onlab.DAL.Interfaces;



namespace Onlab.DAL
{
    /*
    Class: IMusicBrainzProvider
    Description:
        Implements IMusicBrainzProvider via MetaBrainz.MusicBrainz.
    */
    class MusicBrainzProvider : IMusicBrainzProvider
    {
        public void GetResultsByAcoustID(string acoustID)
        {
            throw new System.NotImplementedException();
        }

        public void GetResultsByMBID(string MBID)
        {
            throw new System.NotImplementedException();
        }

        public void GetResultsByMetaData(string title, string artist = null, string album = null)
        {
            throw new System.NotImplementedException();
        }
    }
}
