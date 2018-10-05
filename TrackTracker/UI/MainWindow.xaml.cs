using System;
using System.Threading;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

using WinForms = System.Windows.Forms;
using TrackTracker.BLL;
using TrackTracker.BLL.Enums;



namespace TrackTracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool data_fileFormatSelected;
        private bool data_driveLetterSelected;
        private WinForms.FolderBrowserDialog fbdMedia; //the FBD for the music folder path
        private Timer onlineStatusTimer;

        public MainWindow()
        {
            InitializeComponent();
            data_fileFormatSelected = false;
            data_driveLetterSelected = false;

            //load up the file format selection box with the currently supported values from SupportedFileExtension instead of burning values in
            foreach (SupportedFileExtension ext in Enum.GetValues(typeof(SupportedFileExtension)).Cast<SupportedFileExtension>()) //casting to get typed iteration, just in case
            {
                data_comboBoxFileFormat.Items.Add(ext.ToString());
            }

            fbdMedia = new WinForms.FolderBrowserDialog();
            fbdMedia.ShowNewFolderButton = false; //folder is supposed to exist already
            fbdMedia.Description = "Select local music library folder:";

            tags = new ObservableCollection<MetaTag>();

            onlineStatusTimer = new Timer(SetOnlinestatusUIElement, null, 3000, 5000);
            //checking in every 5 sec, waiting 3 sec before first check to ensure that GUI is loaded
        }

        private void SetOnlinestatusUIElement(object state)
        {
            labelOnlineStatus.Dispatcher.Invoke( () =>
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
            );
        }

        private void menuItemApplicationExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public void SetProgressBarValue(int value, string description)
        {
            if (value > 0)
            {
                labelProcessingName.Visibility = Visibility.Visible;
                labelProcessingValue.Visibility = Visibility.Visible;
                progressBar.Visibility = Visibility.Visible;
            }
            else if (value == 0)
            {
                labelProcessingName.Visibility = Visibility.Hidden;
                labelProcessingValue.Visibility = Visibility.Hidden;
                progressBar.Visibility = Visibility.Hidden;
            }
            progressBar.Dispatcher.Invoke(() => progressBar.Value = value, System.Windows.Threading.DispatcherPriority.Background);
            labelProcessingValue.Content = description;
        }
    }
}