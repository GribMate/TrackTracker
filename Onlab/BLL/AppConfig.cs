using System;
using System.Collections.Generic;

namespace Onlab.BLL
{
    public class AppConfig
    {
        //TODO: error handling and extended interface

        private Dictionary<MediaPlayerType, string> mediaPlayerPaths;
        private Dictionary<string, LocalMediaPack> addedLocalMediaPacks;
        private Dictionary<string, LocalMediaPack> activeLocalMediaPacks;

        public AppConfig()
        {
            mediaPlayerPaths = new Dictionary<MediaPlayerType, string>();
            addedLocalMediaPacks = new Dictionary<string, LocalMediaPack>();
            activeLocalMediaPacks = new Dictionary<string, LocalMediaPack>();
        }
        
        public bool AddMediaPlayerPath(MediaPlayerType type, string path) //adds a new media player location
        {
            if (path is null) throw new ArgumentNullException();
            if (path.Length < 4) throw new ArgumentException(); //"C:\x" is 4 chars

            if (mediaPlayerPaths.ContainsKey(type)) return false; //cannot add, already added, use ModifyMediaPlayerPath() instead
            else
            {
                mediaPlayerPaths.Add(type, path);
                return true; //added successfully
            }
        }
        public bool ModifyMediaPlayerPath(MediaPlayerType type, string newPath) //changes an already existing media player location
        {
            if (newPath is null) throw new ArgumentNullException();
            if (newPath.Length < 4) throw new ArgumentException(); //"C:\x" is 4 chars

            if (!mediaPlayerPaths.ContainsKey(type)) return false; //cannot modify non existent entry, use AddMediaPlayerPath() instead
            else
            {
                mediaPlayerPaths.Remove(type);
                mediaPlayerPaths.Add(type, newPath);
                return true; //added successfully
            }
        }
        public bool TryDeleteMediaPlayerPath(MediaPlayerType type)
        {
            throw new NotImplementedException();
        }
        public bool TryGetMediaPlayerPath(MediaPlayerType type, out string path)
        {
            throw new NotImplementedException();
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
