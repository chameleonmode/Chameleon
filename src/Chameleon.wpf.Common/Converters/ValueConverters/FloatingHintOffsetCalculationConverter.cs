using System;
using System.Globalization;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

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

            System.Drawing.FontFamily fontFamily = (System.Drawing.FontFamily)values[0];
            double fontSize = (double)values[1];
            double floatingScale = (double)values[2];
                                                 //TODO: look into
            double floatingHintHeight = fontFamily.GetLineSpacing(System.Drawing.FontStyle.Regular) * fontSize * floatingScale;

            if (IsType<Point>(targetType))
            {
                return new Point(0, (int)-floatingHintHeight);
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
