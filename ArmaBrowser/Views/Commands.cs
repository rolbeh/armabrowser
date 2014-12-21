using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ArmaBrowser.View
{
    static class Commands
    {
        public static ICommand MarkAsFavorite { get; private set; }

        static Commands()
        {
            MarkAsFavorite = new RoutedUICommand("Favorite", "MarkAsFavorite", typeof(Commands));

            CommandManager.RegisterClassCommandBinding(typeof(UIElement), new CommandBinding( Commands.MarkAsFavorite, MarkAsFavorite_OnExecuted));
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
