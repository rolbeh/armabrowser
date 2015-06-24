using System.Windows;
using System.Windows.Controls;
using ArmaBrowser.ViewModel;

namespace ArmaBrowser.Views.Controls
{
    /// <summary>
    /// Interaktionslogik für PreferencesControl.xaml
    /// </summary>
    public partial class PreferencesControl : UserControl
    {
        

        public PreferencesControl()
        {
            InitializeComponent();
        }

        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (true.Equals(e.NewValue))
            {
                LoggerTextBox.Clear();
                using (Logger.Default.Subscribe(new TextBoxLoggerApender(LoggerTextBox)))
                {
                    ((ServerListViewModel)DataContext).LookForInstallation();
                }
            }
        }
    }
}
