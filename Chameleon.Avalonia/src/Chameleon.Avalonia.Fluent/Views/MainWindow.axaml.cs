using Avalonia;
using Avalonia.Markup.Xaml;
using Chameleon.Av.Fluent.Common.Startup;
using FluentAvalonia.UI.Windowing;

namespace Chameleon.Av.Fluent.Views;

public partial class MainWindow : AppWindow
{
    public MainWindow()
    {
        AvaloniaXamlLoader.Load(this);

#if DEBUG
        this.AttachDevTools();
#endif

        SplashScreen = new MainAppSplashScreen(new MainAppSplashContent());
        TitleBar.ExtendsContentIntoTitleBar = true;
        TitleBar.TitleBarHitTestType = TitleBarHitTestType.Complex;
    }
}