using System.Windows;



namespace TrackTracker
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e) // Entry point before GUI loads
        {
            base.OnStartup(e);

            TrackTracker.Startup.Initialize(); // Must be called before GUI loads (sets up DI, and first run settings)
        }
    }
}
