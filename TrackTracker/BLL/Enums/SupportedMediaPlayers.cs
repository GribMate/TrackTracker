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
}
