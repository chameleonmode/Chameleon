using System;
using System.Globalization;
using System.Windows.Data;

namespace Chameleon.Common.ValueConverters
{
    [ValueConversion(typeof(int), typeof(string))]
    public class IntegerConverter : IValueConverter
    {
        enum Parameters
        {
            Normal,
            ZeroAsEmpty
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(string))
            {
                throw new InvalidOperationException("The target must be a string");
            }

            var inputValue = (int)value;
            if (inputValue != 0)
            {
                return inputValue.ToString();
            }

            var param = GetParameters(parameter);
            if (param == Parameters.ZeroAsEmpty)
            {
                return string.Empty;
            }
            return inputValue.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int.TryParse(value as string, out var result);
            return result;
        }

        private Parameters GetParameters(object parameter)
        {
            if (Enum.TryParse<Parameters>(parameter?.ToString(), out var param))
            {
                return param;
            }
            return Parameters.Normal;
        }
    }
}
