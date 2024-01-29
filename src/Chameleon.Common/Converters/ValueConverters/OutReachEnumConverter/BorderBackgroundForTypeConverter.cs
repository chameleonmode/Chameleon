using Chameleon.Interfaces.OutReach;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Chameleon.Common.Converters.ValueConverters
{
    public class BorderBackgroundForTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var type = (OutReachLinkUrlType)value;

            Color color;
            switch (type)
            {
                case OutReachLinkUrlType.Social:
                    color = ConvertFromString("#FFF0FB");
                    break;
                case OutReachLinkUrlType.Blog:
                    color = ConvertFromString("#E5F8F0");
                    break;
                case OutReachLinkUrlType.Forum:
                    color = ConvertFromString("#FFF8E5");
                    break;
                case OutReachLinkUrlType.Comment:
                    color = ConvertFromString("#E3F0FF");
                    break;
                default:
                    color = ConvertFromString("#FFFFFF");
                    break;
            }

            return new SolidColorBrush(color);
        }

        private Color ConvertFromString(string color)
        {
            return (Color)ColorConverter.ConvertFromString(color);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
