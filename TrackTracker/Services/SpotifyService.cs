using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
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
        private static AutoResetEvent loginWait = null;



        public async Task Login()
        {
            AuthorizationCodeAuth auth = new AuthorizationCodeAuth(_clientId, _secretId, "http://localhost:4002", "http://localhost:4002",
                Scope.PlaylistModifyPrivate | Scope.PlaylistModifyPublic | Scope.PlaylistReadCollaborative |
                Scope.PlaylistReadPrivate | Scope.Streaming | Scope.UserFollowModify | Scope.UserFollowRead |
                Scope.UserLibraryModify | Scope.UserLibraryRead | Scope.UserModifyPlaybackState |
                Scope.UserReadBirthdate | Scope.UserReadCurrentlyPlaying | Scope.UserReadEmail |
                Scope.UserReadPlaybackState | Scope.UserReadPrivate | Scope.UserReadRecentlyPlayed |
                Scope.UserTopRead);

            loginWait = new AutoResetEvent(false);

            auth.AuthReceived += async (sender, payload) =>
            {
                AuthorizationCodeAuth authSender = (AuthorizationCodeAuth)sender;
                auth.Stop();

                Token token = await authSender.ExchangeCode(payload.Code);
                api = new SpotifyWebAPI
                {
                    AccessToken = token.AccessToken,
                    TokenType = token.TokenType
                };

                loginWait.Set();
            };

            await Task.Factory.StartNew(() =>
             {
                 auth.Start();
                 auth.OpenBrowser();
             });

            loginWait.WaitOne();
        }

        public async Task<Dictionary<string, object>> GetAccountInformation()
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            PrivateProfile profile = await api.GetPrivateProfileAsync();
            string profileName = (String.IsNullOrWhiteSpace(profile.DisplayName)) ? profile.Id : profile.DisplayName;
            data.Add("ProfileName", profileName);
            // TODO: Maybe support more information display

            return data;
        }

        public async Task<Dictionary<string, string>> GetAllPlaylists() // Gets a collection of ID - Name pairs, representing a user's playlists
        {
            Dictionary<string, string> data = new Dictionary<string, string>();

            PrivateProfile profile = await api.GetPrivateProfileAsync();
            Paging<SimplePlaylist> playlists = await api.GetUserPlaylistsAsync(profile.Id);
            do
            {
                playlists.Items.ForEach(playlist => // Adds all currently paged data to collection
                {
                    data.Add(playlist.Id, playlist.Name);
                });
            } while (playlists.HasNextPage()); // Gets the next paged data

            return data;
        }

        public async Task<List<string>> GetPlaylistTrackIDs(string playListID)
        {
            List<string> data = new List<string>();

            PrivateProfile profile = await api.GetPrivateProfileAsync();
            Paging<SimplePlaylist> playlists = await api.GetUserPlaylistsAsync(profile.Id);
            do
            {
                playlists.Items.ForEach(playlist =>
                {
                    if (playlist.Id.Equals(playListID))
                    {
                        FullPlaylist playlistData = api.GetPlaylist(profile.Id, playlist.Id);
                        do
                        {
                            playlistData.Tracks.Items.ForEach(track =>
                            {
                                data.Add(track.Track.Id);
                            });
                        } while (playlistData.Tracks.HasNextPage());
                    }
                });
            } while (playlists.HasNextPage());

            return data;
        }

        public async Task<Dictionary<string, string>> GetTrackData(string trackID)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();

            FullTrack track = await api.GetTrackAsync(trackID);

            data.Add("TrackURI", track.Uri);
            data.Add("TrackName", track.Name);
            data.Add("FirstArtistName", track.Artists.First().Name);
            data.Add("AlbumName", track.Album.Name);

            return data;
        }

        public async Task<string> GetCurrentlyPlaying()
        {
            PlaybackContext context = await api.GetPlaybackAsync();
            if (context.Item != null)
            {
                return context.Item.Name;
            }
            else
            {
                return "No currently playing track found.";
            }
        }

        public async Task PausePlayback()
        {
            PlaybackContext context = await api.GetPlaybackAsync();

            if (context.IsPlaying)
                await api.PausePlaybackAsync();
        }

        public async Task ResumePlayback()
        {
            PlaybackContext context = await api.GetPlaybackAsync();

            if (context.IsPlaying == false)
                await api.ResumePlaybackAsync("", "", null, "", 0);
        }

        public async Task ChangePlaybackMusic(string trackURI)
        {
            PlaybackContext context = await api.GetPlaybackAsync();

            await api.ResumePlaybackAsync("", "", new List<string>() { trackURI }, "", 0);
        }
    }
}