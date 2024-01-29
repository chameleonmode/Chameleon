using Chameleon.Interfaces.OutReach;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Graphics.Converters;
using System;
using System.Globalization;

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

        private Color ConvertFromString(string color) => new ColorTypeConverter().ConvertFromString(color) as Color;


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
