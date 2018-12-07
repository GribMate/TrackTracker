using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

using TrackTracker.BLL.Model;
using TrackTracker.BLL.Model.Base;

namespace TrackTracker.BLL.GlobalContexts
{
    /*
     * Provides a static single instance collection of app-wide data that cannot be tied to either one of the View tabs separately.
     * 
     * Statistical data for PieCharts.
     * 
     * TODO: rethink after MVVM
    */
    public static class StatisticsContext
    {
        public static ObservableCollection<PieChartableData> CountsByArtist { get; set; } = new ObservableCollection<PieChartableData>();
        public static ObservableCollection<PieChartableData> CountsByAlbum { get; set; } = new ObservableCollection<PieChartableData>();
        public static ObservableCollection<PieChartableData> CountsByGenre { get; set; } = new ObservableCollection<PieChartableData>();
        public static ObservableCollection<PieChartableData> CountsByDecade { get; set; } = new ObservableCollection<PieChartableData>();
    }
}
