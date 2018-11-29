using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;



namespace TrackTracker.GUI.ViewModels.Base
{
    /*
     * Encapsulates INPC implementation, that is common for all ViewModels.
    */
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged; // Notifies the Views about every change in the VMs

        protected virtual void SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = "") // Gets called in every property's set() method, overwrites storage value (if newer) and notifies Views
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
                return;

            storage = value;
            NotifyPropertyChanged(propertyName);
        }
        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = "") // Notifies Views about a value change
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
