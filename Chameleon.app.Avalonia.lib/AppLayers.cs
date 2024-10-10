using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.VisualTree;

namespace Chameleon.app.Avalonia.lib;

public static class AppLayers {
	public static Window? GetMainWindow()
	{
		//Should have been Implemented here
		//Desktop
		//if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime { MainWindow: { } window })
		//{
		//    return window!;

		//}

		//Android (and iOS?)
		//else if (Application.Current?.ApplicationLifetime is ISingleViewApplicationLifetime { MainView: { } mainView })
		//{
		//    var visualRoot = mainView.GetVisualRoot();
		//    if (visualRoot is TopLevel topLevel)
		//    {
		//        return topLevel.Clipboard!;
		//    }
		//}
		return Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime && desktopLifetime.MainWindow != null
			? desktopLifetime.MainWindow
			: null;
	}

	public static Visual? GetToplevetVisual()
	{
		return GetMainWindow()?.GetVisualRoot() as Visual;
	}
}
