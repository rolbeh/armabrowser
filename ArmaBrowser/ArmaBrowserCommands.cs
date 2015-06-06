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

        static ArmaBrowserCommands()
        {
            
            RefreshAddonsCommand = new RoutedCommand("Refresh Addons", typeof(ArmaBrowserCommands));


            CommandManager.RegisterClassCommandBinding(typeof(FrameworkElement), new CommandBinding(RefreshAddonsCommand, RefreshAddonsCommand_OnExecuted));

        }

        private static void RefreshAddonsCommand_OnExecuted(object sender, ExecutedRoutedEventArgs executedRoutedEventArgs)
        {
            var dc = ((FrameworkElement) sender).DataContext as ViewModel.ServerListViewModel;
            if (dc != null)
            {
                dc.RefreshAddons();
            } 

        }
    }
}
