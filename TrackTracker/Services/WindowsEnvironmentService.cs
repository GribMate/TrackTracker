using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Win32;
using System.Net;

using Onlab.Services.Interfaces;



namespace Onlab.Services
{
    /*
    Class: WindowsEnvironmentService
    Description:
        Implements IEnvironmentService, currently on Windows platform via Win32 API and .NET Framework 4.6.1 calls.
    */
    public class WindowsEnvironmentService : IEnvironmentService
    {
        public bool InternetConnectionIsAlive() //returns true if the application can connect to the internet
        {
            try
            {
                using (var client = new WebClient())
                {
                    using (client.OpenRead("http://clients3.google.com/generate_204")) //"204 - No content" generator page from Google
                    {
                        return true;
                    }
                }
            }
            catch (Exception) //catching anything, since we assume that only an exception-free opening means there is a stable connection
            {
                return false;
            }
        }
        public string TryFindFoobar() //tries to locate foobar2000 installation through various methods, returns null for no success
        {
            string toReturn = null;

            RegistryKey key = Registry.ClassesRoot.OpenSubKey("Applications\\foobar2000.exe");
            if (key != null) //foobar2000 is likely to be installed, but context menus might not be enabled
            {
                if ((key = Registry.ClassesRoot.OpenSubKey("Applications\\foobar2000.exe\\shell\\open\\command")) != null) //foobar2000 is likely to be installed with context menus which give proper location
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
                    string[] potentialDirs = Directory.GetDirectories(Environment.GetEnvironmentVariable("ProgramFiles(x86)"),
                                                                      "foobar2000", SearchOption.AllDirectories);
                    if (potentialDirs.Length > 0) toReturn = potentialDirs[0]; //we just assume the first hit is correct
                }
            }
            return toReturn; //null, if nothing found inside if() blocks
        }
        public List<string> GetExternalDriveNames() //returns all attached external drive names
        {
            List<string> driveNames = new List<string>();

            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.DriveType == DriveType.Removable && drive.IsReady) driveNames.Add(drive.Name); //only adding ready, external drives
            }

            return driveNames;
        }
    }
}
