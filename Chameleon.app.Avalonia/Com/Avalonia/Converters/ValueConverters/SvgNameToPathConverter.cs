using Avalonia.Data.Converters;
using System.Globalization;

namespace Chameleon.Avalonia.Common.Converters.ValueConverters;

public class SvgNameToPathConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null || value.ToString() == string.Empty)
        {
            return null;
        }

        return $"avares://Chameleon.app.Avalonia/Assets/Svgs/{value}.svg";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
