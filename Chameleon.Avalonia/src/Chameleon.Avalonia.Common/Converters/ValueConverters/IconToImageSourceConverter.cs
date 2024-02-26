using Avalonia.Data.Converters;
using System;
using System.Drawing;
using System.Windows;

namespace Chameleon.Common.ValueConverters
{
    public class IconToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Icon icon = value as Icon;
            
            if (icon == null)
            {
                return null;
            }

            return icon.ToBitmap();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
