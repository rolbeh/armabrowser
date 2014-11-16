using ArmaBrowser.ViewModel;
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
                using (  Logger.Default.Subscribe(new TextBoxLoggerApender(LoggerTextBox)))
                {
                    ((ServerListViewModel)DataContext).LookForInstallation();
                }
            }
        }
    }
}
