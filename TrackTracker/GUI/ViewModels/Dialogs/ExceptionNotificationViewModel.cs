using System;
using System.Windows.Input;

using TrackTracker.GUI.Interfaces;
using TrackTracker.GUI.ViewModels.Base;



namespace TrackTracker.GUI.ViewModels.Dialogs
{
    public class ExceptionNotificationViewModel : ViewModelBase
    {
        public ExceptionNotificationViewModel(string title, string description, string details = null)
        {
            CloseCommand = new RelayCommand<IClosable>(exe => ExecuteClose(exe), can => CanExecuteClose);


            if (String.IsNullOrWhiteSpace(title))
                Title = "Unknown error";
            else
                Title = title;

            if (String.IsNullOrWhiteSpace(description))
                Description = "We don't know what went wrong. Please restart the application!";
            else
                Description = description;

            if (String.IsNullOrWhiteSpace(details))
                Details = "";
            else
                Details = details;
        }



        public ICommand CloseCommand { get; }



        private string title;
        public string Title
        {
            get => title;
            private set { SetProperty(ref title, value); }
        }

        private string description;
        public string Description
        {
            get => description;
            private set { SetProperty(ref description, value); }
        }

        private string details;
        public string Details
        {
            get => details;
            private set { SetProperty(ref details, value); }
        }



        private bool CanExecuteClose
        {
            get => true;
        }
        private void ExecuteClose(IClosable window)
        {
            if (window != null)
                window.Close();
        }
    }
}
