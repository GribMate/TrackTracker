using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackTracker.Services.Interfaces
{
    public interface IFoobarService
    {
        // TODO
        bool IsFoobarInstalled();
        string DetectFoobarPath();
        Task Pause();
        Task Play();
        void MakePlaylist(string location);
        void StartPlaylist(string location);
    }
}
