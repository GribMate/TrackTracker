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
            ExceptionNotification enDialog = new ExceptionNotification();
            ExceptionNotificationViewModel enVM = new ExceptionNotificationViewModel(title, description, details);

            enDialog.DataContext = enVM;
            if (owner != null)
                enDialog.Owner = owner;

            enDialog.ShowDialog();
        }
    }
}
