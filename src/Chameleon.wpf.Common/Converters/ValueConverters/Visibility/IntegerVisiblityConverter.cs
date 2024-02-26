using Microsoft.Maui;
using Microsoft.Maui.Controls;
using System;
using System.Globalization;
using System.Windows;


namespace Chameleon.Common.ValueConverters
{
    public class IntegerVisiblityConverter : IValueConverter
    {
        public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ConvertToVisibility(value);
        }
        
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        protected Visibility ConvertToVisibility(object value)
        {
            var number = System.Convert.ToInt32(value);
            if (number < 0)
            {
                return Visibility.Hidden;
            }
            if (number == 0)
            {
                return Visibility.Collapsed;
            }
            return Visibility.Visible;
        }
    }
}
