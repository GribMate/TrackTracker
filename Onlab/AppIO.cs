using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Win32;



namespace Onlab
{
    /*
    Class: AppIO
    State: Under construction | DEBUG
    Description:
        Handles input and output operations of the application.
        Static class because it provides global functions.
    */
    public static class AppIO
    {
        private static string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData); //local machine's %appdata% folder
        private static string configFilePath = Path.Combine(appDataPath, "Onlab\\config.data"); //the full classified path of the config file
        
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
        public static bool MediaPathIsValid(string path) //determines whether the given media folder is valid
        {
            DirectoryInfo media = new DirectoryInfo(path);

            if (!media.Exists) return false;
            if (media.Attributes != FileAttributes.Directory) return false; //just in case

            return true; //none of the criteria returned false
        }
        public static void CacheOfflineFiles()
        {
            RecursiveDirectorySearch(GlobalVariables.Config.LocalMediaPath);
            
        }

        private static void RecursiveDirectorySearch(string path)
        {
            foreach (string file in Directory.GetFiles(path))
            {
                ExtensionType type;
                if (IsProperFileType(file, out type)) GlobalVariables.AddMusicFile(file, type);
            }

            foreach (string dir in Directory.GetDirectories(path))
            {
                foreach (string dirFile in Directory.GetFiles(dir))
                {
                    ExtensionType type;
                    if (IsProperFileType(dirFile, out type)) GlobalVariables.AddMusicFile(dirFile, type);
                }
                RecursiveDirectorySearch(dir);
            }
        }

        private static bool IsProperFileType(string path, out ExtensionType type)
        {
            bool isProper = true;
            type = ExtensionType.MP3; //does not matter, either gets overwritten or ignored if unsupported file

            string ext = Path.GetExtension(path);
            ext = ext.Substring(1); //getting rid of the "." before the file extension
            try { type = (ExtensionType)Enum.Parse(typeof(ExtensionType), ext, true); }
            catch (ArgumentException) { isProper = false; }

            return isProper;
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






        public static bool IsFirstRun() //determines whether it is the first run of the program by trying to locate the config file
        {
            return !File.Exists(configFilePath); //if the file does not exist, it is the first run, and vice versa
        }
        public static string TryFindMedia(out bool isLocal) //tries to locate the prestructured music folder searching removable drives, returns null for no success
        {
            string toReturn = null;
            isLocal = false;

            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (DriveInfo drive in drives)
            {
                if (!drive.IsReady) continue; //we need to skip unready devices (such as unmounted CD/DVD players)

                //only searching at top directories to avoid long search time
                //if the folder is not located there, a manual location will be neccessary
                //note: an option to search recursively is not implemented for the sake of simplicity
                string[] potentialDirs = Directory.GetDirectories(drive.Name, "Zenék", SearchOption.TopDirectoryOnly);
                if (potentialDirs.Length > 0)
                {
                    toReturn = potentialDirs[0];
                    if (drive.DriveType == DriveType.Fixed) isLocal = true;
                    break; //we just assume the first hit is correct and break the cycle
                }
            }

            return toReturn; //null, if nothing found on removable drives
        }
        public static void LoadPersistentData() //loads the persistent data | config file ---> GlobalVariables
        {
            StreamReader sr = new StreamReader(configFilePath); //using pure text-based config for the sake of simplicity

            //GlobalVariables.Config.FoobarPath = sr.ReadLine();
            GlobalVariables.Config.LocalMediaPath = sr.ReadLine();
            //GlobalVariables.Config.IsCopiedMedia = Convert.ToBoolean(sr.ReadLine());

            sr.Close();
            sr.Dispose();
        }
        public static void SavePersistentData(string foobarPath, string mediaPath, bool copied) //saves the given persistent data | arguments ---> config file
        {
            
            Directory.CreateDirectory(Path.Combine(appDataPath, "Onlab")); //creating the folder
            StreamWriter sw = new StreamWriter(configFilePath); //using pure text-based config for the sake of simplicity

            sw.WriteLine(foobarPath);
            sw.WriteLine(mediaPath);
            sw.WriteLine(copied.ToString());

            sw.Flush();
            sw.Close();
            sw.Dispose();
        }
    }
}
