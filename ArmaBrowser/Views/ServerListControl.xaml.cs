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
    /// Interaktionslogik für ServerListControl.xaml
    /// </summary>
    public partial class ServerListControl : UserControl
    {
        public ServerListControl()
        {
            InitializeComponent();
        }

        private void ServerDatagrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ButtonState == MouseButtonState.Pressed)
            {
                var org = e.OriginalSource as FrameworkElement;
                if(org != null)
                {
                    if (org.DataContext is Logic.IServerItem)
                    {
                        ApplicationCommands.Open.Execute(org.DataContext, (IInputElement)sender);
                    }
                }
            }
        }

        private void ServerDatagrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}
