using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Net;
using System.Xml;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ArmaServerBrowser
{
    using Data;
    using ArmaServerBrowser.Logic;
    using System.IO;
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.IsVisibleChanged += MainWindow_IsVisibleChanged;
            if (Properties.Settings.Default.MainWindowState > -1)
            {
                this.Height = Properties.Settings.Default.MainWindowHeight;
                this.Width = Properties.Settings.Default.MainWindowWidth;
                this.Top = Properties.Settings.Default.MainWindowTop;
                this.Left = Properties.Settings.Default.MainWindowLeft;

            }
            InitializeComponent();

        }

        private void MainWindow_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!this.IsInitialized && e.NewValue.Equals(true))
                return;

            this.IsVisibleChanged -= MainWindow_IsVisibleChanged;
            if (Properties.Settings.Default.MainWindowState > -1)
            {
                //Task.Delay(300).ContinueWith(t =>
                //{
                this.WindowState = (System.Windows.WindowState)Properties.Settings.Default.MainWindowState;
                //}, TaskScheduler.FromCurrentSynchronizationContext());                
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MyViewModel.Reload();
        }

        private void OpenCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = MyViewModel.SelectedServerItem != null || MyViewModel.LaunchWithoutHost;
        }

        private void OpenCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MyViewModel.OpenArma();
        }

        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.MainWindowHeight = this.ActualHeight;
            Properties.Settings.Default.MainWindowWidth = this.ActualWidth;
            Properties.Settings.Default.MainWindowTop = this.Top;
            Properties.Settings.Default.MainWindowLeft = this.Left;
            Properties.Settings.Default.MainWindowState = (int)this.WindowState;
            App.Current.Shutdown(0);
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if (this.WindowState == System.Windows.WindowState.Maximized)
                {
                    this.WindowState = System.Windows.WindowState.Normal;
                    e.Handled = true;
                    return;
                }

                if (this.WindowState == System.Windows.WindowState.Normal)
                {
                    this.WindowState = System.Windows.WindowState.Maximized;
                    e.Handled = true;
                    return;
                }
            }
        }

        private void PoweredBy_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MyViewModel.TextFilter = "Armajunkies";
        }

        private void PoweredByHyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Uri.ToString());
        }

        private void ServerListControl_Loaded(object sender, RoutedEventArgs e)
        {
            MyViewModel.Reload();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.Save();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            App.Current.MainWindow.WindowState = System.Windows.WindowState.Minimized;
        }

        private void CvsSelectedAddons_OnFilter(object sender, FilterEventArgs e)
        {
            var item = e.Item as IAddon;
            if (item != null)
            {
                e.Accepted = item.IsActive;
            }
        }

        private void TestJsonButton_OnClick(object sender, RoutedEventArgs e)
        {
           // MyViewModel.TestJson();           
        }
    }
     
}
