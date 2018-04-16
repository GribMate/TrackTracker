using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Win32;

namespace Onlab.DAL
{
    public static class EnvironmentProvider
    {
        public static string TryFindFoobar() //tries to locate foobar2000 installation through various methods, returns null for no success
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
        public static List<string> GetSystemDriveNames()
        {
            List<string> driveNames = new List<string>();

            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                driveNames.Add(drive.Name);
            }

            return driveNames;
        }
    }
}
