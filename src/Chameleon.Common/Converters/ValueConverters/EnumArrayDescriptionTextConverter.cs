using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace Chameleon.Common.Converters.ValueConverters
{
    public class EnumArrayDescriptionTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is Array)
            {
                List<string> list = new List<string>();
                var array = value as Array;
                foreach (var item in array)
                {
                    var fi = item.GetType().GetField(item.ToString());
                    if (fi != null)
                    {
                        var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
                        list.Add(attributes[0].Description);
                    }
                }
                return list;
            }
            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


}
