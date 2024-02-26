using Chameleon.Interfaces.Country;
using Microsoft.Maui.Controls;
using System;
using System.Globalization;
using System.Windows;

namespace Chameleon.Common.ValueConverters
{
    public class CountryIdToCountryTitleConverter 
        : CountryIdToCountryConverter
        , IMultiValueConverter
    {
        public new object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var result = base.Convert(values, targetType, parameter, culture);

            if(result == null || result == BindableProperty.UnsetValue)
            {
                return BindableProperty.UnsetValue;
            }
            return ((ICountry)result).Name;
        }

        public new object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
