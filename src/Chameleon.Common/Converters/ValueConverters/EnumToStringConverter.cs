using System;
using System.Globalization;
using System.Windows.Data;

namespace Chameleon.Common.Converters.ValueConverters
{
    public class EnumToStringConverter : IValueConverter
    {
        enum Parameters
        {
            ToLowerText,
            ToUpperText
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var enumString = Enum.GetName(value.GetType(), value);

            if(parameter != null)
            {
                var param = (Parameters)Enum.Parse(typeof(Parameters), (string)parameter);
                if(param == Parameters.ToLowerText)
                {
                    enumString = enumString.ToLower();
                }

                if (param == Parameters.ToUpperText)
                {
                    enumString = enumString.ToUpper();
                }
            }
            return enumString;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
