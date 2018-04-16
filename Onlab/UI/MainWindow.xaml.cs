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

using WinForms = System.Windows.Forms; //TODO: maybe import WPFContrib v2 for redesigned FBD

namespace Onlab
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool data_fileFormatSelected;
        private bool data_driveLetterSelected;
        private WinForms.FolderBrowserDialog fbdMedia; //the FBD for the music folder path
        private ObservableCollection<MetaTag> tags;

        public MainWindow()
        {
            //GlobalVariables initalizer must run before GUI loads to avoid NullReferenceExceptions
            GlobalVariables.Initialize(); //TODO: maybe reposition global initializer @ App.xaml.cs ?

            InitializeComponent();

            hasRun = false;

            data_fileFormatSelected = false;
            data_driveLetterSelected = false;

            //load up the file format selection box with the currently supported values from ExtensionType instead of burning values in
            foreach (ExtensionType ext in Enum.GetValues(typeof(ExtensionType)).Cast<ExtensionType>()) //casting to get typed iteration, just in case
            {
                data_comboBoxFileFormat.Items.Add(ext.ToString());
            }

            fbdMedia = new WinForms.FolderBrowserDialog();
            fbdMedia.ShowNewFolderButton = false; //folder is supposed to exist already
            fbdMedia.Description = "Select local music library folder:";

            tags = new ObservableCollection<MetaTag>();
        }

        private void menuItemApplicationExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void data_radioButtonFolder_Checked(object sender, RoutedEventArgs e)
        {
            if (data_radioButtonFolder.IsInitialized) //just in case, default IsChecked throws null exception
            {
                data_textBoxOfflineFolderPath.IsEnabled = true;
                data_buttonBrowse.IsEnabled = true;
                data_comboBoxFileFormat.IsEnabled = false;
                data_comboBoxDriveLetter.IsEnabled = false;

                if (data_textBoxOfflineFolderPath.Text != "Please select your offline music folder...") data_buttonAddFiles.IsEnabled = true;
                else data_buttonAddFiles.IsEnabled = false;
            }
        }
        private void data_radioButtonDrive_Checked(object sender, RoutedEventArgs e)
        {
            if (data_radioButtonDrive.IsInitialized) //just in case, default IsChecked throws null exception
            {
                data_comboBoxFileFormat.IsEnabled = true;
                data_comboBoxDriveLetter.IsEnabled = true;
                data_textBoxOfflineFolderPath.IsEnabled = false;
                data_buttonBrowse.IsEnabled = false;

                if (data_fileFormatSelected && data_driveLetterSelected) data_buttonAddFiles.IsEnabled = true;
                else data_buttonAddFiles.IsEnabled = false;
            }
        }
        private void data_buttonBrowse_Click(object sender, RoutedEventArgs e)
        {
            if (fbdMedia.ShowDialog() == WinForms.DialogResult.OK)
            {
                if (AppIO.MediaPathIsValid(fbdMedia.SelectedPath)) //matches all criteria to validate folder
                {
                    data_textBoxOfflineFolderPath.Text = fbdMedia.SelectedPath;
                    data_buttonAddFiles.IsEnabled = true;
                } //if conditions are not met, buttonOk remains disabled
            }
        }
        private void data_buttonAddFiles_Click(object sender, RoutedEventArgs e)
        {
            LocalMediaPack lmp = null;

            if (data_radioButtonDrive.IsChecked == true)
            {
                ExtensionType type = (ExtensionType)data_comboBoxFileFormat.SelectedIndex; //will always correspond to the proper value (see constructor)
                string drive = data_comboBoxDriveLetter.SelectedItem.ToString();
                lmp = new LocalMediaPack(drive, true, true, type); //TODO: detect if removable media
                AppIO.CacheOfflineFilesFromDriveSearch(lmp, type, drive); //loading up LMP object with file paths
            }
            else if (data_radioButtonFolder.IsChecked == true)
            {
                lmp = new LocalMediaPack(data_textBoxOfflineFolderPath.Text, false, false);
                AppIO.CacheOfflineFilesFromPath(lmp, data_textBoxOfflineFolderPath.Text); //loading up LMP object with file paths
                data_buttonAddFiles.IsEnabled = false;
                data_textBoxOfflineFolderPath.Text = "Please select your offline music folder...";
            }

            GlobalVariables.Config.AddLocalMediaPack(lmp);
        }

        private void tracklist_Initialized(object sender, EventArgs e)
        {
            tracklist_dataGridTrackList.ItemsSource = GlobalVariables.TracklistData.Tracks;
            tracklist_dataGridTagList.ItemsSource = tags;
        }

        private void datasources_Initialized(object sender, EventArgs e)
        {
            //TODO: need to call only once, probably should be placed in constructor

            foreach (string driveName in AppIO.GetSystemDriveNames())
            {
                data_comboBoxDriveLetter.Items.Add(driveName);
            }
        }

        private void data_comboBoxFileFormat_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            data_fileFormatSelected = true;
            if (data_fileFormatSelected && data_driveLetterSelected) data_buttonAddFiles.IsEnabled = true;
        }

        private void data_comboBoxDriveLetter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            data_driveLetterSelected = true;
            if (data_fileFormatSelected && data_driveLetterSelected) data_buttonAddFiles.IsEnabled = true;
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

        private void playzone_buttonSelectAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (Track track in GlobalVariables.PlayzoneData.Tracks)
            {
                track.IsSelectedInGUI = true;
            }
        }

        private void playzone_buttonReverseSelection_Click(object sender, RoutedEventArgs e)
        {
            foreach (Track track in GlobalVariables.PlayzoneData.Tracks)
            {
                if (track.IsSelectedInGUI) track.IsSelectedInGUI = false;
                else track.IsSelectedInGUI = true;
            }
        }

        private void playzone_buttonAddToMix_Click(object sender, RoutedEventArgs e)
        {
            System.IO.StreamWriter sw = new System.IO.StreamWriter("D:\\testplaylist.m3u");
            foreach (Track track in GlobalVariables.PlayzoneData.Tracks)
            {
                if (track.IsSelectedInGUI) sw.WriteLine(track.FileHandle.Name);
            }
            sw.Flush();
            sw.Close();
            sw.Dispose();
            System.Diagnostics.Process.Start("D:\\testplaylist.m3u");
        }

        private void playzone_Initialized(object sender, EventArgs e)
        {
            //load up the player type selection box with the currently supported values from MediaPlayerType instead of burning values in
            foreach (MediaPlayerType ext in Enum.GetValues(typeof(MediaPlayerType)).Cast<MediaPlayerType>()) //casting to get typed iteration, just in case
            {
                playzone_comboBoxPlayerType.Items.Add(ext.ToString());
            }

            playzone_dataGridPlayList.ItemsSource = GlobalVariables.PlayzoneData.Tracks;
        }

        private bool hasRun = false;

        private void playzone_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!hasRun)
            {
                hasRun = true;
                foreach (LocalMediaPack lmp in GlobalVariables.Config.ActiveLocalMediaPacks)
                {
                    foreach (var path in lmp.GetFilePaths)
                    {
                        GlobalVariables.PlayzoneData.AddMusicFile(path.Value, path.Key);
                    }
                }
            }
        }

        private void playzone_comboBoxPlayerType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MediaPlayerType type = (MediaPlayerType)playzone_comboBoxPlayerType.SelectedIndex; //will always correspond to the proper value (see constructor)
            switch (type)
            {
                case MediaPlayerType.Foobar2000:
                    string path = AppIO.TryFindFoobar();
                    if (path.Length > 2)
                    {
                        GlobalVariables.Config.AddMediaPlayerPath(type, path);
                        playzone_textBlockPlayerLocation.Text = path + " | PLAYER LINKED!";
                    }
                    break;
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