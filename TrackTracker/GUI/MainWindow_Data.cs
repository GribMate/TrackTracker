using System;
using System.Windows;
using System.Windows.Controls;

using WinForms = System.Windows.Forms;
using TrackTracker.BLL;
using TrackTracker.BLL.Enums;



namespace TrackTracker
{
    public partial class MainWindow : Window
    {

        private void datasources_Initialized(object sender, EventArgs e)
        {
            //TODO: need to call only once, probably should be placed in constructor

            foreach (string driveName in GlobalAlgorithms.EnvironmentService.GetExternalDriveNames())
            {
                data_comboBoxDriveLetter.Items.Add(driveName);
            }
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
                if (GlobalAlgorithms.FileService.MediaPathIsValid(fbdMedia.SelectedPath)) //matches all criteria to validate folder
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
                SupportedFileExtension type = (SupportedFileExtension)data_comboBoxFileFormat.SelectedIndex; //will always correspond to the proper value (see constructor)
                string drive = data_comboBoxDriveLetter.SelectedItem.ToString();
                lmp = new LocalMediaPack(drive, true, type);
                GlobalAlgorithms.LoadFilesFromDrive(GlobalAlgorithms.FileService, lmp, drive, type);
            }
            else if (data_radioButtonFolder.IsChecked == true)
            {
                lmp = new LocalMediaPack(data_textBoxOfflineFolderPath.Text, false);
                GlobalAlgorithms.LoadFilesFromDirectory(GlobalAlgorithms.FileService, lmp, data_textBoxOfflineFolderPath.Text); //loading up LMP object with file paths
                data_buttonAddFiles.IsEnabled = false;
                data_textBoxOfflineFolderPath.Text = "Please select your offline music folder...";
            }

            GlobalAlgorithms.LocalMediaPackContainer.AddLMP(lmp, true);
        }
        private void data_buttonLinkSpotify_Click(object sender, RoutedEventArgs e)
        {
            GlobalAlgorithms.SpotifyService.TEST_LOGIN_PLAYLIST(new Services.SpotifyService.LoginCallback(callback_login));
        }
        private void data_SpotifyLists_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (data_SpotifyLists.SelectedIndex != -1)
                data_buttonAddSpotifyLists.IsEnabled = true;
            else
                data_buttonAddSpotifyLists.IsEnabled = false;
        }
        private void data_buttonAddSpotifyLists_Click(object sender, RoutedEventArgs e)
        {
            string selectedPlaylistName = data_SpotifyLists.SelectedItem.ToString();

            GlobalAlgorithms.SpotifyService.TEST_PLAYLISTDATA(selectedPlaylistName, new Services.SpotifyService.PlaylistCallback(callback_playlists));
        }

        private void callback_login(string name, System.Collections.Generic.List<string> playlistNames)
        {
            this.Dispatcher.Invoke(() =>
            {
                data_SpotifyName.Content = name;
                data_SpotifyCount.Content = playlistNames.Count;

                data_SpotifyLists.Items.Clear();

                foreach (string playlistName in playlistNames)
                {
                    data_SpotifyLists.Items.Add(playlistName);
                }
            });
        }

        private void callback_playlists(System.Collections.Generic.List<string> tracks)
        {
            this.Dispatcher.Invoke(() =>
            {
                string s = null;
                foreach (string track in tracks)
                {
                    s += track;
                    s += "\n";
                }
                MessageBox.Show(s, "Tracks on list are:");
            });
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
    }
}
