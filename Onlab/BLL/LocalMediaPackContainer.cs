using System;
using System.Collections.Generic;
using System.Text;

namespace Onlab.BLL
{
    public class LocalMediaPackContainer
    {
        //TODO: error handling and extended interface

        private Dictionary<string, LocalMediaPack> addedLocalMediaPacks;
        private Dictionary<string, LocalMediaPack> activeLocalMediaPacks;

        public LocalMediaPackContainer()
        {
            addedLocalMediaPacks = new Dictionary<string, LocalMediaPack>();
            activeLocalMediaPacks = new Dictionary<string, LocalMediaPack>();
        }

        public bool AddLocalMediaPack(LocalMediaPack pack, bool writeToDB)
        {
            if (pack is null) throw new ArgumentNullException();

            if (addedLocalMediaPacks.ContainsKey(pack.RootPath)) return false; //cannot add again, already added
            else
            {
                addedLocalMediaPacks.Add(pack.RootPath, pack);

                if (writeToDB)
                {
                    string[] values = new string[5];
                    values[0] = pack.RootPath;
                    values[1] = pack.BaseExtension.ToString();
                    values[2] = Convert.ToInt32(pack.IsOnRemovableDrive).ToString(); //0 or 1
                    values[3] = Convert.ToInt32(pack.IsResultOfDriveSearch).ToString(); //0 or 1
                    StringBuilder sb = new StringBuilder();
                    foreach (string path in pack.GetFilePaths.Keys)
                    {
                        sb.Append(path);
                        sb.Append("|");
                    }
                    sb.Remove(sb.Length - 1, 1); //removing unnecessary closing "|"
                    values[4] = sb.ToString();
                    GlobalVariables.DatabaseProvider.InsertInto("LocalMediaPacks", values);
                }

                return true; //added successfully
            }
        }
        public void ActivateLocalMediaPack(string rootPath)
        {
            if (addedLocalMediaPacks.ContainsKey(rootPath) && !activeLocalMediaPacks.ContainsKey(rootPath))
            {
                LocalMediaPack toChange;
                addedLocalMediaPacks.TryGetValue(rootPath, out toChange);
                addedLocalMediaPacks.Remove(rootPath);
                activeLocalMediaPacks.Add(rootPath, toChange);

                foreach (var path in toChange.GetFilePaths)
                {
                    GlobalVariables.TracklistData.AddMusicFile(path.Value, path.Key);
                }
            }
        }
        public void DeactivateLocalMediaPack(string rootPath)
        {
            if (activeLocalMediaPacks.ContainsKey(rootPath) && !addedLocalMediaPacks.ContainsKey(rootPath))
            {
                LocalMediaPack toChange;
                activeLocalMediaPacks.TryGetValue(rootPath, out toChange);
                activeLocalMediaPacks.Remove(rootPath);
                addedLocalMediaPacks.Add(rootPath, toChange);

                foreach (var path in toChange.GetFilePaths)
                {
                    GlobalVariables.TracklistData.RemoveMusicFile(path.Key);
                }
            }
        }
        public bool TryDeleteLocalMediaPath(string path)
        {
            throw new NotImplementedException();
        }
        public List<LocalMediaPack> AddedLocalMediaPacks
        {
            get
            {
                List<LocalMediaPack> added = new List<LocalMediaPack>();
                foreach (var item in addedLocalMediaPacks)
                {
                    added.Add(item.Value);
                }
                return added;
            }
        }
        public List<LocalMediaPack> ActiveLocalMediaPacks
        {
            get
            {
                List<LocalMediaPack> active = new List<LocalMediaPack>();
                foreach (var item in activeLocalMediaPacks)
                {
                    active.Add(item.Value);
                }
                return active;
            }
        }
    }
}
