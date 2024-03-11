using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Chrome;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Chameleon.Avalonia.FluentAvalonia.Views;
using FluentAvalonia.UI.Windowing;

namespace Chameleon.Avalonia.FluentAvalonia;

public partial class MainWindow : AppWindow
{
    public MainWindow()
    {
        AvaloniaXamlLoader.Load(this);

#if DEBUG
        this.AttachDevTools();
#endif

        SplashScreen = new MainAppSplashScreen(this);

        TitleBar.ExtendsContentIntoTitleBar = true;
        TitleBar.TitleBarHitTestType = TitleBarHitTestType.Complex;
    }

    internal class MainAppSplashScreen : IApplicationSplashScreen
    {
        public MainAppSplashScreen(MainWindow owner)
        {
            _owner = owner;
        }

        public string AppName { get; }
        public IImage AppIcon { get; }
        public object SplashScreenContent => new MainAppSplashContent();
        public int MinimumShowTime => 2000;

        public Action InitApp { get; set; }

        public Task RunTasks(CancellationToken cancellationToken)
        {
            if (InitApp == null)
                return Task.CompletedTask;

            return Task.Run(InitApp, cancellationToken);
        }

        private MainWindow _owner;
    }
}