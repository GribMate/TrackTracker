using System;

namespace Onlab.DAL.Interfaces
{
    /*
    Interface: IFLACDecompresserProvider
    Description:
        Handles decompressing FLAC files into WAV files (raw byte streams).
    */
    public interface IFLACDecompresserProvider
    {
        bool DetectToolAvailabilty(); //returns true if the external converter tool (EXE) is installed
        bool DecompressFile(string sourceLocation, string targetLocation); //decompresses a FLAC file from sourceLocation to a WAV file at targetLocation
    }
}
