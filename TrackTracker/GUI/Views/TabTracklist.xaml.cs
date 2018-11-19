using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TrackTracker.BLL;
using TrackTracker.Services.Interfaces;

namespace TrackTracker.GUI.Views
{
    /// <summary>
    /// Interaction logic for TabTracklist.xaml
    /// </summary>
    public partial class TabTracklist : UserControl
    {
        private ObservableCollection<MetaTag> tags;

        private int todo_duration = -1;


        public TabTracklist()
        {
            InitializeComponent();

            tags = new ObservableCollection<MetaTag>();

            tracklist_dataGridTrackList.ItemsSource = GlobalContext.TracklistTracks;
            tracklist_dataGridTagList.ItemsSource = tags;
        }

        private void tracklist_buttonManageSources_Click(object sender, RoutedEventArgs e)
        {
            Dialogs.ManageTracklistSources mts = new Dialogs.ManageTracklistSources();
            //mts.Owner = this; //enables center-screen display  //TODO
            mts.ShowDialog();
        }
        private void tracklist_buttonSelectAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (Track track in GlobalContext.TracklistTracks)
            {
                track.IsSelectedInGUI = true;
            }
        }
        private void tracklist_buttonReverseSelection_Click(object sender, RoutedEventArgs e)
        {
            foreach (Track track in GlobalContext.TracklistTracks)
            {
                if (track.IsSelectedInGUI) track.IsSelectedInGUI = false;
                else track.IsSelectedInGUI = true;
            }
        }

        private async void tracklist_buttonSearch_Click(object sender, RoutedEventArgs e) //querying MBAPI
        {
            foreach (Track track in GlobalContext.TracklistTracks)
            {
                if (track.IsSelectedInGUI)
                {
                    List<Track> results = new List<Track>();
                    //SetProgressBarValue(25, "Querying MusicBrainz API for " + GlobalContext.FileService.GetFileNameFromFilePath(track.FileHandle.Name));  //TODO
                    results = await GetMatchesForTrack(track);
                    tracklist_dataGridMatchList.ItemsSource = results;
                    //SetProgressBarValue(75, "Querying MusicBrainz API " + GlobalContext.FileService.GetFileNameFromFilePath(track.FileHandle.Name));  //TODO
                    System.Threading.Thread.Sleep(100);
                    //SetProgressBarValue(0, " ");  //TODO

                    if (tracklist_checkBoxAutoSelect.IsChecked.Value && results.Count > 0)
                    {
                        //TODO: implement magic
                        //_id = GlobalContext.AcoustIDProvider.GetIDByFingerprint(_fingerprint, _duration).Result;
                        track.AddCandidateTrack(results[0]);
                        track.SelectActiveCandidate(results[0].MetaData.MusicBrainzTrackId);
                    }
                }
            }
        }

        private async void tracklist_buttonUpdateTags_Click(object sender, RoutedEventArgs e) //updating selected tracks
        {
            foreach (Track track in GlobalContext.TracklistTracks)
            {
                if (track.IsSelectedInGUI)
                {
                    //track.SetMetaDataFromActiveCandidate();
                    string trial_ID = await DependencyInjector.GetService<IFingerprintService>().GetIDByFingerprint(track.AcoustID, todo_duration);
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
                //en.Owner = this;  //TODO
                en.ShowDialog();
            }
        }
        private void tracklist_buttonGetFingerprint_Click(object sender, RoutedEventArgs e) //getting fingerprint for selected tracks
        {
            //SetProgressBarValue(25, "Generating fingerprints...");  //TODO
            foreach (Track track in GlobalContext.TracklistTracks)
            {
                if (track.IsSelectedInGUI)
                {
                    DependencyInjector.GetService<IFingerprintService>().GetFingerprint(track.FileHandle.Name, new Services.AcoustIDService.FingerPrintCallback(callback_fv));
                }
            }
        }
        private void callback_fv(string path, string fingerprint, int duration, Services.NAudioDecoder decoder)
        {
            this.Dispatcher.Invoke(() =>
            {
                //SetProgressBarValue(75, "Generating fingerprints for" + path); //TODO
                foreach (Track track in GlobalContext.TracklistTracks)
                {
                    if (track.FileHandle.Name == path)
                    {
                        track.AcoustID = fingerprint;
                        todo_duration = duration;
                        //string trial_ID = GlobalContext.AcoustIDProvider.GetIDByFingerprint(fingerprint, duration);
                        //TODO: duration and other non-editable metadata
                        break; //1 match
                    }
                }
                //SetProgressBarValue(0, " ");  //TODO
            });
            decoder.Dispose();
        }




        public async Task<List<Track>> GetMatchesForTrack(Track track)
        {
            List<Track> matches = new List<Track>();

            if (!String.IsNullOrEmpty(track.MetaData.MusicBrainzTrackId)) //track has an MBID, we need a lookup
            {
                Track result = await GetMatchByMBID(track.MetaData.MusicBrainzTrackId);
                matches.Add(result);
            }
            else if (!String.IsNullOrEmpty(track.MetaData.Title)) //we don't have MBID, we need a search by some metadata
            {
                matches = await GetMatchesByMetaData(track.MetaData.Title,
                    track.MetaData.JoinedAlbumArtists.Split(';').First(), //can be null
                    track.MetaData.Album); //can be null
            }
            else if (!String.IsNullOrEmpty(track.FileHandle.Name)) //we don't have MBID, nor a title, but we can try some magic from the file name
            {
                string trackName = DependencyInjector.GetService<IFileService>().GetFileNameFromFilePath(track.FileHandle.Name);
                if (trackName.Contains("-"))
                {
                    string[] splitted = trackName.Split('-');
                    if (splitted.Length == 2)
                    {
                        if (splitted[0].Length > 0 && splitted[1].Length > 0)
                        {
                            string supposedArtist = splitted[0].Trim();
                            string supposedTitle = splitted[1].Trim();
                            matches = await GetMatchesByMetaData(supposedTitle, supposedArtist);
                        }
                    }
                }
            }
            else
            {
                Dialogs.ExceptionNotification en = new Dialogs.ExceptionNotification(
                    "Track matching error",
                    "Cannot query this track against MusicBrainz, since it has no relevant metadata.",
                    "Try to use fingerprinting!");
                en.ShowDialog();
            }

            return matches;
        }
        private async Task<Track> GetMatchByMBID(string MBID)
        {
            AudioMetaData result = await DependencyInjector.GetService<IMetadataService>().GetRecordingByMBID(MBID);

            return new Track(result);
        }
        private async Task<List<Track>> GetMatchesByMetaData(string title, string artist = null, string album = null)
        {
            List<AudioMetaData> results = await DependencyInjector.GetService<IMetadataService>().GetRecordingsByMetaData(title, artist, album);

            List<Track> toReturn = new List<Track>();

            foreach (AudioMetaData result in results)
            {
                toReturn.Add(new Track(result));
            }

            return toReturn;
        }
    }
}
