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
            Query query = new Query("TrackTracker", "0.1Alpha", "GMate375@gmail.com"); //TODO: not this mail / version
            IRecording recording = await query.LookupRecordingAsync(new Guid(MBID), Include.Releases & Include.Tags);

            AudioMetaData result = new AudioMetaData();
            if (result.Title != null) result.Title = result.Title;

            //checking album title
            if (recording.Releases != null && recording.Releases.Count > 0)
            {
                if (recording.Releases[0].Title != null) result.Album = recording.Releases[0].Title; //TODO: accepting the first release might be improved
            }

            //checking album artists
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
                    result.JoinedAlbumArtists = sbArt.ToString().Substring(0, sbArt.Length - 1); //remove closing ';'
                    result.JoinedAlbumArtistsSort = sbArtSort.ToString().Substring(0, sbArtSort.Length - 1); //remove closing ';'
                }
            }

            //checking other data
            else if (recording.Releases != null && recording.Releases.Count > 0) //TODO: accepting the first release might be improved
            {
                if (recording.Releases[0].Date != null) result.Year = (uint)recording.Releases[0].Date.Year;
                if (recording.Releases[0].MbId != null) result.MusicBrainzReleaseId = recording.Releases[0].MbId.ToString();
                if (recording.Releases[0].ArtistCredit != null && recording.Releases[0].ArtistCredit.Count > 0)
                {
                    if (recording.Releases[0].ArtistCredit[0].Artist.MbId != null) result.MusicBrainzArtistId = recording.Releases[0].ArtistCredit[0].Artist.MbId.ToString();
                }
            }

            return result;
        }

        public async Task<List<AudioMetaData>> GetRecordingsByMetaData(string title, string artist = null, string album = null, int? limit = null) //returns all the recordings which correspond to the parameters
        {
            Query query = new Query("TrackTracker", "0.1Alpha", "GMate375@gmail.com"); //TODO: not this mail / version

            ISearchResults<IFoundRecording> retrieved = await query.FindRecordingsAsync(GetQueryStringFromMetaData(title, artist, album), limit);

            List<AudioMetaData> results = new List<AudioMetaData>();

            //JSON conversion tends to be buggy, so we null check everyting
            if (retrieved != null && retrieved.TotalResults > 0)
            {
                foreach (IFoundRecording recording in retrieved.Results)
                {
                    AudioMetaData result = new AudioMetaData();
                    result.Title = title; //we searched at least by the title, we don't want to update that

                    //checking album title
                    if (album != null)
                    {
                        result.Album = album;
                    }
                    else if (recording.Releases != null && recording.Releases.Count > 0)
                    {
                        if (recording.Releases[0].Title != null) result.Album = recording.Releases[0].Title; //TODO: accepting the first release might be improved
                    }

                    //checking album artists
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
                            result.JoinedAlbumArtists = sbArt.ToString().Substring(0, sbArt.Length - 1); //remove closing ';'
                            result.JoinedAlbumArtistsSort = sbArtSort.ToString().Substring(0, sbArtSort.Length - 1); //remove closing ';'
                        }
                    }
                    if (artist != null) //checking after parsing results, since "JoinedAlbumArtistsSort" might be new info even if "artist" is known
                    {
                        result.JoinedAlbumArtists = artist;
                    }

                    //checking other data
                    else if (recording.Releases != null && recording.Releases.Count > 0) //TODO: accepting the first release might be improved
                    {
                        if (recording.Releases[0].Date != null) result.Year = (uint)recording.Releases[0].Date.Year;
                        if (recording.Releases[0].MbId != null) result.MusicBrainzReleaseId = recording.Releases[0].MbId.ToString();
                        if (recording.Releases[0].ArtistCredit != null && recording.Releases[0].ArtistCredit.Count > 0)
                        {
                            if (recording.Releases[0].ArtistCredit[0].Artist.MbId != null) result.MusicBrainzArtistId = recording.Releases[0].ArtistCredit[0].Artist.MbId.ToString();
                        }
                    }

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
