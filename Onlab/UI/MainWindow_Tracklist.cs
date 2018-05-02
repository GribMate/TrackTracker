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
        private ObservableCollection<MetaTag> tags;


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
        private async void Row_DoubleClick(object sender, MouseButtonEventArgs e)
        {     
            if (e.ChangedButton == MouseButton.Left)
            {
                SetProgressBarValue(1, "Querying MusicBrainz API...");
                Track selectedTrack = tracklist_dataGridTrackList.SelectedItem as Track;
                SetProgressBarValue(25, "Querying MusicBrainz API...");
                tracklist_dataGridMatchList.ItemsSource = await GlobalAlgorithms.GetMatchesForTrack(selectedTrack);
                SetProgressBarValue(75, "Querying MusicBrainz API...");
                System.Threading.Thread.Sleep(250);
                SetProgressBarValue(100, "Querying MusicBrainz API...");
                System.Threading.Thread.Sleep(250);
                SetProgressBarValue(0, "Querying MusicBrainz API...");
            }
        }
        private void tracklist_buttonUpdateTags_Click(object sender, RoutedEventArgs e)
        {
            foreach (Track track in GlobalVariables.TracklistData.Tracks)
            {
                if (track.IsSelectedInGUI)
                {
                    track.SetMetaDataFromActiveCandidate();
                }
            }
        }
        private void tracklist_dataGridTrackList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tracklist_dataGridTrackList.SelectedItem != null)
            {
                Track selectedTrack = tracklist_dataGridTrackList.SelectedItem as Track;

                tags = new ObservableCollection<MetaTag>(selectedTrack.GetTags());

                tracklist_dataGridTagList.ItemsSource = tags;
            }
        }
        private void tracklist_dataGridMatchList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tracklist_dataGridTrackList.SelectedItem != null && tracklist_dataGridMatchList.SelectedItem != null)
            {
                Track selectedTrackOld = tracklist_dataGridTrackList.SelectedItem as Track;
                Track selectedTrackCandidate = tracklist_dataGridMatchList.SelectedItem as Track;

                selectedTrackOld.AddCandidateTrack(selectedTrackCandidate);
                selectedTrackOld.SelectActiveCandidate(selectedTrackCandidate.MetaData.MusicBrainzTrackId);
            }
        }
    }
}
