using System;
using System.ComponentModel;



namespace TrackTracker.GUI.ViewModels
{
    public class ViewModelBase<TViewModel> : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChangedEvent(Func<TViewModel, object> propertySelector)
        {
            throw new NotImplementedException();
        }
        // merge commit with desktop will affect this class
    }
}
