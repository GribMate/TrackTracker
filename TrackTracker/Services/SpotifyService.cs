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
        private static string _clientId = "dda143ae78d64b43a8e390d5fdbc8cce";
        private static string _secretId = "045ea52fa6d440718c900bacbd3dd4f5";

        private static SpotifyWebAPI api = null;

        public void TEST_LOGIN_PLAYLIST()
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
            PrintUsefulData();
        }

        private async void PrintUsefulData()
        {
            PrivateProfile profile = await api.GetPrivateProfileAsync();
            string name = string.IsNullOrEmpty(profile.DisplayName) ? profile.Id : profile.DisplayName;
            Console.WriteLine();
            System.Windows.MessageBox.Show($"Spotify account name: {name}", "Success!");

            Paging<SimplePlaylist> playlists = await api.GetUserPlaylistsAsync(profile.Id);
            do
            {
                playlists.Items.ForEach(playlist =>
                {
                    Console.WriteLine();
                    System.Windows.MessageBox.Show($"- {playlist.Name}", "Playlist found:");
                });
            } while (playlists.HasNextPage());
        }
    }
}
