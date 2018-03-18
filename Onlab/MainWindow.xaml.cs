using System;
using System.Collections.Generic;
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

        public MainWindow()
        {
            //GlobalVariables initalizer must run before GUI loads to avoid NullReferenceExceptions
            GlobalVariables.Initialize(); //TODO: maybe reposition global initializer @ App.xaml.cs ?

            InitializeComponent();

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
                    GlobalVariables.Config.AddLocalMediaPath(fbdMedia.SelectedPath);
                } //if conditions are not met, buttonOk remains disabled
            }
        }
        private void data_buttonAddFiles_Click(object sender, RoutedEventArgs e)
        {
            if (data_radioButtonDrive.IsChecked == true)
            {
                ExtensionType type = (ExtensionType)data_comboBoxFileFormat.SelectedIndex; //will always correspond to the proper value (see constructor)
                string drive = data_comboBoxDriveLetter.SelectedItem.ToString();

                AppIO.CacheOfflineFilesFromDriveSearch(type, drive);
            }
            else if (data_radioButtonFolder.IsChecked == true)
            {
                AppIO.CacheOfflineFilesFromPath(data_textBoxOfflineFolderPath.Text);
                data_buttonAddFiles.IsEnabled = false;
                data_textBoxOfflineFolderPath.Text = "Please select your offline music folder...";
            }
        }

        private void tracklist_Initialized(object sender, EventArgs e)
        {
            tracklist_dataGridTrackList.ItemsSource = GlobalVariables.TracklistData.MusicFiles;
        }

        private void datasources_Initialized(object sender, EventArgs e)
        {
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

        }


        /*

private void buttonDetect_Click(object sender, RoutedEventArgs e)
{
    string path = AppIO.TryFindFoobar();
    if (path.Length > 2)
    {
        //textBoxFoobarPath.Text = path + " | LINKED!";
        foobarPath = path;
        GlobalVariables.Config.FoobarPath = path;
    }
} */ //TODO: Foobar2000 detect button code
    }
}