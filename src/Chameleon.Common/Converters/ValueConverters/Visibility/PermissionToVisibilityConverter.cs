using System;
using System.Windows;
using System.Globalization;
using System.Windows.Data;
using Chameleon.Interfaces.Auth;

namespace Chameleon.Common.ValueConverters
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class PermissionToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ConvertToVisibility(parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        protected Visibility ConvertToVisibility(object parameter)
        {
            var applicationUser = this.GetCurrentUser();

            if (applicationUser != null && !applicationUser.HasPemission(parameter.ToString()))
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }
    }
}
