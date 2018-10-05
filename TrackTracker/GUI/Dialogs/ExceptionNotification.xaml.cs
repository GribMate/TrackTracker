using System;
using System.Windows;



namespace TrackTracker.Dialogs
{
    public partial class ExceptionNotification : Window
    {
        public ExceptionNotification(string title, string description, string details = null)
        {
            try
            {
                InitializeComponent();

                if (title != null) Title = title;
                else Title = "Unknown error";
                if (description != null) textBlockDescription.Text = description;
                else textBlockDescription.Text = "We don't know what went wrong. Please restart the application!";
                if (details != null) textBlockDetails.Text = details;
                else textBlockDetails.Text = "";
            }
            catch (Exception) { } //do not throw exceptions from this window, since it's job is to show them
        }

        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            try { Close(); }
            catch (Exception) { } //do not throw exceptions from this window, since it's job is to show them
        }
    }
}
