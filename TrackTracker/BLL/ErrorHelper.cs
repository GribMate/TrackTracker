using System;
using System.Windows;

using TrackTracker.GUI.ViewModels.Dialogs;
using TrackTracker.GUI.Views.Dialogs;



namespace TrackTracker.BLL
{
    public static class ErrorHelper
    {
        public static void ShowExceptionDialog(string title, string description, string details, Window owner = null)
        {
            try
            {
                ExceptionNotification enDialog = new ExceptionNotification();
                ExceptionNotificationViewModel enVM = new ExceptionNotificationViewModel(title, description, details);

                enDialog.DataContext = enVM;
                if (owner != null)
                    enDialog.Owner = owner;

                enDialog.ShowDialog();
            }
            catch (Exception) // Do not throw exceptions from this window, since it's job is to show them
            {
                try
                {
                    // However we try a simpler method to at least tell the user that the app will close
                    MessageBox.Show("An unknown error occured during the handling of another error. The application will now exit. Sorry! :(",
                        "Unknown error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception) { } // If even that fails, nothing to do...
                finally
                {
                    Application.Current.Dispatcher.BeginInvoke((Action) (() =>
                    {
                        Application.Current.Shutdown(); // Trying to manually close the application in a thread-safe manner
                    }));
                }
            }
        }
    }
}
