using System;

namespace TrackTracker.BLL.Model
{
    /*
     * Represents one single key-value pair that is associated with a metadata element of a track (can be local file or remote data),
     * where the value is a collection (of strings).
    */
    public class MetaTagCollection : MetaTagBase
    {
        public static char Delimiter { get; } // Cannot be changed per instance

        // Value and JoinedValue properties are always synchronized and consistent with each other
        private string[] _value;
        public new string[] Value
        {
            get => _value;
            set
            {
                if (_value != value)  // Only compute if different
                {
                    _value = value;
                    _joinedValue = GetJoinedFromArray(value);
                }
            }
        }

        private string _joinedValue;
        public string JoinedValue
        {
            get => _joinedValue;
            set
            {
                if (_joinedValue != value) // Only compute if different
                {
                    _joinedValue = value;
                    _value = Value = GetArrayFromJoined(value);
                }
            }
        }

        static MetaTagCollection()
        {
            Delimiter = ';'; // Considered "/", but Windows uses ";" by default
        }
        public MetaTagCollection(string key, string[] value = null) : base(key, value)
        {
            Value = value;         
        }
        public MetaTagCollection(string key, string joinedValue = null) : base (key, joinedValue)
        {
            JoinedValue = joinedValue;
        }

        public new string ToString() // Joined string printing service by ToString() redefinement - ease of use
        {
            return (String.IsNullOrWhiteSpace(JoinedValue)) ? null : JoinedValue;
        }

        private string GetJoinedFromArray(string[] array)
        {
            if (array != null && array.Length > 0) // We have some value
            {
                if (array.Length == 1) // Exactly one value
                {
                    return array[0];
                }
                else // Multiple values
                {
                    string toReturn = "";
                    foreach (string item in array)
                    {
                        toReturn += item;
                        toReturn += Delimiter;
                    }
                    return toReturn.Substring(0, toReturn.Length - 1); // Remove unnecessary closing delimiter
                }
            }
            else
            {
                return null;
            }
        }
        private string[] GetArrayFromJoined(string joined)
        {
            if (!String.IsNullOrWhiteSpace(joined)) // We have some value string
            {
                return joined.Split(Delimiter); // We don't care how many values are in the joined string, since it's at least 1
            }
            else
            {
                return null;
            }
        }
    }
}