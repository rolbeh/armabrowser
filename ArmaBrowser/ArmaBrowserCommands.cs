using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ArmaBrowser
{
    static class ArmaBrowserCommands
    {
        public static RoutedCommand RefreshAddonsCommand { get; private set; }
        public static ICommand MarkAsFavorite { get; private set; }

        public static ICommand OpenAddonFolder { get; private set; }

        static ArmaBrowserCommands()
        {
            
            RefreshAddonsCommand = new RoutedCommand("Refresh Addons", typeof(ArmaBrowserCommands));
            MarkAsFavorite = new RoutedUICommand("Favorite", "MarkAsFavorite", typeof(ArmaBrowserCommands));
            OpenAddonFolder = new RoutedUICommand("Open", "OpenAddonFolder", typeof(ArmaBrowserCommands));

            CommandManager.RegisterClassCommandBinding(typeof(UIElement), new CommandBinding(ArmaBrowserCommands.MarkAsFavorite, MarkAsFavorite_OnExecuted));
            CommandManager.RegisterClassCommandBinding(typeof(FrameworkElement), new CommandBinding(RefreshAddonsCommand, RefreshAddonsCommand_OnExecuted));
            CommandManager.RegisterClassCommandBinding(typeof(FrameworkElement), new CommandBinding(OpenAddonFolder, OpenAddonFolder_OnExecuted));

        }

        private static void OpenAddonFolder_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter is string)
            {
                if (System.IO.Directory.Exists(e.Parameter.ToString()))
                    System.Diagnostics.Process.Start(e.Parameter.ToString());
                else
                    MessageBox.Show("Folder not found");
            }
        }

        private static void RefreshAddonsCommand_OnExecuted(object sender, ExecutedRoutedEventArgs executedRoutedEventArgs)
        {
            var dc = ((FrameworkElement) sender).DataContext as ViewModel.ServerListViewModel;
            if (dc != null)
            {
                dc.RefreshAddons();
            } 

        }

        private static void MarkAsFavorite_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var dataContext = e.Parameter as ViewModel.ServerListViewModel;
            if (dataContext != null)
            {
                var item = dataContext.SelectedServerItem;
                if (item != null)
                {
                    item.IsFavorite = !item.IsFavorite;
                    dataContext.SaveFavorits();
                }
            }
        }
    }
}
