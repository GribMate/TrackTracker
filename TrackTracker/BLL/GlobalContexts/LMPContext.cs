using System;
using System.Collections.ObjectModel;

using TrackTracker.BLL.Model;



namespace TrackTracker.BLL.GlobalContexts
{
    /*
     * Provides a static single instance collection of app-wide data that cannot be tied to either one of the View tabs separately.
     * 
     * LMPs.
     * 
     * TODO: rethink after MVVM
    */
    public static class LMPContext
    {

        // LMPs that are added by the user, but not actively displayed in Tracklist
        public static ObservableCollection<LocalMediaPack> StoredLocalMediaPacks = new ObservableCollection<LocalMediaPack>();

        // LMPs that are currently displayed in Tracklist
        public static ObservableCollection<LocalMediaPack> ActiveLocalMediaPacks = new ObservableCollection<LocalMediaPack>();



        // The API of this class intentionally does not support modifying an existing LMP in any collection,
        // since every LMP should be different and exactly one should exist per rootPath.
        // Note that this doesn't exclude the possibility of an LMP containing a subset or a superset of another LMP's paths.

        public static void ActivateLMP(LocalMediaPack lmp) // Activates a given LMP object
        {
            if (lmp == null)
                throw new ArgumentNullException(nameof(lmp), $"Cannot activate LMP, since the parameter is null.");

            if (StoredLocalMediaPacks.Contains(lmp) == true && ActiveLocalMediaPacks.Contains(lmp) == false)
            {
                StoredLocalMediaPacks.Remove(lmp);
                ActiveLocalMediaPacks.Add(lmp);

                foreach (var path in lmp.GetAllFilePaths())
                {
                    TracklistContext.AddMusicFile(path.Value, path.Key); // TODO: review
                }
            }
        }
        public static void DeactivateLMP(LocalMediaPack lmp) // Deactivates a given LMP object
        {
            if (lmp == null)
                throw new ArgumentNullException(nameof(lmp), $"Cannot deactivate LMP, since the parameter is null.");

            if (StoredLocalMediaPacks.Contains(lmp) == false && ActiveLocalMediaPacks.Contains(lmp) == true)
            {
                ActiveLocalMediaPacks.Remove(lmp);
                StoredLocalMediaPacks.Add(lmp);
               
                foreach (var path in lmp.GetAllFilePaths())
                {
                    TracklistContext.RemoveMusicFile(path.Key); // TODO: review
                }
            }
        }
    }
}
