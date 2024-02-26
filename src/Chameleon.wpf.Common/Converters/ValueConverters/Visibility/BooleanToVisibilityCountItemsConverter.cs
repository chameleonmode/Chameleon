using Microsoft.Maui;
using Microsoft.Maui.Controls;
using System;
using System.Globalization;
using System.Linq;
using System.Windows;


namespace Chameleon.Common.ValueConverters
{
    public class BooleanToVisibilityCountItemsConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {            
            var showAll = (bool)values[1];
            if (showAll)
            {
                return Visibility.Visible;
            }

            var index = (int)values[0];
            var count = Int32.Parse(parameter.ToString());
            if (index < count)
            {
                return Visibility.Visible;
            }

            return Visibility.Collapsed;
        }

        public object[] ConvertBack(
            object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
