using System;
using System.Threading.Tasks;

using TrackTracker.BLL.Enums;
using TrackTracker.BLL.Model.Base;
using TrackTracker.Services.Interfaces;



namespace TrackTracker.BLL.Model
{
    /*
     * Describes a metadata set that corresponds to a physical, local, offline music file.
    */
    public class MusicFileProperties : ModelObjectBase
    {
        private IFileService fileService;

        public MusicFileProperties(string path)
        {
            fileService = DependencyInjector.GetService<IFileService>();

            Path = path;
            PathIsValid = ValidatePath(path);

            FileName = fileService.GetFileNameFromFilePath(path);
            FileExtension = SupportedFileExtensionConverter.ParseExtensionString(fileService.GetExtensionFromFilePath(path));

            IsFingerprinted = false;
            Duration = -1;
            Fingerprint = null;
            BitDepth = -1;
            Channels = -1;
            SampleRate = -1;
        }

        // Basic info for location
        public string Path { get; set; } // Direct and fully qualified path to the music file on the computer
        public bool PathIsValid { get; set; } // Is the given path still valid and alive or needs refreshing?

        // Extended info of physical file
        public string FileName { get; set; } // The file name, without the fully qualified path or the extension
        public SupportedFileExtension FileExtension { get; set; } // The file extension, typed

        public bool IsFingerprinted { get; private set; } // Is the file fingerprinted already?

        // Data only available after fingerprinting was run
        public int Duration { get; set; } // Duration of the music file in seconds
        public string Fingerprint { get; set; } // Fingerprint of the music file, provided by external service

        // Additional information (only after fingerprinting)
        public int BitDepth { get; set; } // 16 bit is the only currently fully supported bit depth
        public int Channels { get; set; } // 1 (mono), 2 (stereo) or more (5.1 => 6, 7.1 => 8)
        public int SampleRate { get; set; } // Sample rate in Hz

        public void RegisterFingerprintData(string fingerprint, int duration, int bitDepth, int channels, int sampleRate)
        {
            if (String.IsNullOrWhiteSpace(fingerprint))
                throw new ArgumentNullException(nameof(fingerprint), $"Cannot set fingerprint data on file \"{Path}\", since the fingerprint is null or empty.");

            Fingerprint = fingerprint;
            Duration = duration;
            BitDepth = bitDepth;
            Channels = channels;
            SampleRate = sampleRate;

            IsFingerprinted = true;
        }

        private bool ValidatePath(string path)
        {
            if (fileService.FileExists(path))
                return true;
            else
                return false;
        }
    }
}