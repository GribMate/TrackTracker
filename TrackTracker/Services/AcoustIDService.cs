using System;
using System.IO;
using System.Threading.Tasks;
using AcoustID;
using AcoustID.Web;
using AcoustID.Chromaprint;
using AcoustID.Audio;
using NAudio.Wave;

using Onlab.Services.Interfaces;

namespace Onlab.Services
{
    /*
    Class: AcoustIDService
    Description:
        Implements IFingerprintProvider, currently using AcoustID fingerprinting service, NAudio and ChromaPrint libraries.
    */
    public class AcoustIDService : IFingerprintService
        //TODO: add proper async-await functions
    {
        public delegate void FingerPrintCallback(string path, string fingerprint, int duration, NAudioDecoder decoder);
        //TODO: event

        public bool DetectDecompressToolAvailabilty()
        {
            throw new NotImplementedException();
        }

        public bool DecompressFile(string sourceLocation, string targetLocation)
        {
            throw new NotImplementedException();
        }

        public void GetFingerprint(string filePath, FingerPrintCallback callback)
        {
            if (filePath == null) throw new ArgumentNullException();
            if (filePath.Length < 8) throw new ArgumentException(); //"C:\x.mp3" is 8 chars long
            if (!File.Exists(filePath)) throw new FileNotFoundException();

            NAudioDecoder decoder = new NAudioDecoder(filePath);

            /*
             * Various audio data for example. Might be implemented later onto GUI.
             * 
            int bits = decoder.Format.BitDepth;
            int channels = decoder.Format.Channels;
            int sampleRate = decoder.Format.SampleRate;

            string audioData = String.Format("{0}Hz, {1}bit{2}, {3}",
                decoder.Format.SampleRate, bits, bits != 16 ? " (not supported)" : "",
                channels == 2 ? "stereo" : (channels == 1 ? "mono" : "multi-channel"));
            */

            Task.Factory.StartNew(() =>
            {
                ChromaContext context = new ChromaContext();
                context.Start(decoder.SampleRate, decoder.Channels);
                decoder.Decode(context.Consumer, 120);
                context.Finish();

                callback(filePath, context.GetFingerprint(), decoder.Duration, decoder); //returning results
            });
        }

        public async Task<string> GetIDByFingerprint(string fingerprint, int duration)
        {
            Configuration.ClientKey = "JRfomg6Xqm";

            LookupService service = new LookupService();
            LookupResponse response = await service.GetAsync(fingerprint, duration, new string[] { "recordings", "compress" });

            if (!string.IsNullOrEmpty(response.ErrorMessage))
            {
                Dialogs.ExceptionNotification en = new Dialogs.ExceptionNotification("Error", "Error in AcoustID WebAPI.");
                en.ShowDialog();
            }
            else if (response.Results.Count == 0)
            {
                Dialogs.ExceptionNotification en = new Dialogs.ExceptionNotification("Empty", "No results for given fingerprint.");
                en.ShowDialog();
            }

            string resultID = "";
            double maxScore = 0;
            foreach (var result in response.Results)
            {
                if (result.Score > maxScore)
                {
                    maxScore = result.Score;
                    resultID = result.Id;
                }
            }
            return resultID;
        }
    }


    public class NAudioDecoder : IDecoder, IDisposable //TODO: not here?
    {
        private static readonly int BUFFER_SIZE = 2 * 192000;
        private WaveStream reader;
        private string file;

        public int Duration { get; private set; } //Gets the duration of the audio source (in seconds).
        public int SampleRate { get; private set; } //Gets the sample rate of the audio source.
        public int Channels { get; private set; } //Gets the number of channels.
        public int BitDepth { get; private set; } //Gets the sample rate of the audio source (must be 16 bits per sample).  

        public NAudioDecoder(string file)
        {
            this.file = file;

            // Initialization.
            // Open the WaveStream and keep it open until Dispose() is called. This might lock
            // the file. A better approach would be to open the stream only when needed.
            var extension = Path.GetExtension(file).ToLowerInvariant();

            if (extension.Equals(".wav"))
            {
                reader = new WaveFileReader(file);
            }
            else
            {
                reader = new Mp3FileReader(file);
            }

            WaveFormat format = reader.WaveFormat;

            Duration = (int)reader.TotalTime.TotalSeconds;
            SampleRate = format.SampleRate;
            Channels = format.Channels;
            BitDepth = format.BitsPerSample;

            if (format.BitsPerSample != 16)
            {
                Dispose(true);
            }
        }

        public bool Decode(IAudioConsumer consumer, int maxLength)
        {
            if (reader == null) return false;

            int remaining, length, size;
            byte[] buffer = new byte[2 * BUFFER_SIZE];
            short[] data = new short[BUFFER_SIZE];

            // Samples to read to get maxLength seconds of audio
            remaining = maxLength * Channels * SampleRate;

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

            return true;
        }


        #region IDisposable implementation

        private bool hasDisposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Dispose(bool disposing)
        {
            if (!hasDisposed)
            {
                if (reader != null)
                {
                    reader.Close();
                    reader.Dispose();
                    reader = null;
                }
            }

            hasDisposed = disposing;
        }

        ~NAudioDecoder()
        {
            Dispose(true);
        }

        #endregion
    }
}
