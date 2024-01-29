using System;
using System.Globalization;
using System.Windows.Data;

namespace Chameleon.Common.ValueConverters
{
    [ValueConversion(typeof(bool), typeof(bool))]
    public class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(bool) && targetType != typeof(bool?))
            {
                throw new InvalidOperationException("The target must be a boolean");
            }

            if (value == null)
            {
                return true;
            }

            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return true;
            }
            return !(bool)value;
        }
    }
}
