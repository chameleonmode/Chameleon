using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace Chameleon.Common.ValueConverters
{
    public enum Gender
    {
        Unknown = 0,
        Male = 1,
        Female = 2
    }

    [ValueConversion(typeof(int), typeof(bool?))]
    public class GenderToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(parameter is Gender))
            {
                throw new InvalidOperationException("The parameter must be a Gender enum");
            }

            var gender = (Gender)parameter;
            var inputValue = (int)value;

            switch(inputValue)
            {
                case 1: return gender == Gender.Male;
                case 2: return gender == Gender.Female;
                default: return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool? isChecked = value as bool?;

            if((isChecked == null)||(!isChecked.HasValue))
            {
                return DependencyProperty.UnsetValue;
            }

            if (!(parameter is Gender) || !isChecked.Value)
            {
                return DependencyProperty.UnsetValue;
            }

            if (isChecked.Value)
            {
                var gender = (Gender)parameter;
                return (int)gender;
            }

            return DependencyProperty.UnsetValue;
        }
    }
}
