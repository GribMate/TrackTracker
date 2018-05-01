using AcoustID.Chromaprint;



namespace Onlab.DAL.FingerprinterUtility
{
    public abstract class AudioDecoder : IAudioDecoder
    {
        protected static readonly int BUFFER_SIZE = 2 * 192000;

        protected int sampleRate;
        protected int channels;

        public int SampleRate
        {
            get { return sampleRate; }
        }

        public int Channels
        {
            get { return channels; }
        }

        public AudioProperties Format { get; protected set; }

        public abstract bool Decode(IAudioConsumer consumer, int maxLength);

        public virtual void Dispose()
        {
        }
    }
}
