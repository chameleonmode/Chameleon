using System;
using System.ComponentModel;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace Chameleon.Common.ValueConverters
{
    public class ListSortDirectionToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is ListSortDirection)
            {
                switch (value)
                {
                    case ListSortDirection.Ascending:
                        return "Name A → Z";
                    case ListSortDirection.Descending:
                        return "Name Z → A";
                }
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
