using Microsoft.Maui;
using System;
using System.Globalization;


namespace Chameleon.Common.ValueConverters
{
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
