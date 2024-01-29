using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Diagnostics;

namespace Chameleon.Common.Converters.ValueConverters
{
    [Flags]
    public enum DashboardTasks
    {
        [Description("Internal tasks")]
        InternalTasks = 0x2,

        [Description("Virtual Asisstant tasks")]
        VirtualAsisstantTasks = 0x4,

        [Description("OutReach Manager tasks")]
        OutReachManagerTasks = 0x8,

        [Description("All tasks")]
        AllTasks = InternalTasks | VirtualAsisstantTasks | OutReachManagerTasks,
    }

    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class DashboardTasksEnumToVisibilityConverter : IValueConverter
    {
        private const string Invert = "Invert";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var param = (string)parameter.ToString();
            var isInvert = param.EndsWith(Invert);
            param = param.Replace(Invert, "");

            var valueTask = (DashboardTasks)Enum.Parse(typeof(DashboardTasks), (string)value.ToString());
            var parameterTask = (DashboardTasks)Enum.Parse(typeof(DashboardTasks), param);

            var visibility = valueTask.HasFlag(parameterTask) ? Visibility.Visible : Visibility.Collapsed;

            if (isInvert && valueTask != DashboardTasks.AllTasks)
            {
                return visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            }
            return visibility;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility && (Visibility)value == Visibility.Visible)
            {
                return true;
            }
            return false;
        }
    }
}
