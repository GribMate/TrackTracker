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

        DataViewModel() : base()
        {
            environmentService = DependencyInjector.GetService<IEnvironmentService>();
            Init();
        }

        private void Init()
        {
            ExternalDriveNames = environmentService.GetExternalDriveNames();
            
            foreach (SupportedFileExtension ext in Enum.GetValues(typeof(SupportedFileExtension)).Cast<SupportedFileExtension>()) // Casting to get typed iteration, just in case
            {
                SupportedFileFormats.Add(ext.ToString());
            }

            // Folder search is the default selected option
            FolderOption = true;
            DriveOption = false;
        }

        public List<string> ExternalDriveNames { get; set; }
        public List<string> SupportedFileFormats { get; set; }

        public string SelectedExternalDriveName { get; set; }
        public string SelectedSupportedFileFormat { get; set; }

        public bool FolderOption { get; set; }
        public bool DriveOption { get; set; }

        public string offlineFolderPath;
        public string OfflineFolderPath
        {
            get => offlineFolderPath;
            set
            {
                if (offlineFolderPath != value)
                {
                    offlineFolderPath = value;
                    RaisePropertyChangedEvent(x => x.OfflineFolderPath);
                }
            }
        }
    }
}
