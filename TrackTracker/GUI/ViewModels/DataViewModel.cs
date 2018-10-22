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
        }

        public string SelectedExternalDriveName { get; set; }
        public string SelectedSupportedFileFormat { get; set; }

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
                }
            }
        }
    }
}
