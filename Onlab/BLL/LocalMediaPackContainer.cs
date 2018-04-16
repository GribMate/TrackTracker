using System;
using System.Collections.Generic;

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

        public bool AddLocalMediaPack(LocalMediaPack pack)
        {
            if (pack is null) throw new ArgumentNullException();

            if (addedLocalMediaPacks.ContainsKey(pack.RootPath)) return false; //cannot add again, already added
            else
            {
                addedLocalMediaPacks.Add(pack.RootPath, pack);
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
