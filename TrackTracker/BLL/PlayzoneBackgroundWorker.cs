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

            playerWorker.RunWorkerAsync();
        }
        public void Pause()
        {
            isPaused = true;
            playerWorker.CancelAsync();
        }
        public void Stop()
        {
            playerWorker.CancelAsync();
            currentlyPlaying = null;
            tracks.Clear();
        }



        private async void ConsumePlaylist(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                if (playerWorker.CancellationPending)
                {
                    e.Cancel = true;
                    if (isPaused)
                    {

                    }
                    else
                    {

                    }
                    //break;
                }
                else
                {
                    if (tracks.Count > 0)
                    {
                        currentlyPlaying = tracks.First();
                        tracks.Remove(currentlyPlaying);

                        if (currentlyPlaying.IsPlayableOffline)
                        {
                            await spotifyService.PausePlayback();

                            switch (currentlyPlaying.OfflinePlayer)
                            {
                                case SupportedMediaPlayers.Foobar2000: // TODO: not this way
                                    string playlistPath = AppDomain.CurrentDomain.BaseDirectory + "_playzone_mix.m3u";
                                    System.IO.StreamWriter sw = new System.IO.StreamWriter(playlistPath);
                                    sw.WriteLine(currentlyPlaying.FilePath);
                                    sw.Flush();
                                    sw.Close();
                                    sw.Dispose();
                                    System.Diagnostics.Process.Start(playlistPath);
                                    break;
                            }
                            //Thread.Sleep(currentlyPlaying.PlaytimeInSeconds * 1000); // TODO: not this way
                            Thread.Sleep(10000);
                        }
                        else if (currentlyPlaying.IsPlayableOnline)
                        {
                            switch (currentlyPlaying.OnlinePlayer)
                            {
                                case SupportedMediaPlayers.Spotify:
                                    await spotifyService.ChangePlaybackMusic(currentlyPlaying.SpotifyURI);
                                    break;
                            }
                            //Thread.Sleep(currentlyPlaying.PlaytimeInSeconds * 1000 + 2000); // + 2 sec for API delay // TODO: not this way
                            Thread.Sleep(12000);
                        }
                    }
                }
            }
        }
    }
}
