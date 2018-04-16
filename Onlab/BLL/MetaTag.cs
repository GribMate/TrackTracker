using System.ComponentModel;

namespace Onlab.BLL
{
    public class MetaTag : INotifyPropertyChanged
    {
        private string name;
        private string oldValue;
        private string newValue;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Name { get => name; } //name is the ID of the tag, cannot be changed later
        public string OldValue
        {
            get => oldValue; //can be null
            set
            {
                if (oldValue != value)
                {
                    oldValue = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(OldValue)));
                }
            }
        }
        public string NewValue { get => this.newValue; } //it is generated from MusicBrainz API call, should not be changed

        public MetaTag(string name, string oldValue, string newValue)
        {
            this.name = name;
            this.oldValue = oldValue;
            this.newValue = newValue;
        }
    }
}
