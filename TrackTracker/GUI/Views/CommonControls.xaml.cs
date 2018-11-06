using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// Interaction logic for CommonControls.xaml
    /// </summary>
    public partial class CommonControls : UserControl
    {
        private Timer onlineStatusTimer;

        public CommonControls()
        {
            InitializeComponent();

            onlineStatusTimer = new Timer(SetOnlinestatusUIElement, null, 3000, 5000);
            //checking in every 5 sec, waiting 3 sec before first check to ensure that GUI is loaded
        }

        private void SetOnlinestatusUIElement(object state)
        {
            labelOnlineStatus.Dispatcher.Invoke(() =>
            {
                if (DependencyInjector.GetService<IEnvironmentService>().InternetConnectionIsAlive()) //we have connection
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
