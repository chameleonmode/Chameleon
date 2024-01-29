using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Chameleon.Common.ValueConverters
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BooleanToVisibilityConverter : IValueConverter
    {
        enum Parameters
        {
            Normal,
            Inverted
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = (bool)value;
            if (parameter != null)
            {
                var direction = (Parameters)Enum.Parse(typeof(Parameters), (string)parameter);
                if (direction == Parameters.Inverted)
                {
                    return !val ? Visibility.Visible : Visibility.Collapsed;
                }
            }
            return val ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility && (Visibility)value == Visibility.Visible)
            {
                return true;
            }
            return false;
        }
    }
}
