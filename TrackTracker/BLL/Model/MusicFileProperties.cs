using System;

using TrackTracker.BLL.Enums;

namespace TrackTracker.BLL.Model
{
    /*
     * Describes a metadata set that corresponds to a physical, local, offline music file.
    */
    public class MusicFileProperties
    {
        // Basic info for location
        public string Path { get; set; } // Direct and fully qualified path to the music file on the computer
        public bool PathIsValid { get; set; } // Is the given path still valid and alive or needs refreshing?

        // Extended info of physical file
        public string FileName { get; set; } // The file name, without the fully qualified path or the extension
        public SupportedFileExtension FileExtension { get; set; } // The file extension, typed

        // Data only available after fingerprinting was run
        public int Duration { get; set; } // Duration of the music file in seconds
        public string Fingerprint { get; set; } // Fingerprint of the music file, provided by external service


        // Additional information (only after fingerprinting)
        public int BitDepth { get; set; } // 16 bit is the only currently fully supported bit depth
        public int Channels { get; set; } // 1 (mono), 2 (stereo) or more (5.1, 7.1)
        public int SampleRate { get; set; } // Sample rate in Hz
    }
}