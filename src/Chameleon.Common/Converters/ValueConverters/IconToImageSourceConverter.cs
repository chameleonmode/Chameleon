using System;
using System.Drawing;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Chameleon.Common.ValueConverters
{
    [ValueConversion(typeof(Icon), typeof(ImageSource))]
    public class IconToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Icon icon = value as Icon;
            
            if (icon == null)
            {
                return null;
            }

            return Imaging.CreateBitmapSourceFromHIcon(
                   icon.Handle,
                   Int32Rect.Empty,
                   BitmapSizeOptions.FromWidthAndHeight(icon.Width, icon.Height));
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
