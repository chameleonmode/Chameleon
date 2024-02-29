using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Chameleon.Interfaces.Windows;

namespace Chameleon.Avalonia.Common.Helpers;

public static class ApplicationHelper
{
    public static Window GetMainWindow()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime && desktopLifetime.MainWindow != null)
            return desktopLifetime.MainWindow;

        throw new ArgumentNullException("MainWindow");
    } 
}
