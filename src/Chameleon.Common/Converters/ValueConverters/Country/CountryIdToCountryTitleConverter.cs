using Chameleon.Interfaces.Country;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Chameleon.Common.ValueConverters
{
    [ValueConversion(typeof(int), typeof(string))]
    public class CountryIdToCountryTitleConverter 
        : CountryIdToCountryConverter
        , IMultiValueConverter
    {
        public new object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var result = base.Convert(values, targetType, parameter, culture);

            if(result == null || result == DependencyProperty.UnsetValue)
            {
                return DependencyProperty.UnsetValue;
            }
            return ((ICountry)result).Name;
        }

        public new object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
