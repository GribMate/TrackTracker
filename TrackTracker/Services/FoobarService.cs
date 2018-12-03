using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TrackTracker.Services.Interfaces;



namespace TrackTracker.Services
{
    public class FoobarService : IFoobarService
    {
        // TODO
        public string DetectFoobarPath()
        {
            throw new NotImplementedException();
        }

        public bool IsFoobarInstalled()
        {
            throw new NotImplementedException();
        }

        public void MakePlaylist(string location)
        {
            throw new NotImplementedException();
        }

        public void StartPlaylist(string location)
        {
            throw new NotImplementedException();
        }

        public async Task Play()
        {
            throw new NotImplementedException();
        }

        public async Task Pause()
        {
            await Task.Factory.StartNew(() =>
            {
                string foobarPath = "C:\\Program Files (x86)\\foobar2000\\foobar2000.exe";
                string pauseSwitch = "/pause";

                System.Diagnostics.Process.Start(foobarPath, pauseSwitch);
            });
        }
    }
}
