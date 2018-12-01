using System;
using System.Windows.Input;

using TrackTracker.BLL.GlobalContexts;
using TrackTracker.BLL.Model;
using TrackTracker.GUI.Interfaces;
using TrackTracker.GUI.ViewModels.Base;



namespace TrackTracker.GUI.ViewModels.Dialogs
{
    public class ManageTracklistSourcesViewModel : ViewModelBase
    {
        public ManageTracklistSourcesViewModel()
        {
            //SelectActiveAllCommand = new RelayCommand(exe => ExecuteSelectActiveAll(), can => CanExecuteSelectActiveAll);
            //SelectActiveReverseCommand = new RelayCommand(exe => ExecuteSelectActiveReverse(), can => CanExecuteSelectActiveReverse);
            //SelectStoredAllCommand = new RelayCommand(exe => ExecuteSelectStoredAll(), can => CanExecuteSelectStoredAll);
            //SelectStoredReverseCommand = new RelayCommand(exe => ExecuteSelectStoredReverse(), can => CanExecuteSelectStoredReverse);
            ActivateCommand = new RelayCommand<object>(exe => ExecuteActivate(), can => CanExecuteActivate);
            DeactivateCommand = new RelayCommand<object>(exe => ExecuteDeactivate(), can => CanExecuteDeactivate);
            CloseCommand = new RelayCommand<IClosable>(exe => ExecuteClose(exe), can => CanExecuteClose);
        }

        //public ICommand SelectActiveAllCommand { get; }
        //public ICommand SelectActiveReverseCommand { get; }
        //public ICommand SelectStoredAllCommand { get; }
        //public ICommand SelectStoredReverseCommand { get; }
        public ICommand ActivateCommand { get; }
        public ICommand DeactivateCommand { get; }
        public ICommand CloseCommand { get; }



        //public bool CanExecuteSelectActiveAll
        //{
        //    get => LMPContext.ActiveLocalMediaPacks.Count > 0;
        //}
        //private void ExecuteSelectActiveAll()
        //{
        //    foreach (LocalMediaPack lmp in LMPContext.ActiveLocalMediaPacks)
        //    {
        //        lmp.IsSelected = true;
        //    }
        //}

        //public bool CanExecuteSelectActiveReverse
        //{
        //    get => LMPContext.ActiveLocalMediaPacks.Count > 0;
        //}
        //private void ExecuteSelectActiveReverse()
        //{
        //    foreach (LocalMediaPack lmp in LMPContext.ActiveLocalMediaPacks)
        //    {
        //        if (lmp.IsSelected)
        //            lmp.IsSelected = false;
        //        else
        //            lmp.IsSelected = true;
        //    }
        //}

        //public bool CanExecuteSelectStoredAll
        //{
        //    get => LMPContext.StoredLocalMediaPacks.Count > 0;
        //}
        //private void ExecuteSelectStoredAll()
        //{
        //    foreach (LocalMediaPack lmp in LMPContext.StoredLocalMediaPacks)
        //    {
        //        lmp.IsSelected = true;
        //    }
        //}

        //public bool CanExecuteSelectStoredReverse
        //{
        //    get => LMPContext.StoredLocalMediaPacks.Count > 0;
        //}
        //private void ExecuteSelectStoredReverse()
        //{
        //    foreach (LocalMediaPack lmp in LMPContext.StoredLocalMediaPacks)
        //    {
        //        if (lmp.IsSelected)
        //            lmp.IsSelected = false;
        //        else
        //            lmp.IsSelected = true;
        //    }
        //}

        private LocalMediaPack selectedStoredLMP;
        public LocalMediaPack SelectedStoredLMP
        {
            get => selectedStoredLMP;
            set { SetProperty(ref selectedStoredLMP, value); }
        }

        private LocalMediaPack selectedActiveLMP;
        public LocalMediaPack SelectedActiveLMP
        {
            get => selectedActiveLMP;
            set { SetProperty(ref selectedActiveLMP, value); }
        }

        public bool CanExecuteActivate
        {
            get => SelectedStoredLMP != null;
            //{
            //    foreach (LocalMediaPack lmp in LMPContext.StoredLocalMediaPacks)
            //    {
            //        if (lmp.IsSelected)
            //            return true;
            //    }

            //    return false;
            //}
        }
        private void ExecuteActivate()
        {
            //foreach (LocalMediaPack lmp in LMPContext.StoredLocalMediaPacks)
            //{
            //    if (lmp.IsSelected)
            //        LMPContext.ActivateLMP(lmp);
            //}
            LMPContext.ActivateLMP(SelectedStoredLMP);
        }

        public bool CanExecuteDeactivate
        {
            get => SelectedActiveLMP != null;
            //{
            //    foreach (LocalMediaPack lmp in LMPContext.ActiveLocalMediaPacks)
            //    {
            //        if (lmp.IsSelected)
            //            return true;
            //    }

            //    return false;
            //}
        }
        private void ExecuteDeactivate()
        {
            //foreach (LocalMediaPack lmp in LMPContext.ActiveLocalMediaPacks)
            //{
            //    if (lmp.IsSelected)
            //        LMPContext.DeactivateLMP(lmp);
            //}
            LMPContext.DeactivateLMP(SelectedActiveLMP);
        }

        public bool CanExecuteClose
        {
            get => true;
        }
        private void ExecuteClose(IClosable window)
        {
            if (window != null)
                window.Close();
        }
    }
}
