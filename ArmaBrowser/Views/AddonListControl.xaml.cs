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
    /// Interaktionslogik für AddonListControl.xaml
    /// </summary>
    public partial class AddonListControl : UserControl
    {
        public AddonListControl()
        {
            InitializeComponent();
        } 

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }
    }
}
