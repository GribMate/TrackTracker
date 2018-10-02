using System;

namespace TrackTracker.BLL.Model
{
    /*
     * Represents one single key-value pair that is associated with a metadata element of a track (can be local file or remote data).
     * Child classes implement different supported value formats.
    */
    public class MetaTagBase
    {
        public string Key { get; } // Cannot be modified after initialization and is unique for a given track
        public virtual object Value { get; set; } // Default data

        public MetaTagBase(string key, object value = null)
        {
            if (String.IsNullOrWhiteSpace(key)) throw new ArgumentNullException(nameof(key), "MetaTag key value must be initialized.");

            Key = key;
            Value = value;
        }

        public new virtual string ToString() // Default printing service by ToString() redefinement - ease of use
        {
            return (Value == null) ? null : Value.ToString();
        }
    }
}