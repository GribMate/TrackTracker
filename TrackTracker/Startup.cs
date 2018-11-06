using System;

using TrackTracker.BLL;
using TrackTracker.Services;
using TrackTracker.Services.Interfaces;



namespace TrackTracker
{
    /*
     * Static initializer class, that gets called once on every app start and manages prerequired setup.
    */
    public static class Startup
    {
        public static void Initialize() // First function to be called after OS gave control (and before GUI loads)
        {
            SetDependencies(); // Initializing DI

            PersistenceHelper.InitPersistenceOnStartup(DependencyInjector.GetService<IDatabaseService>(), DependencyInjector.GetService<IFileService>()); // Initializing data, and setting up database if not present already
        }

        private static void SetDependencies()
        {
            DependencyInjector.AddService<IDatabaseService, SQLiteService>();
            DependencyInjector.AddService<IFileService, FileService>();
            DependencyInjector.AddService<IEnvironmentService, WindowsEnvironmentService>();
            DependencyInjector.AddService<IMetadataService, MusicBrainzService>();
            DependencyInjector.AddService<IFingerprintService, AcoustIDService>();
            DependencyInjector.AddService<ISpotifyService, SpotifyService>();
        }
    }
}
