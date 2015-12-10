using System;
using System.Globalization;
using System.Windows.Data;
using ArmaBrowser.Logic;

namespace ArmaBrowser.Views.Converter
{
    class LoadingServerListContextProcentConverter : IValueConverter
    {
        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is int)) return value;
            if (!(parameter is int)) return value;
            if ((int)parameter == 0) return 0;

            return (int)value / ((int)parameter * 1d);

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}