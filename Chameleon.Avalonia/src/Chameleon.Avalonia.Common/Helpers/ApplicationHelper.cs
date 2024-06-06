using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Primitives;
using Avalonia.Input.Platform;
using Avalonia.VisualTree;

namespace Chameleon.Avalonia.Common.Helpers;

public static class ApplicationHelper
{
    public static Window? GetMainWindow()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime && desktopLifetime.MainWindow != null)
            return desktopLifetime.MainWindow;

        return null;
    }

    public static OverlayLayer? GetOverlayLayer(TopLevel? topLevel = null)
    {
        OverlayLayer? ol;
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime al)
        {
            var windows = al.Windows;
            for (int i = 0; i < windows.Count; i++)
            {
                if (windows[i].IsActive)
                {
                    topLevel = windows[i];
                    break;
                }
            }

            if (topLevel == null)
            {
                if (al.MainWindow == null)
                    throw new NotSupportedException("No TopLevel root found to parent ContentDialog");
                topLevel = al.MainWindow;
            }

            ol = OverlayLayer.GetOverlayLayer(topLevel);
        }
        else if (Application.Current?.ApplicationLifetime is ISingleViewApplicationLifetime sl)
        {
            topLevel = TopLevel.GetTopLevel(sl.MainView);
            ol = OverlayLayer.GetOverlayLayer(sl.MainView);
        }
        else
        {
            throw new InvalidOperationException("No TopLevel found for GetMainOverlayLayer and no ApplicationLifetime is set. " +
                "Please either supply a valid ApplicationLifetime");
        }

        return ol;
    }

    public static T? FindResource<T>(object key)  where T : class
    {
        return Application.Current?.FindResource(key) as T ?? default;
    }

    public static T? TryGetResource<T>(object key) where T : class
    {
        if(Application.Current != null && 
            Application.Current.TryGetResource(key, null, out var icon) && 
            icon is T i)
            return i;

        return  default;
    }

    public static Visual? GetToplevetVisual()
    {
        return GetMainWindow()?.GetVisualRoot() as Visual;
    }

    public static IClipboard GetClipboard()
    {
        //Desktop
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime { MainWindow: { } window })
        {
            return window.Clipboard!;

        }

        //Android (and iOS?)
        else if (Application.Current?.ApplicationLifetime is ISingleViewApplicationLifetime { MainView: { } mainView })
        {
            var visualRoot = mainView.GetVisualRoot();
            if (visualRoot is TopLevel topLevel)
            {
                return topLevel.Clipboard!;
            }
        }

        return null!;
    }
}
