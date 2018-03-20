using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Onlab.Dialogs
{
    /// <summary>
    /// Interaction logic for ExceptionNotification.xaml
    /// </summary>
    public partial class ExceptionNotification : Window
    {
        public ExceptionNotification(string title, string description, string details = null)
        {
            try
            {
                InitializeComponent();

                if (title != null) Title = title;
                else Title = "Unknown error";
                if (description != null) textBlockDescription.Text = description;
                else textBlockDescription.Text = "We don't know what went wrong. Please restart the application!";
                if (details != null) textBlockDetails.Text = details;
                else textBlockDetails.Text = "";
            }
            catch (Exception) { } //do not throw exceptions from this window, since it's job is to show them
        }

        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            try { Close(); }
            catch (Exception) { } //do not throw exceptions from this window, since it's job is to show them
        }
    }
}
