using System;
using System.Globalization;
using System.Windows.Data;

namespace Chameleon.Common.ValueConverters
{
    [ValueConversion(typeof(string), typeof(string))]
    public class SvgNameToPathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || value.ToString() == string.Empty)
            {
                return null;
            }

            return $"pack://application:,,,/Chameleon.SvgIcons;component/Resource/{value}.svg";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
