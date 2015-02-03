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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ArmaBrowser.Views
{
    /// <summary>
    /// Interaktionslogik für AddonsControl.xaml
    /// </summary>
    partial class AddonsControl : UserControl
    {
        public AddonsControl()
        {
            InitializeComponent();
            AddonsListBox.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("DisplayText", System.ComponentModel.ListSortDirection.Ascending));
        }

        private void ListBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is ScrollViewer)
            {
                ((ListBox)sender).SelectedIndex = -1;
            }
        }
 
    }
}
