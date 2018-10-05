using System;
using System.ComponentModel;



namespace TrackTracker.BLL
{
    public class MetaTag : INotifyPropertyChanged
    {
        private string name;
        private string oldValue;
        private string newValue;
        private Track ownerReference; //TODO: smells... rework

        public event PropertyChangedEventHandler PropertyChanged;

        public string Name { get => name; } //name is the ID of the tag, cannot be changed later
        public string OldValue
        {
            get => oldValue;
            set
            {
                if (oldValue != value)
                {
                    oldValue = value;
                    switch (name)
                    {
                        case "Title":                           ownerReference.MetaData.Title = value;                          break;
                        case "Album":                           ownerReference.MetaData.Album = value;                          break;
                        case "Album artists":                   ownerReference.MetaData.JoinedAlbumArtists = value;             break;
                        case "Album artists sort order":        ownerReference.MetaData.JoinedAlbumArtistsSort = value;         break;
                        case "Genres":                          ownerReference.MetaData.JoinedGenres = value;                   break;
                        case "Beats per Minute":                ownerReference.MetaData.BeatsPerMinute = UInt32.Parse(value);   break;
                        case "Copyright":                       ownerReference.MetaData.Copyright = value;                      break;
                        case "Year":                            ownerReference.MetaData.Year = UInt32.Parse(value);             break;
                        case "Track":                           ownerReference.MetaData.Track = UInt32.Parse(value);            break;
                        case "Album track count":               ownerReference.MetaData.TrackCount = UInt32.Parse(value);       break;
                        case "Disc":                            ownerReference.MetaData.Disc = UInt32.Parse(value);             break;
                        case "Album disc count":                ownerReference.MetaData.DiscCount = UInt32.Parse(value);        break;
                        case "MusicBrainz Release Artist ID":   ownerReference.MetaData.MusicBrainzReleaseArtistId = value;     break;
                        case "MusicBrainz Track ID":            ownerReference.MetaData.MusicBrainzTrackId = value;             break;
                        case "MusicBrainz Disc ID":             ownerReference.MetaData.MusicBrainzDiscId = value;              break;
                        case "MusicBrainz Release Status":      ownerReference.MetaData.MusicBrainzReleaseStatus = value;       break;
                        case "MusicBrainz Release Type":        ownerReference.MetaData.MusicBrainzReleaseType = value;         break;
                        case "MusicBrainz Release Country":     ownerReference.MetaData.MusicBrainzReleaseCountry = value;      break;
                        case "MusicBrainz Release ID":          ownerReference.MetaData.MusicBrainzReleaseId = value;           break;
                        case "MusicBrainz Artist ID":           ownerReference.MetaData.MusicBrainzArtistId = value;            break;
                    }
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(OldValue)));
                }
            }
        }
        public string NewValue { get => newValue; } //it is generated from MusicBrainz API call, should not be changed

        public MetaTag(string name, string oldValue, string newValue, Track ownerReference)
        {
            this.name = name;
            this.oldValue = oldValue;
            this.newValue = newValue;
            this.ownerReference = ownerReference;
        }
    }
}