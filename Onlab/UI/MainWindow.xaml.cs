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

            SetOnlinestatusUIElement();
        }

        private void SetOnlinestatusUIElement()
        {
            if (GlobalAlgorithms.GetInternetState()) //we have connection
            {
                labelOnlineStatus.Content = "Connected";
                labelOnlineStatus.Foreground = System.Windows.Media.Brushes.LawnGreen;
            }
            else //no or failed internet connection
            {
                labelOnlineStatus.Content = "Disconnected";
                labelOnlineStatus.Foreground = System.Windows.Media.Brushes.Red;
            }
        }

        private void menuItemApplicationExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}