using System;
using System.Collections.Generic;
using System.Threading.Tasks;



namespace TrackTracker.Services.Interfaces
{
    /*
     * Responsible for getting tracklist data and controlling playback on Spotify.
    */
    public interface ISpotifyService
    {
        Task Login();
        Task<Dictionary<string, object>> GetAccountInformation();
        Task<Dictionary<string, string>> GetAllPlaylists();
        Task<List<string>> GetPlaylistTrackIDs(string playListID);
        Task<Dictionary<string, string>> GetTrackData(string trackID);

        Task<string> GetCurrentlyPlaying();
        Task PausePlayback();
        Task ResumePlayback();
        
        Task ChangePlaybackMusic(string trackURI);
    }
}
