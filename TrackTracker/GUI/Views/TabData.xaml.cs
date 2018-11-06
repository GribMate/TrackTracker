using System;
using System.Windows.Controls;

using TrackTracker.GUI.ViewModels;



namespace TrackTracker.GUI.Views
{
    public partial class TabData : UserControl
    {
        public TabData()
        {
            InitializeComponent();
            
            DataContext = new DataViewModel(); // Binding VM
        }
    }
}
