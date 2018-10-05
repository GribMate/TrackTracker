using System;
using System.ComponentModel;



namespace TrackTracker.BLL
{
    public class StatisticalData : INotifyPropertyChanged
    {
        private string name;
        private uint count;

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                RaisePropertyChangeEvent("Name"); //INPC
            }
        }
        public uint Count
        {
            get { return count; }
            set
            {
                count = value;
                RaisePropertyChangeEvent("Count"); //INPC
            }
        }

        #region INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChangeEvent(String propertyName)
        {
            if (PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));

        }
        #endregion
    }
}
