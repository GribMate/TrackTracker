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
                Include.Releases & Include.Tags & Include.Artists & Include.DiscIds & Include.Labels & Include.Media & Include.UserTags,
                ReleaseType.Album, ReleaseStatus.Official);

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
                StringBuilder sbArt = new StringBuilder();
                StringBuilder sbArtSort = new StringBuilder();

                foreach (INameCredit artistCredit in recording.ArtistCredit)
                {
                    if (artistCredit.Artist != null)
                    {
                        if (artistCredit.Artist.Name != null)
                        {
                            sbArt.Append(artistCredit.Artist.Name);
                            sbArt.Append(";");
                        }
                        if (artistCredit.Artist.SortName != null)
                        {
                            sbArtSort.Append(artistCredit.Artist.Name);
                            sbArtSort.Append(";");
                        }
                    }
                }
                result.Add("JoinedAlbumArtists", sbArt.ToString().Substring(0, sbArt.Length - 1)); // Remove closing ';'
                result.Add("JoinedAlbumArtistsSort", sbArtSort.ToString().Substring(0, sbArtSort.Length - 1)); // Remove closing ';'
            }

            // Checking other data
            else if (recording.Releases != null && recording.Releases.Count > 0) //TODO: accepting the first release might be improved
            {
                if (recording.Releases[0].Date != null)
                    result.Add("Year", (uint)recording.Releases[0].Date.Year);
                if (recording.Releases[0].MbId != null)
                    result.Add("MusicBrainzReleaseId", recording.Releases[0].MbId);
                if (recording.Releases[0].ArtistCredit != null && recording.Releases[0].ArtistCredit.Count > 0)
                {
                    if (recording.Releases[0].ArtistCredit[0].Artist.MbId != null)
                        result.Add("MusicBrainzArtistId", recording.Releases[0].ArtistCredit[0].Artist.MbId);
                }
            }

            return result;
        }

        public async Task<List<Dictionary<string, object>>> GetRecordingsByMetaData(string title, string artist = null, string album = null, int? limit = null) // Returns all the recordings' metadata which correspond to the parameters
        {
            Query query = new Query("TrackTracker", "0.1Alpha", "GMate375@gmail.com"); //TODO: not this mail / version

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
                            result.Add("Album", recording.Releases[0].Title); //TODO: accepting the first release might be improved
                    }

                    // Checking album artists
                    if (recording.ArtistCredit != null && recording.ArtistCredit.Count > 0)
                    {
                        StringBuilder sbArt = new StringBuilder();
                        StringBuilder sbArtSort = new StringBuilder();

                        foreach (INameCredit artistCredit in recording.ArtistCredit)
                        {
                            if (artistCredit.Artist != null)
                            {
                                if (artistCredit.Artist.Name != null)
                                {
                                    sbArt.Append(artistCredit.Artist.Name);
                                    sbArt.Append(";");
                                }
                                if (artistCredit.Artist.SortName != null)
                                {
                                    sbArtSort.Append(artistCredit.Artist.Name);
                                    sbArtSort.Append(";");
                                }
                            }
                        }
                        result.Add("JoinedAlbumArtists", sbArt.ToString().Substring(0, sbArt.Length - 1)); // Remove closing ';'
                        result.Add("JoinedAlbumArtistsSort", sbArtSort.ToString().Substring(0, sbArtSort.Length - 1)); // Remove closing ';'
                    }
                    if (artist != null) // Checking after parsing results, since "JoinedAlbumArtistsSort" might be new info even if "artist" is known
                    {
                        if (result.ContainsKey("JoinedAlbumArtists"))
                            result.Remove("JoinedAlbumArtists");

                        result.Add("JoinedAlbumArtists", artist);
                    }

                    // Checking other data
                    else if (recording.Releases != null && recording.Releases.Count > 0) //TODO: accepting the first release might be improved
                    {
                        if (recording.Releases[0].Date != null)
                            result.Add("Year", (uint)recording.Releases[0].Date.Year);
                        if (recording.Releases[0].MbId != null)
                            result.Add("MusicBrainzReleaseId", recording.Releases[0].MbId);
                        if (recording.Releases[0].ArtistCredit != null && recording.Releases[0].ArtistCredit.Count > 0)
                        {
                            if (recording.Releases[0].ArtistCredit[0].Artist.MbId != null)
                                result.Add("MusicBrainzArtistId", recording.Releases[0].ArtistCredit[0].Artist.MbId);
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
