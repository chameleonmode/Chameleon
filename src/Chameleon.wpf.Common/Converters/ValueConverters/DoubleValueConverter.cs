using Microsoft.Maui.Controls;
using System;
using System.Globalization;

namespace Chameleon.Common.ValueConverters
{
    public class DecimalConverter : IValueConverter
    {
        private static readonly NumberFormatInfo _numberFormatInfo = CultureInfo.GetCultureInfo("en-Us").NumberFormat;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(string))
            {
                throw new InvalidOperationException("The target must be a string");
            }
            var inputValue = (decimal)value;
            return inputValue.ToString("F2", _numberFormatInfo);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (decimal.TryParse(value as string, NumberStyles.Any,_numberFormatInfo, out var result))
            {
                return result;
            }
            return 0M;
        }
    }
}
