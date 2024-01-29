using Prism.Services.Dialogs;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Chameleon.Common.ValueConverters
{
    [ValueConversion(typeof(MessageBoxButton), typeof(Visibility))]
    public class MessageBoxButtonToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = (MessageBoxButton)value;
            if (parameter == null)
            {
                return Visibility.Collapsed;
            }

            var button = (ButtonResult)Enum.Parse(typeof(ButtonResult), (string)parameter);
            switch (button)
            {
                case ButtonResult.OK:
                    {
                        if (val == MessageBoxButton.OKCancel || val == MessageBoxButton.OK ||
                            val == MessageBoxButton.YesNo || val == MessageBoxButton.YesNoCancel)
                        {
                            return Visibility.Visible;
                        }

                        break;
                    }
                case ButtonResult.No:
                    {
                        if (val == MessageBoxButton.YesNo || val == MessageBoxButton.YesNoCancel)
                        {
                            return Visibility.Visible;
                        }

                        break;
                    }
                case ButtonResult.Cancel:
                    {
                        if (val == MessageBoxButton.OKCancel || val == MessageBoxButton.YesNoCancel)
                        {
                            return Visibility.Visible;
                        }

                        break;
                    }
            }

            return Visibility.Collapsed;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
