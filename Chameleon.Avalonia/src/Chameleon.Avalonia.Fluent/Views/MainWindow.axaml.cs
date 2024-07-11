using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Chameleon.Av.Fluent.Common.Startup;
using Chameleon.Common.Helpers;
using Chameleon.Interfaces.Startup;
using Chameleon.Interfaces.Views;
using Chameleon.Interfaces.Windows;
using FluentAvalonia.UI.Windowing;

namespace Chameleon.Av.Fluent.Views;

public partial class MainWindow : AppWindow, IMainWindow
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

    public void SetContent(object content, string title = "TEMP")
    {
       // throw new NotImplementedException();
    }

    public void SetContent(string content)
    {
        //throw new NotImplementedException();
    }

    public void ShowWaitIndicator()
    {
       // throw new NotImplementedException();
    }

    public void HideWaitIndicator()
    {
        //throw new NotImplementedException();
    }

    public object GetContent()
    {
        throw new NotImplementedException();
    }

    public void SetContent(INavigationContent navigationContent)
    {
        throw new NotImplementedException();
    }
}