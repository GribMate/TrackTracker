using System;
using System.Threading.Tasks;

using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Models;

using TrackTracker.Services.Interfaces;



namespace TrackTracker.Services
{
    public class SpotifyService : ISpotifyService
    {
        public delegate void LoginCallback(string name, System.Collections.Generic.List<string> playlistNames);
        public delegate void PlaylistCallback(System.Collections.Generic.List<string> tracks);
        //TODO: event

        private LoginCallback callback = null;


        private static string _clientId = "dda143ae78d64b43a8e390d5fdbc8cce";
        private static string _secretId = "045ea52fa6d440718c900bacbd3dd4f5";

        private static SpotifyWebAPI api = null;

        public void TEST_LOGIN_PLAYLIST(LoginCallback callback)
        {
            AuthorizationCodeAuth auth = new AuthorizationCodeAuth(_clientId, _secretId, "http://localhost:4002", "http://localhost:4002",
                Scope.PlaylistModifyPrivate | Scope.PlaylistModifyPublic | Scope.PlaylistReadCollaborative |
                Scope.PlaylistReadPrivate | Scope.Streaming | Scope.UserFollowModify | Scope.UserFollowRead |
                Scope.UserLibraryModify | Scope.UserLibraryRead | Scope.UserModifyPlaybackState |
                Scope.UserReadBirthdate | Scope.UserReadCurrentlyPlaying | Scope.UserReadEmail |
                Scope.UserReadPlaybackState | Scope.UserReadPrivate | Scope.UserReadRecentlyPlayed |
                Scope.UserTopRead);
            auth.AuthReceived += AuthOnAuthReceived;
            auth.Start();
            auth.OpenBrowser();

            this.callback = callback;
        }
        public async void TEST_PLAYLISTDATA(string selectedPlaylistName, PlaylistCallback callback)
        {
            PrivateProfile profile = await api.GetPrivateProfileAsync();
            System.Collections.Generic.List<string> trackNames = new System.Collections.Generic.List<string>();
            Paging<SimplePlaylist> playlists = await api.GetUserPlaylistsAsync(profile.Id);
            do
            {
                playlists.Items.ForEach(playlist =>
                {
                    if (playlist.Name.Equals(selectedPlaylistName))
                    {
                        FullPlaylist playlistData = api.GetPlaylist(profile.Id, playlist.Id);
                        do
                        {
                            playlistData.Tracks.Items.ForEach(track =>
                            {
                                trackNames.Add(track.Track.Name + " (ID: " + track.Track.Id + ")");
                            });
                        } while (playlistData.Tracks.HasNextPage());
                    }
                });
            } while (playlists.HasNextPage());

            callback(trackNames);
        }

        public async Task<string> TEST_PLAYING()
        {
            PlaybackContext context = await api.GetPlaybackAsync();
            if (context.Item != null)
            {
                return context.Item.Name;
            }
            else
            {
                return "No playing track found / error.";
            }
        }

        public void TEST_PLAY_PAUSE()
        {
            PlaybackContext context = api.GetPlayback();

            if (context.IsPlaying)
                api.PausePlayback();
            else
                api.ResumePlayback("", "", null, "", 0);
        }

        private async void AuthOnAuthReceived(object sender, AuthorizationCode payload)
        {
            AuthorizationCodeAuth auth = (AuthorizationCodeAuth)sender;
            auth.Stop();

            Token token = await auth.ExchangeCode(payload.Code);
            api = new SpotifyWebAPI
            {
                AccessToken = token.AccessToken,
                TokenType = token.TokenType
            };


            PrivateProfile profile = await api.GetPrivateProfileAsync();
            System.Collections.Generic.List<string> playlistNames = new System.Collections.Generic.List<string>();
            Paging<SimplePlaylist> playlists = await api.GetUserPlaylistsAsync(profile.Id);
            do
            {
                playlists.Items.ForEach(playlist =>
                {
                    playlistNames.Add(playlist.Name);
                });
            } while (playlists.HasNextPage());

            callback(String.IsNullOrEmpty(profile.DisplayName) ? profile.Id : profile.DisplayName, playlistNames);
        }
    }
}
