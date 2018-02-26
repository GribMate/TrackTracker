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

using JAudioTags;
using WinForms = System.Windows.Forms; //TODO: maybe import WPFContrib v2 for redesigned FBD

namespace Onlab
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private WinForms.FolderBrowserDialog fbdMedia; //the FBD for the music folder path

        public MainWindow()
        {
            InitializeComponent();

            data_comboBoxFileFormat.SelectedIndex = 0;
            data_comboBoxDriveLetter.SelectedIndex = 0;

            fbdMedia = new WinForms.FolderBrowserDialog();
            fbdMedia.ShowNewFolderButton = false; //folder is supposed to exist already
            fbdMedia.Description = "Select local music library folder:";
        }

        private void menuItemApplicationExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void data_radioButtonFolder_Checked(object sender, RoutedEventArgs e)
        {
            if (data_radioButtonFolder.IsInitialized) //just in case, default IsChecked throws null exception
            {
                data_textBoxOfflineFolderPath.IsEnabled = true;
                data_buttonBrowse.IsEnabled = true;
            }
        }
        private void data_radioButtonFolder_Unchecked(object sender, RoutedEventArgs e)
        {
            if (data_radioButtonFolder.IsInitialized) //just in case, default IsChecked throws null exception
            {
                data_textBoxOfflineFolderPath.IsEnabled = false;
                data_buttonBrowse.IsEnabled = false;
            }
        }
        private void data_radioButtonDrive_Checked(object sender, RoutedEventArgs e)
        {
            if (data_radioButtonDrive.IsInitialized) //just in case, default IsChecked throws null exception
            {
                data_comboBoxFileFormat.IsEnabled = true;
                data_comboBoxDriveLetter.IsEnabled = true;
            }
        }
        private void data_radioButtonDrive_Unchecked(object sender, RoutedEventArgs e)
        {
            if (data_radioButtonDrive.IsInitialized) //just in case, default IsChecked throws null exception
            {
                data_comboBoxFileFormat.IsEnabled = false;
                data_comboBoxDriveLetter.IsEnabled = false;
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
                    GlobalVariables.Config.LocalMediaPath = fbdMedia.SelectedPath;
                } //if conditions are not met, buttonOk remains disabled
            }
        }
        private void data_buttonAddFiles_Click(object sender, RoutedEventArgs e)
        {
            AppIO.CacheOfflineFiles(); //no need for argument, GlobalVariables holds the path already
            this.data_buttonAddFiles.IsEnabled = false;
            this.data_textBoxOfflineFolderPath.Text = "Please select your offline music folder...";
        }

        private void tracklist_Initialized(object sender, EventArgs e)
        {
            this.tracklist_dataGridTrackList.ItemsSource = GlobalVariables.MusicFiles;
        }

        private void datasources_Initialized(object sender, EventArgs e)
        {
            foreach (string driveName in AppIO.GetSystemDriveNames())
            {
                this.data_comboBoxDriveLetter.Items.Add(driveName);
            }
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
