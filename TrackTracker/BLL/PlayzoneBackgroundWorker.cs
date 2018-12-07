using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

using TrackTracker.BLL.Model;
using TrackTracker.BLL.Enums;
using TrackTracker.Services.Interfaces;



namespace TrackTracker.BLL
{
    public class PlayzoneBackgroundWorker
    {
        private BackgroundWorker playerWorker;
        private List<PlayableTrackInfo> tracks;
        private PlayableTrackInfo currentlyPlaying;
        private bool isPaused;

        private ISpotifyService spotifyService;
        private IFoobarService foobarService;

        public PlayzoneBackgroundWorker()
        {
            playerWorker = new BackgroundWorker();
            playerWorker.WorkerSupportsCancellation = true;
            playerWorker.DoWork += ConsumePlaylist;

            tracks = new List<PlayableTrackInfo>();
            currentlyPlaying = null;
            isPaused = false;

            spotifyService = DependencyInjector.GetService<ISpotifyService>();
            foobarService = DependencyInjector.GetService<IFoobarService>();
        }



        public void AddPlayableTrack(TrackBase source)
        {
            PlayableTrackInfo playable = new PlayableTrackInfo();

            playable.Title = source.MetaData.Title.Value;
            playable.Artist = source.MetaData.AlbumArtists.JoinedValue;

            playable.IsPlayableOffline = source.PlayableOffline;
            playable.IsPlayableOnline = source.PlayableOnline;

            if (source.PlayableOffline)
                playable.OfflinePlayer = source.SupportedMediaPlayers;
            else
                playable.OnlinePlayer = source.SupportedMediaPlayers;

            if (source is TrackVirtual && source.PlayableOnline)
            {
                TrackVirtual sourceVirtual = source as TrackVirtual;
                playable.SpotifyURI = sourceVirtual.SpotifyURI;
                playable.PlaytimeInSeconds = 10; // TODO
            }
            else if (source is TrackLocal && source.PlayableOffline)
            {
                TrackLocal sourceLocal = source as TrackLocal;
                playable.FilePath = sourceLocal.MusicFileProperties.Path;
                playable.PlaytimeInSeconds = sourceLocal.MusicFileProperties.Duration;
            }

            tracks.Add(playable);
        }

        public void Start()
        {
            if (isPaused != false)
                isPaused = false;
            else
                playerWorker.RunWorkerAsync();
        }
        public void Pause()
        {
            isPaused = true;
        }
        public void Stop()
        {
            playerWorker.CancelAsync();
            currentlyPlaying = null;
            tracks.Clear();
        }



        private async void ConsumePlaylist(object sender, DoWorkEventArgs e)
        {
            bool didCommitPause = false;

            while (true)
            {
                if (playerWorker.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }
                else if (isPaused)
                {
                    if (didCommitPause == false)
                    {
                        if (currentlyPlaying.IsPlayableOffline)
                            await foobarService.Pause();
                        else if (currentlyPlaying.IsPlayableOnline)
                            await spotifyService.PausePlayback();
                        didCommitPause = true;
                    }
                    Thread.Sleep(500);
                    continue;
                }
                else
                {
                    if (tracks.Count > 0)
                    {
                        if (currentlyPlaying != null)
                        {
                            if (currentlyPlaying.IsPlayableOffline)
                                await foobarService.Pause();
                            else if (currentlyPlaying.IsPlayableOnline)
                                await spotifyService.PausePlayback();
                        }

                        didCommitPause = false;

                        currentlyPlaying = tracks.First();
                        tracks.Remove(currentlyPlaying);

                        if (currentlyPlaying.IsPlayableOffline)
                        {
                            switch (currentlyPlaying.OfflinePlayer)
                            {
                                case SupportedMediaPlayers.Foobar2000: // TODO: maybe not this way
                                    string playlistPath = AppDomain.CurrentDomain.BaseDirectory + "_playzone_mix.m3u";
                                    System.IO.StreamWriter sw = new System.IO.StreamWriter(playlistPath);
                                    sw.WriteLine(currentlyPlaying.FilePath);
                                    sw.Flush();
                                    sw.Close();
                                    sw.Dispose();
                                    System.Diagnostics.Process.Start(playlistPath);
                                    break;
                            }

                            int sleepTime = currentlyPlaying.PlaytimeInSeconds * 1000 + 2000;
                            if (sleepTime < 12000)
                                sleepTime = 12000;

                            Thread.Sleep(sleepTime); // + 2 sec for API delay // TODO: maybe not this way
                        }
                        else if (currentlyPlaying.IsPlayableOnline)
                        {
                            switch (currentlyPlaying.OnlinePlayer)
                            {
                                case SupportedMediaPlayers.Spotify:
                                    await spotifyService.ChangePlaybackMusic(currentlyPlaying.SpotifyURI);
                                    break;
                            }

                            int sleepTime = currentlyPlaying.PlaytimeInSeconds * 1000 + 2000;
                            if (sleepTime < 12000)
                                sleepTime = 12000;

                            Thread.Sleep(sleepTime); // + 2 sec for API delay // TODO: maybe not this way
                        }
                    }
                }
            }
        }
    }
}
