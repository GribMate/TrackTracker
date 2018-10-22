using System;
using System.ComponentModel;
using System.Reflection;



namespace TrackTracker.GUI.ViewModels
{
    /*
     * Encapsulates INPC implementation, that is common for all ViewModels.
    */
    public class ViewModelBase<TViewModel> : INotifyPropertyChanged
    {
        private TViewModel VM;

        public event PropertyChangedEventHandler PropertyChanged;

        // TODO: check what's the proper way to do dis

        protected void RaisePropertyChangedEvent(Func<TViewModel, object> propertySelector)
        {
            object property = propertySelector(VM);
            PropertyChanged?.Invoke(VM, new PropertyChangedEventArgs(nameof(property)));
        }
        protected void RaisePropertyChangedEventForAll()
        {
            PropertyInfo[] properties = VM.GetType().GetProperties();
            foreach (PropertyInfo property in properties)
            {
                PropertyChanged?.Invoke(VM, new PropertyChangedEventArgs(nameof(property)));
            }
        }
    }
}
