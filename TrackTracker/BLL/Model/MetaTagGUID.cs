using System;



namespace TrackTracker.BLL.Model
{
    /*
     * Represents one single key-value pair that is associated with a metadata element of a track (can be local file or remote data),
     * where the value is a Globally Unique Identifier (GUID).
    */
    public class MetaTagGUID : MetaTagBase
    {
        public MetaTagGUID(string key, Guid? value = null) : base(key, value)
        {
            Value = value;
        }

        public new Guid? Value { get; set; }

        public override string ToString()
        {
            return (Value == null) ? null : Value.ToString();
        }
    }
}
