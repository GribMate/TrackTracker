using System;

namespace Onlab
{
    /*
    Class: AudioMetaData
    State: Under construction | DEBUG
    Description:
        Represents a music track, that can be played and has various attributes.
        Also can link to a physical file, so that file operations can be done if possible. (wraps around TagLib.AudioFile class).
        Cannot participate in inheritance.
*/
    public class AudioMetaData
    {
        //These variables correspond to JAudioTags.AudioFile properties directly (1-1 convertible to ID3 tags)
        public string Album { get; set; }
        public string Albumartist { get; set; }
        public string Artist { get; set; }
        public string Composer { get; set; }
        public uint Discnumber { get; set; }
        public string Genre { get; set; }
        public string Title { get; set; }
        public uint Tracknumber { get; set; }
        public uint Date { get; set; }
        public string Comment { get; set; }

        //These variables are more customized for TrackTracker features (do not correspond 1-1 to ID3 tags, but are derived from them)
        public string[] SeparatedArtists { get; set; } //typically 1 element, but can be more
        public DateTime TypedDate { get; set; } //typed release date
        public MusicGenre TypedGenre { get; set; } //predefined genres supported, but can be mixed as flags
        public MusicLanguage Language { get; set; } //can be mixed as well (rare case though)

        public AudioMetaData(string album, string albumartist, string artist, string composer, uint discnumber, string genre, string title,
                             uint tracknumber, uint date, string comment, bool generateCustomizedTags = false)
        {
            //no argument checking, because all tags can be null or empty strings
            Album = album;
            Albumartist = albumartist;
            Artist = artist;
            Composer = composer;
            Discnumber = discnumber;
            Genre = genre;
            Title = title;
            Tracknumber = tracknumber;
            Date = date;
            Comment = comment;

            if (generateCustomizedTags) GenerateCustomizedTags();
        }

        public AudioMetaData(TagLib.File file, bool generateCustomizedTags = false)
        {
            if (file == null) throw new ArgumentNullException();

            Album = file.Tag.Album;
            Albumartist = file.Tag.FirstAlbumArtist; //HACK
            Artist = file.Tag.FirstArtist;
            Composer = file.Tag.FirstComposer; //HACK
            Discnumber = file.Tag.Disc;
            Genre = file.Tag.FirstGenre;
            Title = file.Tag.Title;
            Tracknumber = file.Tag.Track;
            Date = file.Tag.Year;
            Comment = file.Tag.Comment;

            if (generateCustomizedTags) GenerateCustomizedTags();
        }
        private void GenerateCustomizedTags()
        {
            throw new NotImplementedException();
            //TODO
        }
    }
}