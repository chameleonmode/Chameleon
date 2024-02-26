using Microsoft.Maui.Controls;
using System;

namespace Chameleon.Common.ValueConverters
{
    public class DateTimeToDateStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ( (value != null) && (value is DateTime) && !value.Equals(default(DateTime)) )
            {
                DateTime date = (DateTime)value;
                return date.ToString("yyyy.MM.dd");
            }
            return BindableProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
