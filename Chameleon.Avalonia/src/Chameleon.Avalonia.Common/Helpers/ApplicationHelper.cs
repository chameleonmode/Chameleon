using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.VisualTree;
using Chameleon.Interfaces.Windows;

namespace Chameleon.Avalonia.Common.Helpers;

public static class ApplicationHelper
{
    public static Window? GetMainWindow()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime && desktopLifetime.MainWindow != null)
            return desktopLifetime.MainWindow;

        return null;
    }

    public static T? FindResource<T>(object key)  where T : class
    {
        return Application.Current?.FindResource(key) as T ?? default;
    }

    public static Visual? GetToplevetVisual()
    {
        return GetMainWindow()?.GetVisualRoot() as Visual;
    }
}
