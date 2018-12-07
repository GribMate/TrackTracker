using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;

using AcoustID;
using AcoustID.Web;
using AcoustID.Chromaprint;
using AcoustID.Audio;
using NAudio.Wave;

using TrackTracker.Services.Interfaces;



namespace TrackTracker.Services
{
    /*
     * Implements IFingerprintProvider, currently using AcoustID fingerprinting service, NAudio and ChromaPrint libraries.
    */
    public class AcoustIDService : IFingerprintService
    {
        private const string FLAC_TOOL_NAME = "flac.exe";
        private const string FLAC_TOOL_DECODE = "-d";

        private static readonly int BUFFER_SIZE = 2 * 192000;
        private WaveStream reader;

        private string fingerprint;
        private int duration; // The duration of the audio source (in seconds)
        private int bitDepth; // The sample rate of the audio source (must be 16 bits per sample)
        private int channels; // The number of channels
        private int sampleRate; // The sample rate of the audio source

        private bool dataReady;

        public AcoustIDService()
        {
            reader = null;
            fingerprint = null;
            duration = -1;
            bitDepth = -1;
            channels = -1;
            sampleRate = -1;

            dataReady = false;
        }

        public async Task RunFingerprinting(string filePath)
        {
            if (filePath == null)
                throw new ArgumentNullException(nameof(filePath), $"Cannot fingerprint audio file, since its path is null.");
            if (filePath.Length < 8) // "C:\x.abc" is 8 characters long
                throw new ArgumentException($"Cannot fingerprint audio file, since its path is too short.", nameof(filePath));
            if (File.Exists(filePath) == false)
                throw new FileNotFoundException($"Cannot fingerprint audio file, since it does not exist on the given path of {filePath}.", nameof(filePath));

            dataReady = false;

            await Task.Factory.StartNew(() =>
            {
                InitReader(filePath);

                ChromaContext context = new ChromaContext();
                context.Start(sampleRate, channels);
                Decode(context.Consumer, 120);
                context.Finish();

                fingerprint = context.GetFingerprint();

                DisposeReader();

                dataReady = true;
            });
        }

        public Dictionary<string, object> GetDataOfLastRun()
        {
            if (dataReady == false)
                throw new InvalidOperationException($"Cannot get results, since the last fingerprinting was corrupted or did not finish yet.");

            Dictionary<string, object> results = new Dictionary<string, object>();
            results.Add("Fingerprint", fingerprint);
            results.Add("Duration", duration);
            results.Add("BitDepth", bitDepth);
            results.Add("Channels", channels);
            results.Add("SampleRate", sampleRate);

            return results;
        }

        public async Task<Dictionary<string, Guid>> GetIDsByFingerprint(string fingerprint, int duration)
        {
            Configuration.ClientKey = "JRfomg6Xqm";

            LookupService service = new LookupService();
            LookupResponse response = await service.GetAsync(fingerprint, duration, new string[] { "recordings", "compress" });

            // TODO: not here

            //if (!string.IsNullOrEmpty(response.ErrorMessage))
            //{
            //    Dialogs.ExceptionNotification en = new Dialogs.ExceptionNotification("Error", "Error in AcoustID WebAPI.");
            //    en.ShowDialog();
            //}
            //else if (response.Results.Count == 0)
            //{
            //    Dialogs.ExceptionNotification en = new Dialogs.ExceptionNotification("Empty", "No results for given fingerprint.");
            //    en.ShowDialog();
            //}

            Guid resultAcoustID = Guid.Empty;
            Guid resultMBID = Guid.Empty;
            double maxScore = 0;
            foreach (var result in response.Results)
            {
                if (result.Score > maxScore)
                {
                    maxScore = result.Score;
                    resultAcoustID = new Guid(result.Id);
                    resultMBID = new Guid(result.Recordings.First().Id);
                }
            }

            Dictionary<string, Guid> toReturn = new Dictionary<string, Guid>();
            toReturn.Add("AcoustID", resultAcoustID);
            toReturn.Add("MusicBrainzTrackId", resultMBID);

            return toReturn;
        }

        public bool DetectDecompressToolAvailabilty()
        {
            string supposedToolPath = AppDomain.CurrentDomain.BaseDirectory + "flac.exe";
            return File.Exists(supposedToolPath);
        }

        public string DecompressFile(string sourceFile)
        {
            System.Diagnostics.Process.Start(AppDomain.CurrentDomain.BaseDirectory + FLAC_TOOL_NAME, FLAC_TOOL_DECODE + " \"" + sourceFile + "\"");

            System.Threading.Thread.Sleep(5000); // TODO: Last minute bugfix, solve later

            return sourceFile.Substring(0, sourceFile.Length - 5) + ".wav";
        }



        private void InitReader(string filePath)
        {
            // Open the WaveStream and keep it open until Dispose() is called, this might lock the file
            // A better approach would be to open the stream only when needed
            string extension = Path.GetExtension(filePath).ToUpper().Substring(1);

            if (extension.Equals("WAV"))
                reader = new WaveFileReader(filePath);

            else if (extension.Equals("MP3"))
                reader = new Mp3FileReader(filePath);

            WaveFormat format = reader.WaveFormat;

            duration = (int)reader.TotalTime.TotalSeconds;
            sampleRate = format.SampleRate;
            channels = format.Channels;
            bitDepth = format.BitsPerSample;

            if (format.BitsPerSample != 16)
            {
                DisposeReader();
                throw new NotSupportedException($"Cannot fingerprint given file. The file must be 16 bits per sample!");
            }
        }

        private void Decode(IAudioConsumer consumer, int maxLength)
        {
            int remaining, length, size;
            byte[] buffer = new byte[2 * BUFFER_SIZE];
            short[] data = new short[BUFFER_SIZE];

            // Samples to read to get maxLength seconds of audio
            remaining = maxLength * channels * sampleRate;

            // Bytes to read
            length = 2 * Math.Min(remaining, BUFFER_SIZE);

            while ((size = reader.Read(buffer, 0, length)) > 0)
            {
                Buffer.BlockCopy(buffer, 0, data, 0, size);

                consumer.Consume(data, size / 2);

                remaining -= size / 2;
                if (remaining <= 0)
                {
                    break;
                }

                length = 2 * Math.Min(remaining, BUFFER_SIZE);
            }
        }

        private void DisposeReader()
        {
            if (reader != null)
            {
                reader.Close();
                reader.Dispose();
                reader = null;
            }
        }
    }
}
