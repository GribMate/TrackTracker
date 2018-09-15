using System;
using System.Threading.Tasks;

namespace Onlab.DAL.Interfaces
{
    /*
    Interface: IAcoustIDProvider
    Description:
        Handles getting AcoustID by fingerprint of a track.
    */
    public interface IAcoustIDProvider
    {
        Task<string> GetIDByFingerprint(string fingerprint, int duration);
    }
}
