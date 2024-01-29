using Chameleon.Interfaces.Country;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace Chameleon.Common.ValueConverters
{
    [ValueConversion(typeof(int), typeof(ICountry))]
    public class CountryIdToCountryConverter : IMultiValueConverter
    {
        private IEnumerable<ICountry> _countries;//HACK use cashed data of Convert operation for converting back

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if(values == null || values.Length < 2)
            {
                throw new InvalidOperationException("The count of bindable data can be > 2");
            }

            if (!(values[1] is IEnumerable<ICountry>))
            {
                return DependencyProperty.UnsetValue;
            }

            _countries = (IEnumerable<ICountry>)values[1];
            if ( (values[0] != null) && values[0].Equals(DependencyProperty.UnsetValue))
            {
                return DependencyProperty.UnsetValue;
            }

            var value = values[0];
            if (value != null && !(value is int?))
            {
                throw new InvalidOperationException("The first binding must be int type");
            }

            var inputValue = (int?)values[0];

            return _countries
                .Where(c => c.Id == inputValue)
                .FirstOrDefault();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            var selectedCountry = value as ICountry;
            if ((_countries == null) || selectedCountry == null)
            {
                return new[] { DependencyProperty.UnsetValue };
            }

            var country = _countries
                .Where(c => c.Id == selectedCountry.Id)
                .FirstOrDefault();

            var countryUiId = (country == default(ICountry)) 
                ? DependencyProperty.UnsetValue 
                : country.Id;

            return new[] { countryUiId };
        }
    }
}
