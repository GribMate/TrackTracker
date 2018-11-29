using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;



namespace TrackTracker.BLL.Model.Base
{
    /*
     * Provides a base for model classes, encapsulating INPC interface, so that on some (hopefully rare)
     * occasions, model entities can signal the view about the change.
    */
    public class ModelObjectBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
                return;

            storage = value;
            NotifyPropertyChanged(propertyName);
        }
        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}