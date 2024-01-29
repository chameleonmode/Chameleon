using Chameleon.Interfaces.OutReach;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Chameleon.Common.Converters.ValueConverters
{
    public class ForegroundForStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var type = (OutReachLinkStatus)value;

            Color color;
            switch (type)
            {
                case OutReachLinkStatus.Live:
                    color = ConvertFromString("#00AE65");
                    break;
                default:
                    color = ConvertFromString("#7B7B7B");
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
