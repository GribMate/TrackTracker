using System;
using System.Threading.Tasks;



namespace TrackTracker.Services.Interfaces
{
    public interface IFoobarService
    {
        // TODO
        string DetectFoobarPath(); // Tries to locate foobar2000 installation through various methods, returns null for no success
        Task Pause();
        Task Play();
        void MakePlaylist(string location);
        void StartPlaylist(string location);
    }
}
