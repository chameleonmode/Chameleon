using Chameleon.Common.Extensions;
using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace Chameleon.Common.ValueConverters
{
    public class IntegerToStringKiloConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(string))
            {
                throw new InvalidOperationException("The target must be a string");
            }
            var inputValue = (int)value;
            return inputValue.KiloFormat();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
