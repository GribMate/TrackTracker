using System;
using System.Linq;
using System.Collections.Generic;

using TrackTracker.Services.Interfaces;
using TrackTracker.BLL;
using TrackTracker.BLL.Enums;



namespace TrackTracker.GUI.ViewModels
{
    public class DataViewModel : ViewModelBase<DataViewModel>
    {
        private IEnvironmentService environmentService;

        private List<string> ExternalDriveNames { get; set; }
        private List<string> SupportedFileFormats { get; set; }

        public DataViewModel() : base()
        {
            environmentService = DependencyInjector.GetService<IEnvironmentService>();
            Init();
        }

        private void Init()
        {
            ExternalDriveNames = environmentService.GetExternalDriveNames();
            
            foreach (SupportedFileExtension ext in Enum.GetValues(typeof(SupportedFileExtension)).Cast<SupportedFileExtension>()) // Casting to get typed iteration, just in case
            {
                if (ext == SupportedFileExtension.Unknown)
                    continue; // Do not offer the selection of "Unknown" format

                SupportedFileFormats.Add(ext.ToString());
            }
        }

        // We only bind one bool to the 2 checkboxes, currently it is enough functionality
        // Might require a better solution in the future...
        private bool isFolderSelected;
        public bool IsFolderSelected
        {
            get => isFolderSelected;
            set
            {
                if (isFolderSelected != value)
                {
                    isFolderSelected = value;
                    RaisePropertyChangedEvent(x => x.IsFolderSelected);
                }
            }
        }

        private string selectedExternalDriveName;
        public string SelectedExternalDriveName
        {
            get => selectedExternalDriveName;
            set
            {
                if (selectedExternalDriveName != value)
                {
                    selectedExternalDriveName = value;
                    RaisePropertyChangedEvent(x => x.SelectedExternalDriveName);
                    RaisePropertyChangedEvent(x => x.CanExecuteAddFiles);
                }
            }
        }

        private SupportedFileExtension selectedSupportedFileExtension;
        public SupportedFileExtension SelectedSupportedFileExtension
        {
            get => selectedSupportedFileExtension;
            set
            {
                if (selectedSupportedFileExtension != value)
                {
                    selectedSupportedFileExtension = value;
                    RaisePropertyChangedEvent(x => x.SelectedSupportedFileExtension);
                    RaisePropertyChangedEvent(x => x.CanExecuteAddFiles);
                }
            }
        }

        private string offlineFolderPath;
        public string OfflineFolderPath
        {
            get => offlineFolderPath;
            set
            {
                if (offlineFolderPath != value)
                {
                    offlineFolderPath = value;
                    RaisePropertyChangedEvent(x => x.OfflineFolderPath);
                    RaisePropertyChangedEvent(x => x.CanExecuteAddFiles);
                }
            }
        }

        public bool CanExecuteAddFiles
        {
            get
            {
                bool folderProper = !String.IsNullOrWhiteSpace(OfflineFolderPath) && OfflineFolderPath.Length > 8; // "C:\x.mp3" is 8 chars
                bool driveProper = !String.IsNullOrWhiteSpace(SelectedExternalDriveName) && SelectedSupportedFileExtension != SupportedFileExtension.Unknown;
                return folderProper || driveProper;
            }
        }
        public void ExecuteAddFiles()
        {
            LocalMediaPack lmp = null;

            if (IsFolderSelected)  // Parsing an offline folder, all supported file formats may appear
            {
                lmp = new LocalMediaPack(OfflineFolderPath, false);
                GlobalAlgorithms.LoadFilesFromDirectory(GlobalAlgorithms.FileService, lmp, OfflineFolderPath); // Loading up LMP object with file paths
                OfflineFolderPath = "Please select your offline music folder...";
            }
            else  // Parsing an external drive, with a given file format
            {
                lmp = new LocalMediaPack(SelectedExternalDriveName, true, SelectedSupportedFileExtension);
                GlobalAlgorithms.LoadFilesFromDrive(GlobalAlgorithms.FileService, lmp, SelectedExternalDriveName, SelectedSupportedFileExtension); // Loading up LMP object with file paths
            }

            GlobalAlgorithms.LocalMediaPackContainer.AddLMP(lmp, true);
        }
    }
}
