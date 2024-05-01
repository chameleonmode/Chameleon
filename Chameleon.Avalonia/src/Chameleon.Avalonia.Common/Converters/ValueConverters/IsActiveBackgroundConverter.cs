using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Styling;
using System.Globalization;
using System.Xml;

namespace Chameleon.Avalonia.Common.Converters.ValueConverters;

public class IsActiveBackgroundConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {     
        var a = Application.Current.ActualThemeVariant;
        if (value is bool v && v)
        {
            if (Application.Current.TryGetResource("SystemFillColorSuccessBackgroundBrush", a, out object b))
                return b;
        }
        if (Application.Current.TryGetResource("SubtleFillColorTransparentBrush", a, out object accentbrush))
           return accentbrush;

        return Brushes.Transparent; 
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
