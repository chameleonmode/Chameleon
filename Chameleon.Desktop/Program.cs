using Avalonia;

namespace Chameleon.Desktop;

class Program {
	// Initialization code. Don't use any Avalonia, third-party APIs or any
	// SynchronizationContext-reliant code before AppMain is called: things aren't initialized
	// yet and stuff might break.
	[STAThread]
	public static void Main(string[] args) => BuildAvaloniaApp()
			.StartWithClassicDesktopLifetime(args);

	// Avalonia configuration, don't remove; also used by visual designer.
	public static AppBuilder BuildAvaloniaApp() {
		// GC.KeepAlive(typeof(Avalonia.Svg.Skia.SvgImageExtension).Assembly);
		// GC.KeepAlive(typeof(Avalonia.Svg.Skia.Svg).Assembly);
		return AppBuilder.Configure<client.App>()
				.UsePlatformDetect()
				.WithInterFont()
				.UseSkia()
				.With(new MacOSPlatformOptions {
					DisableDefaultApplicationMenuItems = true,
				})
#if DEBUG
		.LogToTrace()
#endif
		;
	}
}
