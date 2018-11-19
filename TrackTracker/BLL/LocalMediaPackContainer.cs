using System;
using System.Collections.Generic;
using System.Text;
using TrackTracker.Services.Interfaces;

namespace TrackTracker.BLL
{
    public class LocalMediaPackContainer
    {
        //the API of this class intentionally does not support modifying an existing LMP in any collection
        //since every LMP should be different and exactly one should exist per rootPath
        //note that this doesn't exclude the possibility of an LMP containing a subset or a superset of another LMP's paths

        //TODO: error handling and extended interface

        private Dictionary<string, LocalMediaPack> addedLocalMediaPacks; //LMPs that are added by the user, but not actively displayed in Tracklist
        private Dictionary<string, LocalMediaPack> activeLocalMediaPacks; //LMPs that are currently displayed in Tracklist

        public LocalMediaPackContainer()
        {
            addedLocalMediaPacks = new Dictionary<string, LocalMediaPack>();
            activeLocalMediaPacks = new Dictionary<string, LocalMediaPack>();
        }

        public bool AddLMP(LocalMediaPack pack, bool writeToDB)
        {
            if (pack is null) throw new ArgumentNullException();

            if (addedLocalMediaPacks.ContainsKey(pack.RootPath)) return false; //cannot add again, already added
            else
            {
                addedLocalMediaPacks.Add(pack.RootPath, pack);

                if (writeToDB)
                {
                    string[] values = new string[4];
                    values[0] = pack.RootPath;
                    values[1] = pack.BaseExtension.ToString();
                    values[2] = Convert.ToInt32(pack.IsResultOfDriveSearch).ToString(); //0 or 1
                    StringBuilder sb = new StringBuilder();
                    foreach (string path in pack.GetAllFilePaths().Keys)
                    {
                        sb.Append(path);
                        sb.Append("|");
                    }
                    sb.Remove(sb.Length - 1, 1); //removing unnecessary closing "|"
                    values[3] = sb.ToString();
                    DependencyInjector.GetService<IDatabaseService>().InsertInto("LocalMediaPacks", values);
                }

                return true; //added successfully
            }
        }
        public void ActivateLMP(string rootPath)
        {
            if (rootPath == null) throw new ArgumentNullException();
            if (rootPath.Length < 3) throw new ArgumentException(); //"C:\" is 3 chars long

            if (addedLocalMediaPacks.ContainsKey(rootPath) && !activeLocalMediaPacks.ContainsKey(rootPath))
            {
                LocalMediaPack toChange;
                addedLocalMediaPacks.TryGetValue(rootPath, out toChange);
                addedLocalMediaPacks.Remove(rootPath);
                activeLocalMediaPacks.Add(rootPath, toChange);

                foreach (var path in toChange.GetAllFilePaths())
                {
                    GlobalContext.AddMusicFile(path.Value, path.Key);
                }
            }
        }
        public void DeactivateLMP(string rootPath)
        {
            if (rootPath == null) throw new ArgumentNullException();
            if (rootPath.Length < 3) throw new ArgumentException(); //"C:\" is 3 chars long

            if (activeLocalMediaPacks.ContainsKey(rootPath) && !addedLocalMediaPacks.ContainsKey(rootPath))
            {
                LocalMediaPack toChange;
                activeLocalMediaPacks.TryGetValue(rootPath, out toChange);
                activeLocalMediaPacks.Remove(rootPath);
                addedLocalMediaPacks.Add(rootPath, toChange);

                foreach (var path in toChange.GetAllFilePaths())
                {
                    GlobalContext.RemoveMusicFile(path.Key);
                }
            }
        }
        public bool DeleteLMPFromAdded(string path) //removes an LMP from the list of added LMPs, indentified by its rootPath
        {
            if (path is null) throw new ArgumentNullException();
            if (path.Length < 3) throw new ArgumentException(); //"C:\" is 3 chars long

            if (!addedLocalMediaPacks.ContainsKey(path)) return false; //cannot delete if LMP doesn't already exist
            else
            {
                addedLocalMediaPacks.Remove(path);
                return true; //deleted successfully
            }
        }
        public bool DeleteLMPFromActive(string path) //removes and LMP from the list of active LMPs, identified by its rootPath
        {
            if (path is null) throw new ArgumentNullException();
            if (path.Length < 3) throw new ArgumentException(); //"C:\" is 3 chars long

            if (!activeLocalMediaPacks.ContainsKey(path)) return false; //cannot delete if LMP doesn't already exist
            else
            {
                activeLocalMediaPacks.Remove(path);
                return true; //deleted successfully
            }
        }
        public List<LocalMediaPack> GetAllAddedLMPs() //returns a copy of the list of added LMPs
        {
            return new List<LocalMediaPack>(addedLocalMediaPacks.Values); //copying to avoid modification by reference
        }
        public List<LocalMediaPack> GetAllActiveLMPs() //returns a copy of the list of active LMPs
        {
            return new List<LocalMediaPack>(activeLocalMediaPacks.Values); //copying to avoid modification by reference
        }
        public bool GetLMPFromAdded(string path, out LocalMediaPack lmp) //returns an LMP from the added list by it's rootPath
        {
            if (path == null) throw new ArgumentNullException();
            if (path.Length < 3) throw new ArgumentException(); //"C:\" is 3 characters

            //we don't have to re-write "get value" logic, since Dictionary already provides it
            //however, we still want to hide the internal data structure
            return addedLocalMediaPacks.TryGetValue(path, out lmp);
        }
        public bool GetLMPFromActive(string path, out LocalMediaPack lmp) //returns an LMP from the active list by it's rootPath
        {
            if (path == null) throw new ArgumentNullException();
            if (path.Length < 3) throw new ArgumentException(); //"C:\" is 3 characters

            //we don't have to re-write "get value" logic, since Dictionary already provides it
            //however, we still want to hide the internal data structure
            return activeLocalMediaPacks.TryGetValue(path, out lmp);
        }
    }
}
