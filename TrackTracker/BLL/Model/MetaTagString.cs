using System;



namespace TrackTracker.BLL.Model
{
    /*
     * Represents one single key-value pair that is associated with a metadata element of a track (can be local file or remote data),
     * where the value is a string.
    */
    public class MetaTagString : MetaTagBase
    {
        public MetaTagString(string key, string value = null) : base(key, value)
        {
            Value = value;
        }

        public new string Value { get; set; }
    }
}