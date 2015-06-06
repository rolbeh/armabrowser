using ArmaBrowser.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ArmaBrowser
{
    using WinInterop = System.Windows.Interop;
    using System.Runtime.InteropServices;
    using System.Xml.Serialization;
    using System.IO;
    using System.Threading;
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    partial class MainWindow : Window
    {
        private CancellationTokenSource _joinServerCancellationTokenSrc;
        public MainWindow()
        {

            _joinServerCancellationTokenSrc = new CancellationTokenSource();
            this.SourceInitialized += new EventHandler(win_SourceInitialized);

            this.IsVisibleChanged += MainWindow_IsVisibleChanged;
            if (Properties.Settings.Default.MainWindowState > -1)
            {
                this.Height = Properties.Settings.Default.MainWindowHeight;
                this.Width = Properties.Settings.Default.MainWindowWidth;
                this.Top = Properties.Settings.Default.MainWindowTop;
                this.Left = Properties.Settings.Default.MainWindowLeft;
            }

            InitializeComponent();

            AutoJoinView.Visibility = Visibility.Collapsed;
            TabListBox.SelectedIndex = 0;
            Test.Freeze();

        }

        void win_SourceInitialized(object sender, EventArgs e)
        {
            System.IntPtr handle = (new WinInterop.WindowInteropHelper(this)).Handle;
            WinInterop.HwndSource.FromHwnd(handle).AddHook(new WinInterop.HwndSourceHook(Win.WindowProc));
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

        private void RefrshServerlist_Click(object sender, RoutedEventArgs e)
        {
            MyViewModel.Reload();
        }

        private void OpenCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ((MyViewModel.SelectedServerItem != null && MyViewModel.SelectedServerItem.Port > 0) || MyViewModel.LaunchWithoutHost)
                               && System.IO.Directory.Exists(Properties.Settings.Default.ArmaPath);
        }

        private void OpenCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            JoinServer();
        }

        void CancelJoiningServer()
        {
            var old = _joinServerCancellationTokenSrc;
            _joinServerCancellationTokenSrc = new CancellationTokenSource();
            old.Cancel();
            old.Dispose();
        }

        async void JoinServer()
        {
            AutoJoinView.Visibility = System.Windows.Visibility.Visible;
            try
            {
                AutoJoinView.DataContext = MyViewModel.SelectedServerItem;
                CancelJoiningServer();
                var canJoinServer = await WaitForSlotAsync(_joinServerCancellationTokenSrc.Token);

                if (canJoinServer)
                {
                    MyViewModel.OpenArma();
                    this.WindowState = System.Windows.WindowState.Minimized;
                }
            }
            finally
            {
                AutoJoinView.Visibility = System.Windows.Visibility.Collapsed;
            }

        }

        async Task<bool> WaitForSlotAsync(CancellationToken token)
        {
            return await Task.Run(() =>
            {
                Thread.CurrentThread.Priority = ThreadPriority.AboveNormal;
                while (MyViewModel.SelectedServerItem != null
                    && MyViewModel.SelectedServerItem.IsPlayerSlotsFull)
                {
                    if (token.IsCancellationRequested)
                        return false;
                    Thread.Sleep(1000);
                }

                return true;
            });
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

        private void PoweredByHyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Uri.ToString());
        }

        private void ServerListControl_Loaded(object sender, RoutedEventArgs e)
        {
            //Task.Delay(500).ContinueWith(t => MyViewModel.Reload());
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(HostConfigCollection));
            using (System.IO.TextWriter textwr = new StringWriter())
            {
                serializer.Serialize(textwr, HostConfigCollection.Default);
                Properties.Settings.Default.HostConfigs = textwr.ToString();
                Properties.Settings.Default.Save();
            }
            try
            {
                MyViewModel.StopAll();
            }
            catch
            {
            }
        }

        private void CvsSelectedAddons_OnFilter(object sender, FilterEventArgs e)
        {
            var item = e.Item as IAddon;
            if (item != null)
            {
                e.Accepted = item.IsActive;
            }
        }

        private void TextBlock_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (System.IO.Directory.Exists(Properties.Settings.Default.ArmaPath))
                System.Diagnostics.Process.Start(Properties.Settings.Default.ArmaPath);
            else
                MessageBox.Show("Arma 3 installation not found");
        }

        private void AppbarSettings_OnClick(object sender, RoutedEventArgs e)
        {
            TabListBox.SelectedItem = "Arma III";
        }

        private void AutoJoinControl_Canceled(object sender, RoutedEventArgs e)
        {
            CancelJoiningServer();
        }

        private void OpenInstallFolder_OnClick(object sender, RoutedEventArgs e)
        {
            if (System.IO.Directory.Exists(Properties.Settings.Default.ArmaPath))
                System.Diagnostics.Process.Start(Properties.Settings.Default.ArmaPath);
            else
                MessageBox.Show("Arma 3 installation not found");
        }

        private void PreferencesContextMenuButton_OnClick(object sender, RoutedEventArgs e)
        {
            var btn = (Button) sender;
            PreferencesContextMenu.PlacementTarget = btn;
            PreferencesContextMenu.Placement = PlacementMode.Bottom;
            ContextMenuService.SetPlacement(btn, PlacementMode.Bottom);

            PreferencesContextMenu.IsOpen = true;
        }
    }
}
