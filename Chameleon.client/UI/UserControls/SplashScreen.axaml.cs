using Avalonia.Controls;
using Avalonia.Media;
using FluentAvalonia.UI.Windowing;

namespace Chameleon.client.UI.UserControls;

public partial class SplashScreen : UserControl, IApplicationSplashScreen {
    public SplashScreen() {
        InitializeComponent();
        SplashScreenContent = this;
    }
    public string? AppName { get; }
    public IImage? AppIcon { get; }
    public object SplashScreenContent { get; }
    public int MinimumShowTime => 2000;

    public Func<Task>? InitApp { get; set; }

    public async Task RunTasks(CancellationToken cancellationToken) {
        if (InitApp != null)
            await InitApp.Invoke();
    }
}

