using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

using TrackTracker.BLL.Enums;
using TrackTracker.BLL.Model;
using TrackTracker.GUI.ViewModels.Dialogs;
using TrackTracker.GUI.Views.Dialogs;



namespace TrackTracker.BLL
{
    public static class UtilityHelper
    {
        public static void ShowExceptionDialog(string title, string description, string details, Window owner = null)
        {
            try
            {
                ExceptionNotification enDialog = new ExceptionNotification();
                ExceptionNotificationViewModel enVM = new ExceptionNotificationViewModel(title, description, details);

                enDialog.DataContext = enVM;
                if (owner != null)
                    enDialog.Owner = owner;

                enDialog.ShowDialog();
            }
            catch (Exception) // Do not throw exceptions from this window, since it's job is to show them
            {
                try
                {
                    // However we try a simpler method to at least tell the user that the app will close
                    MessageBox.Show("An unknown error occured during the handling of another error. The application will now exit. Sorry! :(",
                        "Unknown error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception) { } // If even that fails, nothing to do...
                finally
                {
                    Application.Current.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        Application.Current.Shutdown(); // Trying to manually close the application in a thread-safe manner
                    }));
                }
            }
        }

        public static MetaData FormatMetaData(Dictionary<string, object> source)
        {
            MetaData md = new MetaData();

            if (source.Keys.Contains("Genres"))
                md.Genres.Value = (string[])source.Select(pair => pair).Where(pair => pair.Key.Equals("Genres")).First().Value;

            if (source.Keys.Contains("Title"))
                md.Title.Value = (string)source.Select(pair => pair).Where(pair => pair.Key.Equals("Title")).First().Value;

            if (source.Keys.Contains("Album"))
                md.Album.Value = (string)source.Select(pair => pair).Where(pair => pair.Key.Equals("Album")).First().Value;

            if (source.Keys.Contains("Copyright"))
                md.Copyright.Value = (string)source.Select(pair => pair).Where(pair => pair.Key.Equals("Copyright")).First().Value;

            if (source.Keys.Contains("AlbumArtists"))
                md.AlbumArtists.Value = (string[])source.Select(pair => pair).Where(pair => pair.Key.Equals("AlbumArtists")).First().Value;

            if (source.Keys.Contains("AlbumArtistsSort"))
                md.AlbumArtistsSort.Value = (string[])source.Select(pair => pair).Where(pair => pair.Key.Equals("AlbumArtistsSort")).First().Value;

            if (source.Keys.Contains("BeatsPerMinute"))
                md.BeatsPerMinute.Value = (int?)(uint)source.Select(pair => pair).Where(pair => pair.Key.Equals("BeatsPerMinute")).First().Value;

            if (source.Keys.Contains("Year"))
                md.Year.Value = (int?)(uint)source.Select(pair => pair).Where(pair => pair.Key.Equals("Year")).First().Value;

            if (source.Keys.Contains("Track"))
                md.Track.Value = (int?)(uint)source.Select(pair => pair).Where(pair => pair.Key.Equals("Track")).First().Value;

            if (source.Keys.Contains("TrackCount"))
                md.TrackCount.Value = (int?)(uint)source.Select(pair => pair).Where(pair => pair.Key.Equals("TrackCount")).First().Value;

            if (source.Keys.Contains("Disc"))
                md.Disc.Value = (int?)(uint)source.Select(pair => pair).Where(pair => pair.Key.Equals("Disc")).First().Value;

            if (source.Keys.Contains("DiscCount"))
                md.DiscCount.Value = (int?)(uint)source.Select(pair => pair).Where(pair => pair.Key.Equals("DiscCount")).First().Value;

            if (source.Keys.Contains("DiscCount"))
                md.DiscCount.Value = (int?)(uint)source.Select(pair => pair).Where(pair => pair.Key.Equals("DiscCount")).First().Value;

            // We have to null check each GUID
            if (source.Keys.Contains("AcoustID"))
            {
                string guid = (string)source.Select(pair => pair).Where(pair => pair.Key.Equals("AcoustID")).First().Value;
                if (String.IsNullOrWhiteSpace(guid) == false)
                    md.AcoustID.Value = new Guid(guid);
            }

            if (source.Keys.Contains("MusicBrainzReleaseArtistId"))
            {
                string guid = (string)source.Select(pair => pair).Where(pair => pair.Key.Equals("MusicBrainzReleaseArtistId")).First().Value;
                if (String.IsNullOrWhiteSpace(guid) == false)
                    md.MusicBrainzReleaseArtistId.Value = new Guid(guid);
            }

            if (source.Keys.Contains("MusicBrainzTrackId"))
            {
                string guid = (string)source.Select(pair => pair).Where(pair => pair.Key.Equals("MusicBrainzTrackId")).First().Value;
                if (String.IsNullOrWhiteSpace(guid) == false)
                    md.MusicBrainzTrackId.Value = new Guid(guid);
            }

            if (source.Keys.Contains("MusicBrainzDiscId"))
            {
                string guid = (string)source.Select(pair => pair).Where(pair => pair.Key.Equals("MusicBrainzDiscId")).First().Value;
                if (String.IsNullOrWhiteSpace(guid) == false)
                    md.MusicBrainzDiscId.Value = new Guid(guid);
            }

            if (source.Keys.Contains("MusicBrainzReleaseId"))
            {
                string guid = (string)source.Select(pair => pair).Where(pair => pair.Key.Equals("MusicBrainzReleaseId")).First().Value;
                if (String.IsNullOrWhiteSpace(guid) == false)
                    md.MusicBrainzReleaseId.Value = new Guid(guid);
            }

            if (source.Keys.Contains("MusicBrainzArtistId"))
            {
                string guid = (string)source.Select(pair => pair).Where(pair => pair.Key.Equals("MusicBrainzArtistId")).First().Value;
                if (String.IsNullOrWhiteSpace(guid) == false)
                    md.MusicBrainzArtistId.Value = new Guid(guid);
            }

            if (source.Keys.Contains("MusicBrainzReleaseStatus"))
                md.MusicBrainzReleaseStatus.Value = (string)source.Select(pair => pair).Where(pair => pair.Key.Equals("MusicBrainzReleaseStatus")).First().Value;

            if (source.Keys.Contains("MusicBrainzReleaseType"))
                md.MusicBrainzReleaseType.Value = (string)source.Select(pair => pair).Where(pair => pair.Key.Equals("MusicBrainzReleaseType")).First().Value;

            if (source.Keys.Contains("MusicBrainzReleaseCountry"))
                md.MusicBrainzReleaseCountry.Value = (string)source.Select(pair => pair).Where(pair => pair.Key.Equals("MusicBrainzReleaseCountry")).First().Value;

            return md;
        }

        public static MetaData FormatMetaDataSpotify(Dictionary<string, string> source, out string trackURI)
        {
            MetaData md = new MetaData();

            md.Genres.JoinedValue = MusicGenres.Unknown.ToString();

            md.Title.Value = source.Select(pair => pair).Where(pair => pair.Key.Equals("TrackName")).First().Value;
            md.AlbumArtists.JoinedValue = source.Select(pair => pair).Where(pair => pair.Key.Equals("FirstArtistName")).First().Value;
            md.Album.Value = source.Select(pair => pair).Where(pair => pair.Key.Equals("AlbumName")).First().Value;

            trackURI = source.Select(pair => pair).Where(pair => pair.Key.Equals("TrackURI")).First().Value;

            return md;
        }

        public static TrackBase CopyTrack(TrackBase source)
        {
            TrackBase target = null;

            if (source is TrackVirtual)
            {
                TrackVirtual sourceVirtual = source as TrackVirtual;
                target = new TrackVirtual(sourceVirtual.MetaData, sourceVirtual.IsOnlyMetaData)
                {
                    SpotifyID = sourceVirtual.SpotifyID,
                    SpotifyURI = sourceVirtual.SpotifyURI
                };
            }
            else if (source is TrackLocal)
            {
                TrackLocal sourceLocal = source as TrackLocal;
                target = new TrackLocal(sourceLocal.MusicFileProperties, sourceLocal.MetaData, sourceLocal.MetaDataExtended)
                {
                    MatchCandidates = sourceLocal.MatchCandidates,
                    ActiveCandidateMBTrackID = sourceLocal.ActiveCandidateMBTrackID
                };
            }

            target.PlayableOffline = source.PlayableOffline;
            target.PlayableOnline = source.PlayableOnline;
            target.SupportedMediaPlayers = source.SupportedMediaPlayers;

            return target;
        }
    }
}
