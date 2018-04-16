using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using WinForms = System.Windows.Forms;
using Onlab.BLL;



namespace Onlab
{
    public partial class MainWindow : Window
    {
        private void tracklist_Initialized(object sender, EventArgs e)
        {
            tracklist_dataGridTrackList.ItemsSource = GlobalVariables.TracklistData.Tracks;
            tracklist_dataGridTagList.ItemsSource = tags;
        }
        private void tracklist_buttonManageSources_Click(object sender, RoutedEventArgs e)
        {
            Dialogs.ManageTracklistSources mts = new Dialogs.ManageTracklistSources();
            mts.Owner = this; //enables center-screen display
            mts.ShowDialog();
        }
        private void tracklist_buttonSelectAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (Track track in GlobalVariables.TracklistData.Tracks)
            {
                track.IsSelectedInGUI = true;
            }
        }
        private void tracklist_buttonReverseSelection_Click(object sender, RoutedEventArgs e)
        {
            foreach (Track track in GlobalVariables.TracklistData.Tracks)
            {
                if (track.IsSelectedInGUI) track.IsSelectedInGUI = false;
                else track.IsSelectedInGUI = true;
            }
        }
        private void Row_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                Track selectedTrack = tracklist_dataGridTrackList.SelectedItem as Track;
                MetaBrainz.MusicBrainz.Query q = new MetaBrainz.MusicBrainz.Query("TrackTracker");

                if (selectedTrack.MetaData.Title != null)
                {

                    try
                    {
                        var releases = q.FindReleases(selectedTrack.MetaData.Title, 2);
                        List<test_MatchTableRow> rows = new List<test_MatchTableRow>();
                        foreach (var item in releases.Results)
                        {
                            string title = item.Title;
                            string artist = null;
                            if (item.ArtistCredit.Count > 0) artist = item.ArtistCredit[0].Artist.Name;
                            else artist = "Unknown";
                            string mbid = item.MbId.ToString();
                            test_MatchTableRow row = new test_MatchTableRow(artist, title, mbid);
                            rows.Add(row);
                        }
                        tracklist_dataGridMatchList.ItemsSource = rows;
                    }
                    catch (Exception exc)
                    {
                        Dialogs.ExceptionNotification en = new Dialogs.ExceptionNotification("Error while searching MusicBrainz",
                            exc.Message, "File: " + selectedTrack.MetaData.Title);
                        en.Owner = this;
                        en.ShowDialog();
                    }
                }
            }
        }
        private void tracklist_buttonUpdateTags_Click(object sender, RoutedEventArgs e)
        {
            if (tracklist_dataGridTrackList.SelectedIndex != -1 && tracklist_dataGridMatchList.SelectedIndex != -1)
            {
                Track toUpdate = tracklist_dataGridTrackList.SelectedItem as Track;
                test_MatchTableRow newMetaData = tracklist_dataGridMatchList.SelectedItem as test_MatchTableRow;
                toUpdate.MetaData.Title = newMetaData.Title;
                toUpdate.MetaData.JoinedAlbumArtists = newMetaData.Artist;
                toUpdate.MetaData.MusicBrainzReleaseId = newMetaData.MBID;
            }
        }
        private void tracklist_dataGridTrackList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Track selectedTrack = tracklist_dataGridTrackList.SelectedItem as Track;

            tags = new ObservableCollection<MetaTag>(selectedTrack.GetTags());

            tracklist_dataGridTagList.ItemsSource = tags;
        }
    }
}
