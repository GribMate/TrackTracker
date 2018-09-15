using System;
using System.IO;
using AcoustID.Chromaprint;
using AcoustID.Audio;
using NAudio.Wave;



namespace Onlab.DAL.FingerprinterUtility
{
    public class NAudioDecoder : IDecoder, IDisposable
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
