using System;
using System.Threading.Tasks;

namespace Onlab.Services.Interfaces
{
    /*
    Interface: IFingerprintService
    Description:
        Handles getting the fingerprint and AcoustID of a music file (and also decompressing FLAC files).
    */
    public interface IFingerprintService
    {
        bool DetectDecompressToolAvailabilty(); //returns true if the external converter tool (EXE) is installed
        bool DecompressFile(string sourceLocation, string targetLocation); //decompresses a FLAC file from sourceLocation to a WAV file at targetLocation

        void GetFingerprint(string filePath, AcoustIDService.FingerPrintCallback callback); //TODO: event
        Task<string> GetIDByFingerprint(string fingerprint, int duration);
    }
}
