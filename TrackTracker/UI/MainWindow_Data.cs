using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using WinForms = System.Windows.Forms;
using Onlab.BLL;
using TrackTracker.BLL.Enums;



namespace Onlab
{
    public partial class MainWindow : Window
    {
        private void datasources_Initialized(object sender, EventArgs e)
        {
            //TODO: need to call only once, probably should be placed in constructor

            foreach (string driveName in GlobalVariables.EnvironmentService.GetExternalDriveNames())
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
                if (GlobalVariables.FileService.MediaPathIsValid(fbdMedia.SelectedPath)) //matches all criteria to validate folder
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
                GlobalAlgorithms.LoadFilesFromDrive(GlobalVariables.FileService, lmp, drive, type);
            }
            else if (data_radioButtonFolder.IsChecked == true)
            {
                lmp = new LocalMediaPack(data_textBoxOfflineFolderPath.Text, false);
                GlobalAlgorithms.LoadFilesFromDirectory(GlobalVariables.FileService, lmp, data_textBoxOfflineFolderPath.Text); //loading up LMP object with file paths
                data_buttonAddFiles.IsEnabled = false;
                data_textBoxOfflineFolderPath.Text = "Please select your offline music folder...";
            }

            GlobalVariables.LocalMediaPackContainer.AddLMP(lmp, true);
        }
        private void data_buttonLinkSpotify_Click(object sender, RoutedEventArgs e)
        {
            GlobalVariables.SpotifyService.TEST_LOGIN_PLAYLIST();
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
