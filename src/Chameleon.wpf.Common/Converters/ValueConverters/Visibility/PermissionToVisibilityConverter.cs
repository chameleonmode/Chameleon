using System;
using System.Globalization;
using Chameleon.Interfaces.Auth;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace Chameleon.Common.ValueConverters
{
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
