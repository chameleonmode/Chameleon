using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Chameleon.Common.ValueConverters
{
    public class FloatingHintOffsetCalculationConverter : IMultiValueConverter
    {
        private static readonly Point DefaultFloatingOffset = new Point(0, -16);
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length > 3 &&
                values[3] is Point floatingOffset &&
                IsType<Point>(targetType) &&
                floatingOffset != DefaultFloatingOffset)
            {
                return floatingOffset;
            }

            FontFamily fontFamily = (FontFamily)values[0];
            double fontSize = (double)values[1];
            double floatingScale = (double)values[2];

            double floatingHintHeight = fontFamily.LineSpacing * fontSize * floatingScale;

            if (IsType<Point>(targetType))
            {
                return new Point(0, -floatingHintHeight);
            }

            if (IsType<Thickness>(targetType))
            {
                return new Thickness(0, floatingHintHeight, 0, 0);
            }

            throw new NotSupportedException(targetType.FullName);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        private bool IsType<T>(Type type)
        {
            return type == typeof(T);
        }
    }
}
