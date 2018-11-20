using System;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;

using WinForms = System.Windows.Forms;

using TrackTracker.Services.Interfaces;
using TrackTracker.BLL;
using TrackTracker.BLL.Enums;



namespace TrackTracker.GUI.ViewModels
{
    public class CommonControlsViewModel : ViewModelBase
    {
        private IEnvironmentService environmentService;

        private Timer onlineStatusTimer;

        public CommonControlsViewModel() : base()
        {
            environmentService = DependencyInjector.GetService<IEnvironmentService>();

            OnlineStatus = "Checking...";
            OnlineStatusForeground = Brushes.Yellow;
            ProcessingNameVisibility = Visibility.Hidden;
            ProcessingValueVisibility = Visibility.Hidden;
            ProgressBarVisibility = Visibility.Hidden;

            onlineStatusTimer = new Timer(SetOnlinestatusUIElement, null, 3000, 5000); // Checking in every 5 sec, waiting 3 sec before first check to ensure that GUI is loaded
        }



        private string onlineStatus;
        public string OnlineStatus
        {
            get => onlineStatus;
            set
            {
                SetProperty(ref onlineStatus, value);
            }
        }

        private string processingName;
        public string ProcessingName
        {
            get => processingName;
            set
            {
                SetProperty(ref processingName, value);
            }
        }

        private string processingValue;
        public string ProcessingValue
        {
            get => processingValue;
            set
            {
                SetProperty(ref processingValue, value);
            }
        }

        private int progressBarValue;
        public int ProgressBarValue
        {
            get => progressBarValue;
            set
            {
                SetProperty(ref progressBarValue, value);
            }
        }

        private Visibility processingNameVisibility;
        public Visibility ProcessingNameVisibility
        {
            get => processingNameVisibility;
            set
            {
                SetProperty(ref processingNameVisibility, value);
            }
        }

        private Visibility processingValueVisibility;
        public Visibility ProcessingValueVisibility
        {
            get => processingValueVisibility;
            set
            {
                SetProperty(ref processingValueVisibility, value);
            }
        }

        private Visibility progressBarVisibility;
        public Visibility ProgressBarVisibility
        {
            get => progressBarVisibility;
            set
            {
                SetProperty(ref progressBarVisibility, value);
            }
        }

        private SolidColorBrush onlineStatusForeground;
        public SolidColorBrush OnlineStatusForeground
        {
            get => onlineStatusForeground;
            set
            {
                SetProperty(ref onlineStatusForeground, value);
            }
        }



        private void SetOnlinestatusUIElement(object state)
        {
            if (environmentService.InternetConnectionIsAlive()) // We have connection
            {
                OnlineStatus = "Connected";
                OnlineStatusForeground = Brushes.LawnGreen;
            }
            else // None or failed internet connection
            {
                OnlineStatus = "Disconnected";
                OnlineStatusForeground = Brushes.Red;
            }
        }



        public void SetProgressBarValue(int value, string description)
        {
            if (value > 0)
            {
                ProcessingNameVisibility = Visibility.Visible;
                ProcessingValueVisibility = Visibility.Visible;
                ProgressBarVisibility = Visibility.Visible;
            }
            else if (value == 0)
            {
                ProcessingNameVisibility = Visibility.Hidden;
                ProcessingValueVisibility = Visibility.Hidden;
                ProgressBarVisibility = Visibility.Hidden;
            }
            ProgressBarValue = value;
            ProcessingValue = description;
        }
    }
}