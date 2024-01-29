using Chameleon.Interfaces.Bookmarks;
using System;
using System.Windows;
using System.Windows.Data;

namespace Chameleon.Common.ValueConverters
{
    [ValueConversion(typeof(string), typeof(string))]
    public class NameFolderToImageConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((value != null) && (value is BookmarkType) && !value.Equals(default(BookmarkType)))
            {
                switch (value)
                {
                    case BookmarkType.GlobalFolder:
                        return "Globe";
                    case BookmarkType.ProfileFolder:
                        return "Folder";
                    case BookmarkType.File:
                        return "FileOutline";
                }
            }
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
