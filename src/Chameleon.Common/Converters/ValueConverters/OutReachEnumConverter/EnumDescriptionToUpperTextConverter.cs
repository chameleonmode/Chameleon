using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;

namespace Chameleon.Common.Converters.ValueConverters
{
    public class EnumDescriptionToUpperTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var fi = value.GetType().GetField(value.ToString());
            if (fi != null)
            {
                var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
                return ((attributes.Length > 0) && (!String.IsNullOrEmpty(attributes[0].Description))) ? attributes[0].Description.ToUpper() : value.ToString().ToUpper();
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
