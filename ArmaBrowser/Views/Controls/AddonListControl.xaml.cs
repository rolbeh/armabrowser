using System.Windows;
using System.Windows.Controls;

namespace ArmaBrowser.Views.Controls
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
