using System;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;

using MetaBrainz.MusicBrainz;
using MetaBrainz.MusicBrainz.Interfaces;
using MetaBrainz.MusicBrainz.Interfaces.Browses;
using MetaBrainz.MusicBrainz.Interfaces.Entities;
using MetaBrainz.MusicBrainz.Interfaces.Searches;
using MetaBrainz.MusicBrainz.Objects;
using MetaBrainz.MusicBrainz.Objects.Submissions;

using Onlab.Services.Interfaces;
using Onlab.BLL;



namespace Onlab.Services
{
    /*
    Class: IMusicBrainzProvider
    Description:
        Implements IMusicBrainzService via MetaBrainz.MusicBrainz.
    */
    public class MusicBrainzService : IMetadataService
    {
        public async Task<AudioMetaData> GetRecordingByMBID(string MBID) //returns exactly one recording by it's GUID MBID
        {
            Query query = new Query("TrackTracker"); //TODO: use the other ctor with more metadata about app and with own API key
            Guid guid = new Guid(MBID);

            IRecording recording = await query.LookupRecordingAsync(guid, Include.Releases & Include.Tags);

            AudioMetaData audioMetaData = new AudioMetaData()
            {
                Title = "",
                Album = "",
                Copyright = "",
                JoinedAlbumArtists = "",
                JoinedAlbumArtistsSort = "",
                JoinedGenres = "",
                BeatsPerMinute = 0,
                Year = 0,
                Track = 0,
                TrackCount = 0,
                Disc = 0,
                DiscCount = 0,
                MusicBrainzReleaseArtistId = "",
                MusicBrainzTrackId = "",
                MusicBrainzDiscId = "",
                MusicBrainzReleaseStatus = "",
                MusicBrainzReleaseType = "",
                MusicBrainzReleaseCountry = "",
                MusicBrainzReleaseId = "",
                MusicBrainzArtistId = ""
            };

            return audioMetaData;
        }

        public async Task<List<AudioMetaData>> GetRecordingsByMetaData(string title, int limit, string artist = null, string album = null) //returns all the recordings which correspond to the parameters
        {
            Query query = new Query("TrackTracker", "0.1Alpha", "gmate375@gmail.com"); //TODO: not this mail / version

            ISearchResults<IFoundRecording> retrieved = await query.FindRecordingsAsync(GetQueryStringFromMetaData(title, artist, album), limit);

            List<AudioMetaData> results = new List<AudioMetaData>();

            //JSON conversion tends to be buggy, so we null check everyting
            if (retrieved != null && retrieved.TotalResults > 0)
            {
                foreach (IFoundRecording recording in retrieved.Results)
                {
                    AudioMetaData result = new AudioMetaData()
                    {
                        Title = "",
                        Album = "",
                        Copyright = "",
                        JoinedAlbumArtists = "",
                        JoinedAlbumArtistsSort = "",
                        JoinedGenres = "",
                        BeatsPerMinute = 0,
                        Year = 0,
                        Track = 0,
                        TrackCount = 0,
                        Disc = 0,
                        DiscCount = 0,
                        MusicBrainzReleaseArtistId = "",
                        MusicBrainzTrackId = "",
                        MusicBrainzDiscId = "",
                        MusicBrainzReleaseStatus = "",
                        MusicBrainzReleaseType = "",
                        MusicBrainzReleaseCountry = "",
                        MusicBrainzReleaseId = "",
                        MusicBrainzArtistId = ""
                    };

                    results.Add(result);
                }
            }
            return results;
        }

        private string GetQueryStringFromMetaData(string title, string artist = null, string album = null) //gets the Lucene search query from parameters
        {
            //Example: musicbrainz.org/ws/2/recording?query="What I've Done" AND artistname:"Linkin Park" AND release:"Minutes to Midnight"

            StringBuilder queryString = new StringBuilder();

            // Title (required)
            queryString.Append("\"");
            queryString.Append(title);
            queryString.Append("\"");

            // Artist
            if (!String.IsNullOrEmpty(artist))
            {
                queryString.Append(" AND artistname:\"");
                queryString.Append(artist);
                queryString.Append("\"");
            }

            // Album
            if (!String.IsNullOrEmpty(album))
            {
                queryString.Append(" AND release:\"");
                queryString.Append(album);
                queryString.Append("\"");
            }

            return queryString.ToString();
        }
    }
}
