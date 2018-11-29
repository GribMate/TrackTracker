using System;
using System.Windows.Controls;



namespace TrackTracker.GUI.Views
{
    public partial class TabStatistics : UserControl
    {
        public TabStatistics()
        {
            ViewModels.StatisticsViewModel.View = this;
            InitializeComponent();
        }
    }
}