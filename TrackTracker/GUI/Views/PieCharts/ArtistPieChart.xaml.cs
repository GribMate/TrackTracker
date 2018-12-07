using System;
using System.Windows.Controls;

using TrackTracker.BLL.GlobalContexts;



namespace TrackTracker.GUI.Views
{
    public partial class ArtistPieChart : UserControl
    {
        public ArtistPieChart()
        {
            InitializeComponent();

            DataContext = StatisticsContext.CountsByArtist;
        }
    }
}
