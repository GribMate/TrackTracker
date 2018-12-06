using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Win32;

using TrackTracker.Services.Interfaces;



namespace TrackTracker.Services
{
    public class FoobarService : IFoobarService
    {
        private const string FOOBAR_SUBKEY = "Applications\\foobar2000.exe";
        private const string FOOBAR_CONTEXT_KEY = "Applications\\foobar2000.exe\\shell\\open\\command";
        private const string PROGRAMFILES_X86 = "ProgramFiles(x86)";
        private const string FOOBAR_NAME = "foobar2000";


        // TODO
        public string DetectFoobarPath()
        {
            string toReturn = null;

            RegistryKey key = Registry.ClassesRoot.OpenSubKey(FOOBAR_SUBKEY);
            if (key != null) //foobar2000 is likely to be installed, but context menus might not be enabled
            {
                if ((key = Registry.ClassesRoot.OpenSubKey(FOOBAR_CONTEXT_KEY)) != null) //foobar2000 is likely to be installed with context menus which give proper location
                {
                    Object val = key.GetValue(null); //specifying null for "(Default)" named value to be retrieved
                    if (val != null) //yaay, we have a context menu entry!
                    {
                        string path = val as string; //just in case, because the object is a REG_SZ
                        path = path.Substring(1, path.Length - 22); //getting rid of the opening and closing junk
                        //4 <"> symbol | %1 string | 1 whitspace | 1 backslash | foobar2000.exe string
                        //      4      +     2     +      1      +      1      +          14           =    22
                        toReturn = path; //done, we can safely assume foobar2000 will be there
                    }
                }
                else //there is no context menu entry, but there is a foobar2000.exe key, so we have to dig deeper...
                {
                    string[] potentialDirs = Directory.GetDirectories(Environment.GetEnvironmentVariable(PROGRAMFILES_X86),
                                                                      FOOBAR_NAME, SearchOption.AllDirectories);
                    if (potentialDirs.Length > 0) toReturn = potentialDirs[0]; //we just assume the first hit is correct
                }
            }
            return toReturn; //null, if nothing found inside if() blocks
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
