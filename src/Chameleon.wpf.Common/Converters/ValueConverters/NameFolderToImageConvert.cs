using Chameleon.Interfaces.Bookmarks;
using System;
using Microsoft.Maui.Controls;

namespace Chameleon.Common.ValueConverters
{
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
            return BindableProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
