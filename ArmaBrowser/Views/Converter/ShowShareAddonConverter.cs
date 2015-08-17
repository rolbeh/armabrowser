using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using ArmaBrowser.Logic;

namespace ArmaBrowser.Views.Converter
{
    class ShowShareAddonConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var item = value as IAddon;
            if (item == null) return Visibility.Collapsed;

            return (item.IsInstalled && (item.IsEasyInstallable.HasValue && !item.IsEasyInstallable.Value)) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class ShowEasyInstallAddonConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var item = value as IAddon;
            if (item == null) return Visibility.Collapsed;

            return (!item.IsInstalled && item.IsEasyInstallable.HasValue && item.IsEasyInstallable.Value) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
