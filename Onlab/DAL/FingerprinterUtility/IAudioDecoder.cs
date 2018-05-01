using System;
using AcoustID.Audio;



namespace Onlab.DAL.FingerprinterUtility
{
    public interface IAudioDecoder : IDecoder, IDisposable
    {
        AudioProperties Format { get; }
    }
}
