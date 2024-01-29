using Microsoft.Maui.Controls;
using Prism.Dialogs;
using System;
using System.Globalization;

namespace Chameleon.Common.ValueConverters
{
    public class ButtonResultToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = (ButtonResult)value;
            if (parameter == null)
            {
                return false;
            }

            var button = (ButtonResult)Enum.Parse(typeof(ButtonResult), (string)parameter);

            if (button == val)
            {
                return true;
            }

            if (button == ButtonResult.OK && val == ButtonResult.Yes)
            {
                return true;
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
