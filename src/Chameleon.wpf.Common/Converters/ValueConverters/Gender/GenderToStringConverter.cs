using Microsoft.Maui.Controls;
using System;
using System.Globalization;
using System.Windows;


namespace Chameleon.Common.ValueConverters
{
    public class GenderToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var inputValue = (int)value;
            var result = (Gender)inputValue;

            switch(result)
            {
                case Gender.Male:
                    return Properties.Resources.ResourceManager.GetString(Gender.Male.ToString());
                case Gender.Female:
                    return Properties.Resources.ResourceManager.GetString(Gender.Female.ToString());
                default:
                    return BindableProperty.UnsetValue;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
