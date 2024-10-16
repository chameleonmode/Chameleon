using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.VisualTree;

namespace Chameleon.app.Avalonia.app;

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

	public static T? FindResource<T>(object key) where T : class
	{
		return Application.Current?.FindResource(key) as T ?? default;
	}

	//TODO: implement
	//public static void InitializeExceptionHandlerLayer()
	//{
	//	AppDomain.CurrentDomain.UnhandledException += (s, e) =>
	//			OnUnhandledException(
	//					(Exception)e.ExceptionObject,
	//					nameof(AppDomain.CurrentDomain.UnhandledException)
	//					);

	//	//TODO: ? Application.Current.DispatcherUnhandledException += (s, e) =>
	//	//    e.Handled = OnUnhandledException(
	//	//        e.Exception,
	//	//        nameof(Application.Current.DispatcherUnhandledException)
	//	//        );

	//	TaskScheduler.UnobservedTaskException += (s, e) =>
	//	{
	//		e.SetObserved();
	//	};
	//}
}
