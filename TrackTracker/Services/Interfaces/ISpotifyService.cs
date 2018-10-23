using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using static TrackTracker.Services.SpotifyService;

namespace TrackTracker.Services.Interfaces
{
    /*
     * Responsible for getting tracklist data and controlling playback on Spotify.
    */
    public interface ISpotifyService
    {
        void TEST_LOGIN_PLAYLIST(LoginCallback callback);
        void TEST_PLAYLISTDATA(string selectedPlaylistName, PlaylistCallback callback);
        Task<string> TEST_PLAYING();
        void TEST_PLAY_PAUSE();

        /*
        void Login();
        Task<valami> GetAccountInformation();
        Task<List<string>> GetAllPlaylistNames();
        Task<valami> GetPlaylistData(Guid playListID);
        Task<bool> PausePlayback();
        Task<bool> ResumePlayback();
        Task<bool> ChangePlaybackMusic(Guid musicID);
        */
    }
}
