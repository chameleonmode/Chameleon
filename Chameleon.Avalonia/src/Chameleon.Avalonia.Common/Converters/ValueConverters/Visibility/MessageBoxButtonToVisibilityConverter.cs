using Avalonia.Data.Converters;
using Chameleon.Avalonia.Prism.Interfaces.MessageBox;
using Chameleon.Interfaces.MessageBox;
using Prism.Services.Dialogs;
using System;
using System.Globalization;
using System.Windows;

namespace Chameleon.Common.ValueConverters
{
    public class MessageBoxButtonToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = (MessageBoxButton)value;
            if (parameter == null)
            {
                return false;
            }

            var button = (ButtonResult)Enum.Parse(typeof(ButtonResult), (string)parameter);
            switch (button)
            {
                case ButtonResult.OK:
                    {
                        if (val == MessageBoxButton.OKCancel || val == MessageBoxButton.OK ||
                            val == MessageBoxButton.YesNo || val == MessageBoxButton.YesNoCancel)
                        {
                            return true;
                        }

                        break;
                    }
                case ButtonResult.No:
                    {
                        if (val == MessageBoxButton.YesNo || val == MessageBoxButton.YesNoCancel)
                        {
                            return true;
                        }

                        break;
                    }
                case ButtonResult.Cancel:
                    {
                        if (val == MessageBoxButton.OKCancel || val == MessageBoxButton.YesNoCancel)
                        {
                            return true;
                        }

                        break;
                    }
            }

            return false;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
