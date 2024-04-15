using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Platform;
using Svg;
using Svg.Skia;
using System;
using System.Globalization;

namespace Chameleon.Avalonia.Common.Converters.ValueConverters;

public class SvgNameToDataConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null || value.ToString() == string.Empty)
        {
            return null;
        }
        var uri = $"avares://Chameleon.Avalonia.Common/Assets/Svgs/{value}.svg";
        using var stream = AssetLoader.Open(new Uri(uri));
        using var reader = new StreamReader(stream);
        var data = reader.ReadToEnd();

       //Application.Current.TryGetResource();
        return data;
    //  var d = SvgDocument.Open<SvgDocument>(AssetLoader.Open(new Uri(uri)));
        /// using var svg = new SKSvg();
        //   var pic = svg.Load(AssetLoader.Open(new Uri(uri)));
        //  return d.Content;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
