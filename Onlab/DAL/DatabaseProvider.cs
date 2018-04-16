using System;
using System.IO;

namespace Onlab.DAL
{
    public static class DatabaseProvider
    {
        private static string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData); //local machine's %appdata% folder
        private static string configFilePath = Path.Combine(appDataPath, "Onlab\\config.data"); //the full classified path of the config file

        public static bool IsFirstRun() //determines whether it is the first run of the program by trying to locate the config file
        {
            return !File.Exists(configFilePath); //if the file does not exist, it is the first run, and vice versa
        }
        public static void LoadPersistentData() //loads the persistent data | config file ---> GlobalVariables
        {
            StreamReader sr = new StreamReader(configFilePath); //using pure text-based config for the sake of simplicity

            //GlobalVariables.Config.FoobarPath = sr.ReadLine();
            //GlobalVariables.Config.AddLocalMediaPath(sr.ReadLine());
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
