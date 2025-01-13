using System;

using Avalonia;

using Chameleon.app.Avalonia;
using Chameleon.app.client;

using Svga = Avalonia.Svg.Skia.Svg;
using SvgaImageExtension = Avalonia.Svg.Skia.SvgImageExtension;

namespace Chameleon.Avalonia.Desktop;

class Program {
	// Initialization code. Don't use any Avalonia, third-party APIs or any
	// SynchronizationContext-reliant code before AppMain is called: things aren't initialized
	// yet and stuff might break.
	[STAThread]
	public static void Main(string[] args) => BuildAvaloniaApp()
			.StartWithClassicDesktopLifetime(args);

	// Avalonia configuration, don't remove; also used by visual designer.
	public static AppBuilder BuildAvaloniaApp()
	{
		GC.KeepAlive(typeof(SvgaImageExtension).Assembly);
		GC.KeepAlive(typeof(Svga).Assembly);
		return AppBuilder.Configure<App>()
				//.UseAvaloniaNative()
				.UsePlatformDetect()
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
