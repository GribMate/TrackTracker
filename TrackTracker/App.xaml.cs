using System.Windows;



namespace TrackTracker
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e) //entry point before GUI loads
        {
            base.OnStartup(e);
            TrackTracker.BLL.GlobalAlgorithms.Initialize(); //must be called before GUI loads to avoid null reference exceptions
        }
    }
}
