using System;
using System.IO;
using System.Collections.Generic;
using System.Net;

using TrackTracker.Services.Interfaces;



namespace TrackTracker.Services
{
    /*
    Class: WindowsEnvironmentService
    Description:
        Implements IEnvironmentService, currently on Windows platform via Win32 API and .NET Framework 4.6.1 calls.
    */
    public class WindowsEnvironmentService : IEnvironmentService
    {
        private const string GOOGLE_204_ADDRESS = "http://clients3.google.com/generate_204";

        public bool InternetConnectionIsAlive() //returns true if the application can connect to the internet
        {
            try
            {
                using (var client = new WebClient())
                {
                    using (client.OpenRead(GOOGLE_204_ADDRESS)) // "204 - No content" generator page from Google
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