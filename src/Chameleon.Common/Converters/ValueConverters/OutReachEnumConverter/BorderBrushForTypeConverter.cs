using Chameleon.Interfaces.OutReach;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

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
