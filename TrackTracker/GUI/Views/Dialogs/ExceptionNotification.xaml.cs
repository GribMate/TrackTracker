using System;
using System.Windows;

using TrackTracker.GUI.Interfaces;



namespace TrackTracker.GUI.Views.Dialogs
{
    public partial class ExceptionNotification : Window, IClosable
    {
        public ExceptionNotification()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception) { } // Do not throw exceptions from this window, since it's job is to show them
        }
    }
}
