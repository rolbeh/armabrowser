using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ArmaBrowser.Views.Controls
{
    /// <summary>
    /// Interaktionslogik für ServerListControl.xaml
    /// </summary>
    partial class ServerListControl : UserControl
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
