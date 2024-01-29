using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Chameleon.Common.ValueConverters
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class InverseBooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = (bool)value;
            return val ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is Visibility && (Visibility)value == Visibility.Visible)
            {
                return false;
            }
            return true;
        }
    }
}
