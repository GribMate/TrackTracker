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

using TrackTracker.Services.Interfaces;



namespace TrackTracker.Services
{
    /*
     * Implements IMetaDataService via MetaBrainz.MusicBrainz.
    */
    public class MusicBrainzService : IMetadataService
    {
        public async Task<Dictionary<string, object>> GetRecordingByMBID(Guid MBID) // Returns exactly one recording's metadata by it's ID
        {
            Query query = new Query("TrackTracker", "0.1Alpha", "GMate375@gmail.com"); // TODO: not this mail / version
            IRecording recording = await query.LookupRecordingAsync(MBID,
                Include.Releases & Include.Tags & Include.Artists & Include.DiscIds & Include.Labels & Include.Media & Include.UserTags);

            Dictionary<string, object> result = new Dictionary<string, object>();

            // JSON conversion tends to be buggy, so we null check everyting

            // Checking album title
            if (recording.Releases != null && recording.Releases.Count > 0)
            {
                if (recording.Releases[0].Title != null)
                    result.Add("Album", recording.Releases[0].Title); // TODO: accepting the first release might be improved
            }

            // Checking album artists
            if (recording.ArtistCredit != null && recording.ArtistCredit.Count > 0)
            {
                List<string> sbArt = new List<string>();
                List<string> sbArtSort = new List<string>();

                foreach (INameCredit artistCredit in recording.ArtistCredit)
                {
                    if (artistCredit.Artist != null)
                    {
                        if (artistCredit.Artist.Name != null)
                            sbArt.Add(artistCredit.Artist.Name);
                        if (artistCredit.Artist.SortName != null)
                            sbArtSort.Add(artistCredit.Artist.Name);
                    }
                }
                result.Add("AlbumArtists", sbArt.ToArray());
                result.Add("AlbumArtistsSort", sbArtSort.ToArray());

                if (recording.ArtistCredit.Count == 1) // If we only have exactly 1 artist, we accept its MBID
                {
                    // TODO: improve later
                    result.Add("MusicBrainzArtistId", recording.ArtistCredit[0].Artist.MbId.ToString());
                }
            }

            // Checking track MBID
            if (recording.MbId != null)
                result.Add("MusicBrainzTrackId", recording.MbId.ToString());

            // Checking other data
            if (recording.Releases != null && recording.Releases.Count > 0) // TODO: accepting the first release might be improved
            {
                if (recording.Releases[0].Date != null)
                    result.Add("Year", (uint)recording.Releases[0].Date.Year);
                if (recording.Releases[0].MbId != null)
                    result.Add("MusicBrainzReleaseId", recording.Releases[0].MbId.ToString());
                if (recording.Releases[0].Status != null)
                    result.Add("MusicBrainzReleaseStatus", recording.Releases[0].Status);
                if (recording.Releases[0].Country != null)
                    result.Add("MusicBrainzReleaseCountry", recording.Releases[0].Country);
                if (recording.Releases[0].ArtistCredit != null && recording.Releases[0].ArtistCredit.Count > 0)
                {
                    if (recording.Releases[0].ArtistCredit[0].Artist.MbId != null)
                        result.Add("MusicBrainzReleaseArtistId", recording.Releases[0].ArtistCredit[0].Artist.MbId.ToString());
                }
                if (recording.Releases[0].Media != null && recording.Releases[0].Media.Count > 0)
                {
                    result.Add("Track", (uint)recording.Releases[0].Media[0].TrackOffset);
                    result.Add("TrackCount", (uint)recording.Releases[0].Media[0].TrackCount);
                    result.Add("MusicBrainzReleaseType", recording.Releases[0].Media[0].Format);
                }
            }

            return result;
        }

        public async Task<List<Dictionary<string, object>>> GetRecordingsByMetaData(string title, string artist = null, string album = null, int? limit = null) // Returns all the recordings' metadata which correspond to the parameters
        {
            Query query = new Query("TrackTracker", "0.1Alpha", "GMate375@gmail.com"); // TODO: not this mail / version

            ISearchResults<IFoundRecording> retrieved = await query.FindRecordingsAsync(GetQueryStringFromMetaData(title, artist, album), limit);

            List<Dictionary<string, object>> results = new List<Dictionary<string, object>>();

            // JSON conversion tends to be buggy, so we null check everyting
            if (retrieved != null && retrieved.TotalResults > 0)
            {
                foreach (IFoundRecording recording in retrieved.Results)
                {
                    Dictionary<string, object> result = new Dictionary<string, object>();
                    result.Add("Title", title); // We searched at least by the title, we don't want to update that

                    // Checking album title
                    if (album != null)
                    {
                        result.Add("Album", album);
                    }
                    else if (recording.Releases != null && recording.Releases.Count > 0)
                    {
                        if (recording.Releases[0].Title != null)
                            result.Add("Album", recording.Releases[0].Title); // TODO: accepting the first release might be improved
                    }

                    // Checking album artists
                    if (recording.ArtistCredit != null && recording.ArtistCredit.Count > 0)
                    {
                        List<string> sbArt = new List<string>();
                        List<string> sbArtSort = new List<string>();

                        foreach (INameCredit artistCredit in recording.ArtistCredit)
                        {
                            if (artistCredit.Artist != null)
                            {
                                if (artistCredit.Artist.Name != null)
                                    sbArt.Add(artistCredit.Artist.Name);
                                if (artistCredit.Artist.SortName != null)
                                    sbArtSort.Add(artistCredit.Artist.Name);
                            }
                        }
                        result.Add("AlbumArtists", sbArt.ToArray());
                        result.Add("AlbumArtistsSort", sbArtSort.ToArray());

                        if (recording.ArtistCredit.Count == 1)
                        {
                            result.Add("MusicBrainzArtistId", recording.ArtistCredit[0].Artist.MbId.ToString());
                        }
                    }

                    // Checking track MBID
                    if (recording.MbId != null)
                        result.Add("MusicBrainzTrackId", recording.MbId.ToString());

                    // Checking other data
                    if (recording.Releases != null && recording.Releases.Count > 0) // TODO: accepting the first release might be improved
                    {
                        if (recording.Releases[0].Date != null)
                            result.Add("Year", (uint)recording.Releases[0].Date.Year);
                        if (recording.Releases[0].MbId != null)
                            result.Add("MusicBrainzReleaseId", recording.Releases[0].MbId.ToString());
                        if (recording.Releases[0].Status != null)
                            result.Add("MusicBrainzReleaseStatus", recording.Releases[0].Status);
                        if (recording.Releases[0].Country != null)
                            result.Add("MusicBrainzReleaseCountry", recording.Releases[0].Country);
                        if (recording.Releases[0].ArtistCredit != null && recording.Releases[0].ArtistCredit.Count > 0)
                        {
                            if (recording.Releases[0].ArtistCredit[0].Artist.MbId != null)
                                result.Add("MusicBrainzReleaseArtistId", recording.Releases[0].ArtistCredit[0].Artist.MbId.ToString());
                        }
                        if (recording.Releases[0].Media != null && recording.Releases[0].Media.Count > 0)
                        {
                            result.Add("Track", (uint)recording.Releases[0].Media[0].TrackOffset);
                            result.Add("TrackCount", (uint)recording.Releases[0].Media[0].TrackCount);
                            result.Add("MusicBrainzReleaseType", recording.Releases[0].Media[0].Format);
                        }
                    }

                    results.Add(result);
                }
            }
            return results;
        }

        private string GetQueryStringFromMetaData(string title, string artist = null, string album = null) // Gets the Lucene search query from parameters
        {
            // Example: musicbrainz.org/ws/2/recording?query="What I've Done" AND artistname:"Linkin Park" AND release:"Minutes to Midnight"

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
