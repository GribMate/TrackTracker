using System;

using TrackTracker.BLL.Enums;



namespace TrackTracker.BLL.Model
{
    public class PlayableTrackInfo
    {
        public string Title { get; set; }
        public string Artist { get; set; }
        public int PlaytimeInSeconds { get; set; }

        public bool IsPlayableOffline { get; set; }
        public SupportedMediaPlayers OfflinePlayer { get; set; }
        public string FilePath { get; set; }

        public bool IsPlayableOnline { get; set; }
        public SupportedMediaPlayers OnlinePlayer { get; set; }
        public string SpotifyURI { get; set; }
    }
}
