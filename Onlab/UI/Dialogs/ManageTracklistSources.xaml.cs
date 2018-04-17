using System;
using System.Windows;

using Onlab.BLL;



namespace Onlab.Dialogs
{
    /// <summary>
    /// Interaction logic for ManageTracklistSources.xaml
    /// </summary>
    public partial class ManageTracklistSources : Window
    {
        //TODO: Add reverse selection buttons
        public ManageTracklistSources()
        {
            InitializeComponent();
        }

        private void sources_library_buttonSelectAll_Click(object sender, RoutedEventArgs e)
        {
            sources_library_listBoxItems.SelectAll();
        }
        private void sources_added_buttonSelectAll_Click(object sender, RoutedEventArgs e)
        {
            sources_added_listBoxItems.SelectAll();
        }
        private void sources_buttonMoveToAdded_Click(object sender, RoutedEventArgs e)
        {
            if (sources_library_listBoxItems.SelectedIndex != -1)
            {
                var selected = sources_library_listBoxItems.SelectedItems;
                for (int i = 0; i < selected.Count; i++)
                {
                    string rootPath = LocalMediaPack.GetRootPathFromFormattedName(selected[i] as string);
                    GlobalVariables.LocalMediaPackContainer.ActivateLocalMediaPack(rootPath);
                    sources_added_listBoxItems.Items.Add(selected[i]);
                }
                for (int i = selected.Count - 1; i >= 0; i--) //reversed for loop so we can delete and not skip anything
                {
                    sources_library_listBoxItems.Items.Remove(selected[i]);
                }
            }
        }
        private void sources_buttonRemoveFromAdded_Click(object sender, RoutedEventArgs e)
        { 
            if (sources_added_listBoxItems.SelectedIndex != -1)
            {
                var selected = sources_added_listBoxItems.SelectedItems;
                for (int i = 0; i < selected.Count; i++)
                {
                    string rootPath = LocalMediaPack.GetRootPathFromFormattedName(selected[i] as string);
                    GlobalVariables.LocalMediaPackContainer.DeactivateLocalMediaPack(rootPath);
                    sources_library_listBoxItems.Items.Add(selected[i]);
                }
                for (int i = selected.Count - 1; i >= 0; i--) //reversed for loop so we can delete and not skip anything
                {
                    sources_added_listBoxItems.Items.Remove(selected[i]);
                }
            }
        }
        private void Window_Initialized(object sender, EventArgs e)
        {
            foreach (LocalMediaPack lmp in GlobalVariables.LocalMediaPackContainer.ActiveLocalMediaPacks)
            {
                sources_added_listBoxItems.Items.Add(lmp.GetFormattedName());
            }
            foreach (LocalMediaPack lmp in GlobalVariables.LocalMediaPackContainer.AddedLocalMediaPacks)
            {
                sources_library_listBoxItems.Items.Add(lmp.GetFormattedName());
            }
        }
        private void sources_buttonClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
