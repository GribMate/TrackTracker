using System;
using System.Windows.Controls;

using TrackTracker.BLL.GlobalContexts;



namespace TrackTracker.GUI.Views
{
    public partial class AlbumPieChart : UserControl
    {
        public AlbumPieChart()
        {
            InitializeComponent();

            DataContext = StatisticsContext.CountsByAlbum;
        }
    }
}
