using System;



namespace TrackTracker.BLL.Enums
{
    /*
     * Identifier of media players that are supported for tracklist mixing and/or playback control.
     * It is possible for a track to be playable by multiple players.
    */
    [Flags]
    public enum SupportedMediaPlayers
    {
        // Offline
        Foobar2000 = 0, // Default offline

        // Online
        Spotify = 1 // Default online
    }

    public static class SupportedMediaPlayersConverter // Provides static methods to manipulate SupportedMediaPlayers
    {
        public static SupportedMediaPlayers GetAllOfflinePlayers()
        {
            return SupportedMediaPlayers.Foobar2000;
        }
        public static SupportedMediaPlayers GetAllOnlinePlayers()
        {
            return SupportedMediaPlayers.Spotify;
        }
        public static SupportedMediaPlayers GetOfflinePlayersWhichSupportFormat(SupportedFileExtension ext)
        {
            switch (ext)
            {
                case SupportedFileExtension.MP3:
                case SupportedFileExtension.FLAC:
                case SupportedFileExtension.All:
                    return SupportedMediaPlayers.Foobar2000;
            }
            return SupportedMediaPlayers.Foobar2000; // Default offline player
        }
        public static SupportedMediaPlayers GetDefaultOfflinePlayer()
        {
            return SupportedMediaPlayers.Foobar2000;
        }
        public static SupportedMediaPlayers GetDefaultOnlinePlayer()
        {
            return SupportedMediaPlayers.Spotify;
        }
    }
}
