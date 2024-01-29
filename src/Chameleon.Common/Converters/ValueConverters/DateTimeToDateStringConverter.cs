using System;
using System.Windows;
using System.Windows.Data;

namespace Chameleon.Common.ValueConverters
{
    [ValueConversion(typeof(DateTime), typeof(string))]
    public class DateTimeToDateStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ( (value != null) && (value is DateTime) && !value.Equals(default(DateTime)) )
            {
                DateTime date = (DateTime)value;
                return date.ToString("yyyy.MM.dd");
            }
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
