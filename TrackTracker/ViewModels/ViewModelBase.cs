using System;
using System.ComponentModel;
using System.Reflection;



namespace TrackTracker.ViewModels
{
    /*
     * Encapsulates INPC implementation, that is common for all ViewModels.
    */
    public class ViewModelBase : INotifyPropertyChanged
    {
        // TODO: check what's the proper way to do dis

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChangedEvent(PropertyInfo property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(property)));
        }
        protected void RaisePropertyChangedEventForAll(Type type)
        {
            PropertyInfo[] properties = type.GetProperties();
            foreach (PropertyInfo property in properties)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(property)));
            }
        }
    }
}
