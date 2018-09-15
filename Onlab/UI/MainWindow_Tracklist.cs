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

        private int todo_duration = -1;


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

        private async void tracklist_buttonSearch_Click(object sender, RoutedEventArgs e) //querying MBAPI
        {
            foreach (Track track in GlobalVariables.TracklistData.Tracks)
            {
                if (track.IsSelectedInGUI)
                {
                    List<Track> results = new List<Track>();
                    SetProgressBarValue(25, "Querying MusicBrainz API for " + GlobalVariables.FileProvider.GetFileNameFromFilePath(track.FileHandle.Name));
                    results = await GlobalAlgorithms.GetMatchesForTrack(track);
                    tracklist_dataGridMatchList.ItemsSource = results;
                    SetProgressBarValue(75, "Querying MusicBrainz API " + GlobalVariables.FileProvider.GetFileNameFromFilePath(track.FileHandle.Name));
                    System.Threading.Thread.Sleep(100);
                    SetProgressBarValue(0, " ");

                    if (tracklist_checkBoxAutoSelect.IsChecked.Value && results.Count > 0)
                    {
                        //TODO: implement magic
                        //_id = GlobalVariables.AcoustIDProvider.GetIDByFingerprint(_fingerprint, _duration).Result;
                        track.AddCandidateTrack(results[0]);
                        track.SelectActiveCandidate(results[0].MetaData.MusicBrainzTrackId);
                    }
                }
            }
        }

        private async void tracklist_buttonUpdateTags_Click(object sender, RoutedEventArgs e) //updating selected tracks
        {
            foreach (Track track in GlobalVariables.TracklistData.Tracks)
            {
                if (track.IsSelectedInGUI)
                {
                    //track.SetMetaDataFromActiveCandidate();
                    string trial_ID = await GlobalVariables.AcoustIDProvider.GetIDByFingerprint(track.AcoustID, todo_duration);
                    track.MetaData.Copyright = trial_ID;
                }
            }
        }
        private void tracklist_dataGridTrackList_SelectionChanged(object sender, SelectionChangedEventArgs e) //getting details from offline track
        {
            if (tracklist_dataGridTrackList.SelectedItem != null)
            {
                Track selectedTrack = tracklist_dataGridTrackList.SelectedItem as Track;

                tags = new ObservableCollection<MetaTag>(selectedTrack.GetTags());

                tracklist_dataGridTagList.ItemsSource = tags;
            }
        }
        private void tracklist_dataGridMatchList_SelectionChanged(object sender, SelectionChangedEventArgs e) //selecting active match
        {
            if (tracklist_dataGridTrackList.SelectedItem != null && tracklist_dataGridMatchList.SelectedItem != null)
            {
                Track selectedTrackOld = tracklist_dataGridTrackList.SelectedItem as Track;
                Track selectedTrackCandidate = tracklist_dataGridMatchList.SelectedItem as Track;

                selectedTrackOld.AddCandidateTrack(selectedTrackCandidate);
                selectedTrackOld.SelectActiveCandidate(selectedTrackCandidate.MetaData.MusicBrainzTrackId);

                //FOR DEBUG ONLY
                Dialogs.ExceptionNotification en = new Dialogs.ExceptionNotification("For debug", "Active candidate selected!");
                en.Owner = this;
                en.ShowDialog();
            }
        }
        private void tracklist_buttonGetFingerprint_Click(object sender, RoutedEventArgs e) //getting fingerprint for selected tracks
        {
            SetProgressBarValue(25, "Generating fingerprints...");
            foreach (Track track in GlobalVariables.TracklistData.Tracks)
            {
                if (track.IsSelectedInGUI)
                {
                    GlobalVariables.AcoustIDProvider.GetFingerprint(track.FileHandle.Name, new DAL.AcoustIDProvider.FingerPrintCallback(callback_fv));
                }
            }
        }
        private void callback_fv(string path, string fingerprint, int duration, DAL.FingerprinterUtility.NAudioDecoder decoder)
        {
            this.Dispatcher.Invoke(() =>
            {
                SetProgressBarValue(75, "Generating fingerprints for" + path);
                foreach (Track track in GlobalVariables.TracklistData.Tracks)
                {
                    if (track.FileHandle.Name == path)
                    {
                        track.AcoustID = fingerprint;
                        todo_duration = duration;
                        //string trial_ID = GlobalVariables.AcoustIDProvider.GetIDByFingerprint(fingerprint, duration);
                        //TODO: duration and other non-editable metadata
                        break; //1 match
                    }
                }
                SetProgressBarValue(0, " ");
            });
            decoder.Dispose();
        }
    }
}