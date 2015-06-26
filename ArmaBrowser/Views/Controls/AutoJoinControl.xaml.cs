using System.Windows;
using System.Windows.Controls;

namespace ArmaBrowser.Views.Controls
{
    /// <summary>
    /// Interaktionslogik für AutoJoinControl.xaml
    /// </summary>
    public partial class AutoJoinControl : UserControl
    {
        public AutoJoinControl()
        {
            InitializeComponent();
        }

        public event RoutedEventHandler Canceled;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Canceled != null)
                Canceled(this, new RoutedEventArgs());
        }
    }
}
