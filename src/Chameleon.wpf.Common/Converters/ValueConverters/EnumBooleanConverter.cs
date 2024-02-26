using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace Chameleon.Common.ValueConverters
{
    public class EnumBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var parameterString = parameter as string;
            if (parameterString == null)
            {
                return BindableProperty.UnsetValue;
            }

            if (Enum.IsDefined(value.GetType(), value) == false)
            {
                return BindableProperty.UnsetValue;
            }

            var parameterValue = Enum.Parse(value.GetType(), parameterString);
            return parameterValue.Equals(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var parameterString = parameter as string;
            if (parameterString == null)
            {
                return BindableProperty.UnsetValue;
            }

            return Enum.Parse(targetType, parameterString);
        }
    }
}
