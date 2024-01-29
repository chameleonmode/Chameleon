using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Chameleon.Common.ValueConverters
{
    [ValueConversion(typeof(int), typeof(Visibility))]
    public class IntegerVisiblityInverseConverter : IntegerVisiblityConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var visibility = ConvertToVisibility(value);
            if (visibility == Visibility.Visible)
            {
                return Visibility.Collapsed;
            }
            return Visibility.Visible;
        }
    }
}
