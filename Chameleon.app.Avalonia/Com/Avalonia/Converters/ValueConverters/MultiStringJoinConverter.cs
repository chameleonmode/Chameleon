using Avalonia.Data.Converters;
using System.Globalization;

namespace Chameleon.Avalonia.Common.Converters.ValueConverters;

public class MultiStringJoinConverter : IMultiValueConverter
{
    public object Convert(IList<object?> values, Type targetType, object parameter, CultureInfo culture)
    {
        if (parameter != null)
        {
            return string.Join((string)parameter, values);
        }
        else
        {
            return string.Join(":", values);
        }
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        if (parameter != null)
        {
            return ((string)value).Split((char)parameter);
        }
        else
        {
            var str = ((string)value).Split(':');
            return str;
        }
    }
}
