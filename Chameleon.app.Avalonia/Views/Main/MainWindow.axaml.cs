using Avalonia;
using Avalonia.Markup.Xaml;
using Chameleon.Av.Fluent.Common.Startup;
using FluentAvalonia.UI.Windowing;

namespace Chameleon.app.Avalonia.Views.Main;

public partial class MainWindow : AppWindow
{
    public MainWindow()
    {
        AvaloniaXamlLoader.Load(this);

#if DEBUG
        this.AttachDevTools();
#endif

        SplashScreen = new MainAppSplashScreen(new AppSplashContent());
        TitleBar.ExtendsContentIntoTitleBar = true;
        TitleBar.TitleBarHitTestType = TitleBarHitTestType.Complex;
    }
}