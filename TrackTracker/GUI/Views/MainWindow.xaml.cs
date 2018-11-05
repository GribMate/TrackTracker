using System;
using System.Threading;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

using WinForms = System.Windows.Forms;
using TrackTracker.BLL;
using TrackTracker.BLL.Enums;



namespace TrackTracker.GUI.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }

        private void menuItemApplicationExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}