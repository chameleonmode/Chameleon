using System;
using Avalonia;
using Avalonia.Controls;
using Svga = Avalonia.Svg.Skia.Svg;
using SvgaImageExtension = Avalonia.Svg.Skia.SvgImageExtension;
namespace Chameleon;

class Program
{
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
        return AppBuilder.Configure<Av.Fluent.App>()
            .UsePlatformDetect()
#if DEBUG
            .LogToTrace()
#endif
            //.With(new Win32PlatformOptions
            //{
            //    WinUICompositionBackdropCornerRadius = 20f
            //})
            .With(new X11PlatformOptions
            {
            })
            .With(new MacOSPlatformOptions
            {
                DisableDefaultApplicationMenuItems = true,
            })
            .UseSkia();
    }
}
