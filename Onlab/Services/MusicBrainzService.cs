using System;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using MetaBrainz.MusicBrainz;
using MetaBrainz.MusicBrainz.Interfaces;
using MetaBrainz.MusicBrainz.Interfaces.Browses;
using MetaBrainz.MusicBrainz.Interfaces.Entities;
using MetaBrainz.MusicBrainz.Interfaces.Searches;
using MetaBrainz.MusicBrainz.Objects;
using MetaBrainz.MusicBrainz.Objects.Submissions;

using Onlab.Services.Interfaces;



namespace Onlab.Services
{
    /*
    Class: IMusicBrainzProvider
    Description:
        Implements IMusicBrainzService via MetaBrainz.MusicBrainz.
    */
    class MusicBrainzService : IMetadataService
    {
        public async Task<Dictionary<string, string>> GetRecordingByMBID(string MBID) //returns exactly one recording by it's GUID MBID
        {
            Query query = new Query("TrackTracker"); //TODO: use the other ctor with more metadata about app and with own API key
            Guid guid = new Guid(MBID);

            IRecording recording = await query.LookupRecordingAsync(guid, Include.Releases & Include.Tags);

            Dictionary<string, string> tags = new Dictionary<string, string>();

            tags.Add("Annotation", recording.Annotation);
            tags.Add("Disambiguation", recording.Disambiguation);
            tags.Add("MBID", recording.MbId.ToString()); //for test purposes
            tags.Add("UserRating", recording.UserRating.Value.Value.ToString());

            foreach (IUserTag tag in recording.UserTags) //only gets tags that were added by authenticated users
            {
                tags.Add(tag.Name, tag.ToString()); //TODO: debug
            }

            return tags;
        }

        public async Task<List<Dictionary<string, string>>> GetRecordingsByMetaData(string title, int limit, string artist = null, string album = null) //returns all the recordings which correspond to the parameters
        {
            Query query = new Query("TrackTracker"); //TODO: use the other ctor with more metadata about app and with own API key

            ISearchResults<IFoundRecording> retrieved = await query.FindRecordingsAsync(GetQueryStringFromMetaData(title, artist, album), limit);

            List<Dictionary<string, string>> results = new List<Dictionary<string, string>>();
            if (retrieved.TotalResults > 0) //just in case, because MetaBrainz and JSON conversion tends to be buggy
            {
                foreach (IFoundRecording recording in retrieved.Results)
                {
                    Dictionary<string, string> tags = new Dictionary<string, string>();

                    tags.Add("Annotation", recording.Annotation);
                    tags.Add("Disambiguation", recording.Disambiguation);
                    tags.Add("MBID", recording.MbId.ToString()); //for test purposes
                    tags.Add("UserRating", recording.UserRating.Value.Value.ToString());

                    foreach (IUserTag tag in recording.UserTags) //only gets tags that were added by authenticated users
                    {
                        tags.Add(tag.Name, tag.ToString()); //TODO: debug
                    }

                    results.Add(tags);
                }
            }
            return results;
        }

        private string GetQueryStringFromMetaData(string title, string artist = null, string album = null) //gets the Lucene search query from parameters
        {
            //  URL: musicbrainz.org/ws/2/recording?query=%22What%20I%27ve%20Done%22%20AND%20artist:%22Linkin%20Park%22%20AND%20release:%22Minutes%20to%20Midnight%22
            //Human: musicbrainz.org/ws/2/recording?query= " What   I ' ve   Done "    AND   artist: " Linkin   Park "    AND   release: " Minutes   to   Midnight "

            /*  URL encoding rules
             *      ' ---> %27
             *      " ---> %22
             *      _ ---> %20 (whitespace, NOT underline!)
            */

            StringBuilder queryString = new StringBuilder();

            //   TITLE
            queryString.Append("\"");
            queryString.Append(title);
            queryString.Append("\"");

            //   ARTIST
            if (!String.IsNullOrEmpty(artist))
            {
                queryString.Append(" AND artist:\"");
                queryString.Append(artist);
                queryString.Append("\"");
            }

            //   ALBUM
            if (!String.IsNullOrEmpty(album))
            {
                queryString.Append(" AND release:\"");
                queryString.Append(album);
                queryString.Append("\"");
            }

            queryString.Replace("\'", "%27");
            queryString.Replace("\"", "%22");
            queryString.Replace(" ", "%20");

            return queryString.ToString();
        }
    }
}
