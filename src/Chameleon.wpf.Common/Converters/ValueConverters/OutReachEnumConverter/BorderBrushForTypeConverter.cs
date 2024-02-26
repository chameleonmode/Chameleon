using Chameleon.Interfaces.OutReach;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Graphics.Converters;
using System;
using System.Globalization;

namespace Chameleon.Common.Converters.ValueConverters
{
    public class BorderBrushForTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var type = (OutReachLinkUrlType)value;

            Color color;
            switch (type)
            {
                case OutReachLinkUrlType.Social:
                    color = ConvertFromString("#EECFE6");
                    break;
                case OutReachLinkUrlType.Blog:
                    color = ConvertFromString("#D0EFE2");
                    break;
                case OutReachLinkUrlType.Forum:
                    color = ConvertFromString("#F9EAC2");
                    break;
                case OutReachLinkUrlType.Comment:
                    color = ConvertFromString("#C4DCF8");
                    break;
                default:
                    color = ConvertFromString("#FFFFFF");
                    break;
            }

            return new SolidColorBrush(color);
        }

        private Color ConvertFromString(string color) =>  new ColorTypeConverter().ConvertFromString(color) as Color;
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
