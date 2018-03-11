using System;

namespace Onlab
{
    /*
    Class: AudioMetaData
    State: Under construction | DEBUG
    Description:
        Represents a music track, that can be played and has various attributes.
        Also can link to a physical file, so that file operations can be done if possible. (wraps around JAudioTags.AudioFile class).
        Cannot participate in inheritance.
*/
    public class AudioMetaData
    {
        //These variables correspond to JAudioTags.AudioFile properties directly (1-1 convertible to ID3 tags)
        private string album;
        private string albumartist;
        private string artist;
        private string composer;
        private uint discnumber;
        private string genre;
        private string title;
        private uint tracknumber;
        private uint date;
        private string comment;

        //These variables are more customized for TrackTracker features (do not correspond 1-1 to ID3 tags, but are derived from them)
        private string[] separatedArtists; //typically 1 element, but can be more
        private DateTime typedDate; //typed release date
        private MusicGenre typedGenre; //predefined genres supported, but can be mixed as flags
        private MusicLanguage language; //can be mixed as well (rare case though)

        public AudioMetaData(string album, string albumartist, string artist, string composer, uint discnumber, string genre, string title,
                             uint tracknumber, uint date, string comment, bool generateCustomizedTags = false)
        {
            //no argument checking, because all tags can be null or empty strings
            this.album = album;
            this.albumartist = albumartist;
            this.artist = artist;
            this.composer = composer;
            this.discnumber = discnumber;
            this.genre = genre;
            this.title = title;
            this.tracknumber = tracknumber;
            this.date = date;
            this.comment = comment;

            if (generateCustomizedTags) GenerateCustomizedTags();
        }

        public AudioMetaData(TagLib.File file, bool generateCustomizedTags = false)
        {
            if (file == null) throw new ArgumentNullException();

            this.album = file.Tag.Album;
            this.albumartist = file.Tag.FirstAlbumArtist; //HACK
            this.artist = file.Tag.FirstArtist;
            this.composer = file.Tag.FirstComposer; //HACK
            this.discnumber = file.Tag.Disc;
            this.genre = file.Tag.FirstGenre;
            this.title = file.Tag.Title;
            this.tracknumber = file.Tag.Track;
            this.date = file.Tag.Year;
            this.comment = file.Tag.Comment;

            if (generateCustomizedTags) GenerateCustomizedTags();
        }
        private void GenerateCustomizedTags()
        {
            throw new NotFiniteNumberException();
            //TODO
        }
    }
}