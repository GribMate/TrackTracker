using System;
using System.Text;
using System.ComponentModel;



namespace TrackTracker.BLL
{
    /*
    Class: AudioMetaData
    Description:
        Represents all of the currently supported ID3 tags of a music file or virtual track.
        Wraps TagLib.File.Tag properties 1-1 for GUI and comfort purposes.
*/
    public class AudioMetaData : INotifyPropertyChanged
    {
        private TagLib.File fileHandle; //TODO: rework local saving mechanism, so AudioMetaData won't depend on TagLib.File

        //========== CURRENTLY SUPPORTED ID3 TAGS ==========
        private string title;
        private string album;
        private string copyright;
        private string[] albumArtists; //used through JoinedAlbumArtists
        private string[] albumArtistsSort; //used through JoinedAlbumArtistsSort
        private string[] genres; //used through JoinedGenres
        private uint beatsPerMinute;
        private uint year;
        private uint track;
        private uint trackCount;
        private uint disc;
        private uint discCount;
        private string musicBrainzReleaseArtistId;
        private string musicBrainzTrackId;
        private string musicBrainzDiscId;
        private string musicBrainzReleaseStatus;
        private string musicBrainzReleaseType;
        private string musicBrainzReleaseCountry;
        private string musicBrainzReleaseId;
        private string musicBrainzArtistId;

        public string Title
        {
            get => title;
            set
            {
                if (title != value)
                {
                    title = value;
                    if (fileHandle != null)
                    {
                        fileHandle.Tag.Title = value;
                        fileHandle.Save();
                    }
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Title)));
                }
            }
        }
        public string Album
        {
            get => album;
            set
            {
                if (album != value)
                {
                    album = value;
                    if (fileHandle != null)
                    {
                        fileHandle.Tag.Album = value;
                        fileHandle.Save();
                    }
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Album)));
                }
            }
        }
        public string Copyright
        {
            get => copyright;
            set
            {
                if (copyright != value)
                {
                    copyright = value;
                    if (fileHandle != null)
                    {
                        fileHandle.Tag.Copyright = value;
                        fileHandle.Save();
                    }
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Copyright)));
                }
            }
        }
        public string JoinedAlbumArtists
        {
            get
            {
                if (albumArtists != null)
                {
                    StringBuilder toReturn = new StringBuilder();
                    for (int i = 0; i < albumArtists.Length; i++)
                    {
                        toReturn.Append(albumArtists[i]);
                        if (i < albumArtists.Length - 1) toReturn.Append(";");
                    }
                    return toReturn.ToString();
                }
                else return null;
            }
            set //checking if value is different might cost more resources then just raising an event
            {
                if (value != null)
                {
                    if (value.Contains(";"))
                    {
                        albumArtists = value.Split(';');
                        if (fileHandle != null)
                        {
                            fileHandle.Tag.AlbumArtists = value.Split(';');
                            fileHandle.Save();
                        }
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(JoinedAlbumArtists)));
                    }
                    else if (value.Length > 1) //still contains data - only 1 artist
                    {
                        albumArtists = new string[1];
                        albumArtists[0] = value;
                        if (fileHandle != null)
                        {
                            fileHandle.Tag.AlbumArtists = new string[1] { value };
                            fileHandle.Save();
                        }
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(JoinedAlbumArtists)));
                    }
                }
            }
        }
        public string JoinedAlbumArtistsSort
        {
            get
            {
                if (albumArtistsSort != null)
                {
                    StringBuilder toReturn = new StringBuilder();
                    for (int i = 0; i < albumArtistsSort.Length; i++)
                    {
                        toReturn.Append(albumArtistsSort[i]);
                        if (i < albumArtistsSort.Length - 1) toReturn.Append(";");
                    }
                    return toReturn.ToString();
                }
                else return null;
            }
            set //checking if value is different might cost more resources then just raising an event
            {
                if (value != null)
                {
                    if (value.Contains(";"))
                    {
                        albumArtistsSort = value.Split(';');
                        if (fileHandle != null)
                        {
                            fileHandle.Tag.AlbumArtistsSort = value.Split(';');
                            fileHandle.Save();
                        }
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(JoinedAlbumArtistsSort)));
                    }
                    else if (value.Length > 1) //still contains data - only 1 artist
                    {
                        albumArtistsSort = new string[1];
                        albumArtistsSort[0] = value;
                        if (fileHandle != null)
                        {
                            fileHandle.Tag.AlbumArtistsSort = new string[1] { value };
                            fileHandle.Save();
                        }
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(JoinedAlbumArtistsSort)));
                    }
                }
            }
        }
        public string JoinedGenres
        {
            get
            {
                if (genres != null)
                {
                    StringBuilder toReturn = new StringBuilder();
                    for (int i = 0; i < genres.Length; i++)
                    {
                        toReturn.Append(genres[i]);
                        if (i < genres.Length - 1) toReturn.Append(";");
                    }
                    return toReturn.ToString();
                }
                else return null;
            }
            set //checking if value is different might cost more resources then just raising an event
            {
                if (value != null)
                {
                    if (value.Contains(";"))
                    {
                        genres = value.Split(';');
                        if (fileHandle != null)
                        {
                            fileHandle.Tag.Genres = value.Split(';');
                            fileHandle.Save();
                        }
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(JoinedGenres)));
                    }
                    else if (value.Length > 1) //still contains data - only 1 artist
                    {
                        genres = new string[1];
                        genres[0] = value;
                        if (fileHandle != null)
                        {
                            fileHandle.Tag.Genres = new string[1] { value };
                            fileHandle.Save();
                        }
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(JoinedGenres)));
                    }
                }
            }
        }
        public uint BeatsPerMinute
        {
            get => beatsPerMinute;
            set
            {
                if (beatsPerMinute != value)
                {
                    beatsPerMinute = value;
                    if (fileHandle != null)
                    {
                        fileHandle.Tag.BeatsPerMinute = value;
                        fileHandle.Save();
                    }
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BeatsPerMinute)));
                }
            }
        }
        public uint Year
        {
            get => year;
            set
            {
                if (year != value)
                {
                    year = value;
                    if (fileHandle != null)
                    {
                        fileHandle.Tag.Year = value;
                        fileHandle.Save();
                    }
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Year)));
                }
            }
        }
        public uint Track
        {
            get => track;
            set
            {
                if (track != value)
                {
                    track = value;
                    if (fileHandle != null)
                    {
                        fileHandle.Tag.Track = value;
                        fileHandle.Save();
                    }
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Track)));
                }
            }
        }
        public uint TrackCount
        {
            get => trackCount;
            set
            {
                if (trackCount != value)
                {
                    trackCount = value;
                    if (fileHandle != null)
                    {
                        fileHandle.Tag.TrackCount = value;
                        fileHandle.Save();
                    }
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TrackCount)));
                }
            }
        }
        public uint Disc
        {
            get => disc;
            set
            {
                if (disc != value)
                {
                    disc = value;
                    if (fileHandle != null)
                    {
                        fileHandle.Tag.Disc = value;
                        fileHandle.Save();
                    }
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Disc)));
                }
            }
        }
        public uint DiscCount
        {
            get => discCount;
            set
            {
                if (discCount != value)
                {
                    discCount = value;
                    if (fileHandle != null)
                    {
                        fileHandle.Tag.DiscCount = value;
                        fileHandle.Save();
                    }
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DiscCount)));
                }
            }
        }
        public string MusicBrainzReleaseArtistId
        {
            get => musicBrainzReleaseArtistId;
            set
            {
                if (musicBrainzReleaseArtistId != value)
                {
                    musicBrainzReleaseArtistId = value;
                    if (fileHandle != null)
                    {
                        fileHandle.Tag.MusicBrainzReleaseArtistId = value;
                        fileHandle.Save();
                    }
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MusicBrainzReleaseArtistId)));
                }
            }
        }
        public string MusicBrainzTrackId
        {
            get => musicBrainzTrackId;
            set
            {
                if (musicBrainzTrackId != value)
                {
                    musicBrainzTrackId = value;
                    if (fileHandle != null)
                    {
                        fileHandle.Tag.MusicBrainzTrackId = value;
                        fileHandle.Save();
                    }
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MusicBrainzTrackId)));
                }
            }
        }
        public string MusicBrainzDiscId
        {
            get => musicBrainzDiscId;
            set
            {
                if (musicBrainzDiscId != value)
                {
                    musicBrainzDiscId = value;
                    if (fileHandle != null)
                    {
                        fileHandle.Tag.MusicBrainzDiscId = value;
                        fileHandle.Save();
                    }
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MusicBrainzDiscId)));
                }
            }
        }
        public string MusicBrainzReleaseStatus
        {
            get => musicBrainzReleaseStatus;
            set
            {
                if (musicBrainzReleaseStatus != value)
                {
                    musicBrainzReleaseStatus = value;
                    if (fileHandle != null)
                    {
                        fileHandle.Tag.MusicBrainzReleaseStatus = value;
                        fileHandle.Save();
                    }
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MusicBrainzReleaseStatus)));
                }
            }
        }
        public string MusicBrainzReleaseType
        {
            get => musicBrainzReleaseType;
            set
            {
                if (musicBrainzReleaseType != value)
                {
                    musicBrainzReleaseType = value;
                    if (fileHandle != null)
                    {
                        fileHandle.Tag.MusicBrainzReleaseType = value;
                        fileHandle.Save();
                    }
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MusicBrainzReleaseType)));
                }
            }
        }
        public string MusicBrainzReleaseCountry
        {
            get => musicBrainzReleaseCountry;
            set
            {
                if (musicBrainzReleaseCountry != value)
                {
                    musicBrainzReleaseCountry = value;
                    if (fileHandle != null)
                    {
                        fileHandle.Tag.MusicBrainzReleaseCountry = value;
                        fileHandle.Save();
                    }
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MusicBrainzReleaseCountry)));
                }
            }
        }
        public string MusicBrainzReleaseId
        {
            get => musicBrainzReleaseId;
            set
            {
                if (musicBrainzReleaseId != value)
                {
                    musicBrainzReleaseId = value;
                    if (fileHandle != null)
                    {
                        fileHandle.Tag.MusicBrainzReleaseId = value;
                        fileHandle.Save();
                    }
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MusicBrainzReleaseId)));
                }
            }
        }
        public string MusicBrainzArtistId
        {
            get => musicBrainzArtistId;
            set
            {
                if (musicBrainzArtistId != value)
                {
                    musicBrainzArtistId = value;
                    if (fileHandle != null)
                    {
                        fileHandle.Tag.MusicBrainzArtistId = value;
                        fileHandle.Save();
                    }
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MusicBrainzArtistId)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged; //for GUI

        public AudioMetaData(TagLib.File file)
        {
            if (file == null) throw new ArgumentNullException();

            fileHandle = file;

            title = file.Tag.Title;
            album = file.Tag.Album;
            copyright = file.Tag.Copyright;
            albumArtists = file.Tag.AlbumArtists;
            albumArtistsSort = file.Tag.AlbumArtistsSort;
            genres = file.Tag.Genres;
            beatsPerMinute = file.Tag.BeatsPerMinute;
            year = file.Tag.Year;
            track = file.Tag.Track;
            trackCount = file.Tag.TrackCount;
            disc = file.Tag.Disc;
            discCount = file.Tag.DiscCount;
            musicBrainzReleaseArtistId = file.Tag.MusicBrainzReleaseArtistId;
            musicBrainzTrackId = file.Tag.MusicBrainzTrackId;
            musicBrainzDiscId = file.Tag.MusicBrainzDiscId;
            musicBrainzReleaseStatus = file.Tag.MusicBrainzReleaseStatus;
            musicBrainzReleaseType = file.Tag.MusicBrainzReleaseType;
            musicBrainzReleaseCountry = file.Tag.MusicBrainzReleaseCountry;
            musicBrainzReleaseId = file.Tag.MusicBrainzReleaseId;
            musicBrainzArtistId = file.Tag.MusicBrainzArtistId;
        }
        public AudioMetaData
        (
            string title = "",
            string album = "",
            string copyright = "",
            string[] albumArtists = null,
            string[] albumArtistsSort = null,
            string[] genres = null,
            uint beatsPerMinute = 0,
            uint year = 0,
            uint track = 0,
            uint trackCount = 0,
            uint disc = 0,
            uint discCount = 0,
            string musicBrainzReleaseArtistId = "",
            string musicBrainzTrackId = "",
            string musicBrainzDiscId = "",
            string musicBrainzReleaseStatus = "",
            string musicBrainzReleaseType = "",
            string musicBrainzReleaseCountry = "",
            string musicBrainzReleaseId = "",
            string musicBrainzArtistId = ""
        )
        {
            fileHandle = null;

            //no argument checking, because all tags can be null or empty strings
            this.title = title;
            this.album = album;
            this.copyright = copyright;
            this.albumArtists = albumArtists;
            this.albumArtistsSort = albumArtistsSort;
            this.genres = genres;
            this.beatsPerMinute = beatsPerMinute;
            this.year = year;
            this.track = track;
            this.trackCount = trackCount;
            this.disc = disc;
            this.discCount = discCount;
            this.musicBrainzReleaseArtistId = musicBrainzReleaseArtistId;
            this.musicBrainzTrackId = musicBrainzTrackId;
            this.musicBrainzDiscId = musicBrainzDiscId;
            this.musicBrainzReleaseStatus = musicBrainzReleaseStatus;
            this.musicBrainzReleaseType = musicBrainzReleaseType;
            this.musicBrainzReleaseCountry = musicBrainzReleaseCountry;
            this.musicBrainzReleaseId = musicBrainzReleaseId;
            this.musicBrainzArtistId = musicBrainzArtistId;
        }
    }
}