using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using TrackTracker.Services.Interfaces;



namespace TrackTracker.Services
{
    /*
     * Implements ITaggingService with the TagLib# library.
    */
    public class TagLibSharpService : ITaggingService
    {
        public Dictionary<string, object> Read(string path, List<string> allowedExtensions) // Collects metadata into a general collection about a file
        {
            if (String.IsNullOrWhiteSpace(path) || path.Length < 8) // "C:\x.abc" is 8 characters long
                throw new ArgumentNullException(nameof(path), "Music file path is empty, null, or too short.");
            if (allowedExtensions == null || allowedExtensions.Count < 1)
                throw new ArgumentNullException(nameof(allowedExtensions), "Allowed extension list is null or empty.");
            if (File.Exists(path) == false)
                throw new ArgumentException("Music file path is not valid", nameof(path));

            string ext = Path.GetExtension(path).Substring(1).ToUpper(); // Omitting first char, which is a leading dot
            if (allowedExtensions.Contains(ext) == false)
                throw new ArgumentException("The file provided by the path does not have one of the allowed extensions.", nameof(allowedExtensions));

            TagLib.File audioFile = OpenAudioFile(path, ext);
            return GetMetaTagsFromAudioFile(audioFile);
        }
        public void Save(string path, Dictionary<string, object> metaTags) // Saves new tagging information to an already existing file
        {
            if (String.IsNullOrWhiteSpace(path) || path.Length < 8) // "C:\x.abc" is 8 characters long
                throw new ArgumentNullException(nameof(path), "Music file path is empty, null, or too short.");
            if (metaTags == null || metaTags.Count < 1)
                throw new ArgumentNullException(nameof(metaTags), "Collection of metatags to modify is null or empty.");
            if (File.Exists(path) == false)
                throw new ArgumentException("Music file path is not valid", nameof(path));

            string ext = Path.GetExtension(path).Substring(1).ToUpper(); // Omitting first char, which is a leading dot
            // We do not need to check for allowed extensions, since we already opened this file prior to saving, so it must be supported

            TagLib.File audioFile = OpenAudioFile(path, ext);
            SetMetaTagsInAudioFile(audioFile, metaTags);
        }

        private TagLib.File OpenAudioFile(string path, string ext) // Opens a proper file format via TagLib# and gets a file hadler object
        {
            TagLib.File audioFile = null;

            switch (ext) // Opening correct file type with TagLib#
            {
                case "MP3":
                    audioFile = new TagLib.Mpeg.AudioFile(path, TagLib.ReadStyle.Average);
                    break;
                case "FLAC":
                    audioFile = new TagLib.Flac.File(path, TagLib.ReadStyle.Average);
                    break;
                case "APE": // Generally not supported yet
                    audioFile = new TagLib.Ape.File(path, TagLib.ReadStyle.Average);
                    break;
                case "AAC": // Generally not supported yet
                    audioFile = new TagLib.Aac.File(path, TagLib.ReadStyle.Average);
                    break;
                case "AIFF": // Generally not supported yet
                    audioFile = new TagLib.Aiff.File(path, TagLib.ReadStyle.Average);
                    break;
                default:
                    audioFile = TagLib.File.Create(path);
                    break;
            }

            return audioFile;
        }
        private Dictionary<string, object> GetMetaTagsFromAudioFile(TagLib.File audioFile) // Gets all currently supported metatags
        {
            Dictionary<string, object> values = new Dictionary<string, object>();

            values.Add(nameof(audioFile.Tag.Title), audioFile.Tag.Title);
            values.Add(nameof(audioFile.Tag.Album), audioFile.Tag.Album);
            values.Add(nameof(audioFile.Tag.Copyright), audioFile.Tag.Copyright);
            values.Add(nameof(audioFile.Tag.AlbumArtists), audioFile.Tag.AlbumArtists);
            values.Add(nameof(audioFile.Tag.AlbumArtistsSort), audioFile.Tag.AlbumArtistsSort);
            values.Add(nameof(audioFile.Tag.Genres), audioFile.Tag.Genres);
            values.Add(nameof(audioFile.Tag.BeatsPerMinute), audioFile.Tag.BeatsPerMinute);
            values.Add(nameof(audioFile.Tag.Year), audioFile.Tag.Year);
            values.Add(nameof(audioFile.Tag.Track), audioFile.Tag.Track);
            values.Add(nameof(audioFile.Tag.TrackCount), audioFile.Tag.TrackCount);
            values.Add(nameof(audioFile.Tag.Disc), audioFile.Tag.Disc);
            values.Add(nameof(audioFile.Tag.DiscCount), audioFile.Tag.DiscCount);

            values.Add(nameof(audioFile.Tag.MusicBrainzReleaseArtistId), audioFile.Tag.MusicBrainzReleaseArtistId);
            values.Add(nameof(audioFile.Tag.MusicBrainzTrackId), audioFile.Tag.MusicBrainzTrackId);
            values.Add(nameof(audioFile.Tag.MusicBrainzDiscId), audioFile.Tag.MusicBrainzDiscId);
            values.Add(nameof(audioFile.Tag.MusicBrainzReleaseId), audioFile.Tag.MusicBrainzReleaseId);
            values.Add(nameof(audioFile.Tag.MusicBrainzArtistId), audioFile.Tag.MusicBrainzArtistId);

            values.Add(nameof(audioFile.Tag.MusicBrainzReleaseStatus), audioFile.Tag.MusicBrainzReleaseStatus);
            values.Add(nameof(audioFile.Tag.MusicBrainzReleaseType), audioFile.Tag.MusicBrainzReleaseType);
            values.Add(nameof(audioFile.Tag.MusicBrainzReleaseCountry), audioFile.Tag.MusicBrainzReleaseCountry);

            return values;
        }
        private void SetMetaTagsInAudioFile(TagLib.File audioFile, Dictionary<string, object> metaTags) // Sets all metatags and saves the changes to disk
        {
            // Instead of switching on strings, we use reflection to find exact matching properties for the tags
            Type tagType = audioFile.Tag.GetType();

            foreach (KeyValuePair<string, object> tag in metaTags)
            {
                PropertyInfo property = tagType.GetProperty(tag.Key); // Getting the property by its name from reflection
                Type propertyType = property.GetType(); // Getting the type of the property so we can cast to it
                property.SetValue(audioFile.Tag, Convert.ChangeType(tag.Value, propertyType)); // Setting given property with matching value
            }

            audioFile.Save(); // Persisting to disk
        }
    }
}
