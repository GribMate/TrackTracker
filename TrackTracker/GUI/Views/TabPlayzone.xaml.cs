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
using TrackTracker.BLL;
using TrackTracker.BLL.Enums;

namespace TrackTracker.GUI.Views
{
    /// <summary>
    /// Interaction logic for TabPlayzone.xaml
    /// </summary>
    public partial class TabPlayzone : UserControl
    {
        public TabPlayzone()
        {
            InitializeComponent();

            //TODO: after refactor this will not work this way
            //load up the player type selection box with the currently supported values from MediaPlayerType instead of burning values in
            foreach (SupportedMediaPlayers ext in Enum.GetValues(typeof(SupportedMediaPlayers)).Cast<SupportedMediaPlayers>()) //casting to get typed iteration, just in case
            {
                playzone_comboBoxPlayerType.Items.Add(ext.ToString());
            }

            playzone_dataGridPlayList.ItemsSource = GlobalAlgorithms.PlayzoneData.Tracks;
        }

        private void playzone_buttonSelectAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (Track track in GlobalAlgorithms.PlayzoneData.Tracks)
            {
                track.IsSelectedInGUI = true;
            }
        }
        private void playzone_buttonReverseSelection_Click(object sender, RoutedEventArgs e)
        {
            foreach (Track track in GlobalAlgorithms.PlayzoneData.Tracks)
            {
                if (track.IsSelectedInGUI) track.IsSelectedInGUI = false;
                else track.IsSelectedInGUI = true;
            }
        }
        private void playzone_buttonAddToMix_Click(object sender, RoutedEventArgs e)
        {
            System.IO.StreamWriter sw = new System.IO.StreamWriter("D:\\testplaylist.m3u");
            foreach (Track track in GlobalAlgorithms.PlayzoneData.Tracks)
            {
                if (track.IsSelectedInGUI) sw.WriteLine(track.FileHandle.Name);
            }
            sw.Flush();
            sw.Close();
            sw.Dispose();
            System.Diagnostics.Process.Start("D:\\testplaylist.m3u");
        }

        private void playzone_comboBoxPlayerType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SupportedMediaPlayers type = (SupportedMediaPlayers)playzone_comboBoxPlayerType.SelectedIndex; //will always correspond to the proper value (see constructor)
            switch (type)
            {
                case SupportedMediaPlayers.Foobar2000:
                    string path = GlobalAlgorithms.EnvironmentService.TryFindFoobar();
                    if (path.Length > 2)
                    {
                        GlobalAlgorithms.AppConfig.AddMediaPlayerPath(type, path);
                        playzone_textBlockPlayerLocation.Text = path + " | PLAYER LINKED!";
                        playzone_buttonAddToMix.IsEnabled = true;
                    }
                    break;
            }
        }
        private async void playzone_buttonSpotifyPlayPause_Click(object sender, RoutedEventArgs e)
        {
            playzone_textBlockPlayerLocation.Text = await GlobalAlgorithms.SpotifyService.TEST_PLAYING();

            GlobalAlgorithms.SpotifyService.TEST_PLAY_PAUSE();
        }
    }
}
