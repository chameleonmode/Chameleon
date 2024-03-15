using Avalonia.Media;
using FluentAvalonia.UI.Windowing;

namespace Chameleon.Av.Fluent.Common.Startup;

public class MainAppSplashScreen(object splashScreenContent) : IApplicationSplashScreen
{
    public string? AppName { get; }
    public IImage? AppIcon { get; }
    public object SplashScreenContent { get; } = splashScreenContent;
    public int MinimumShowTime => 2000;

    public Func<Task>? InitApp { get; set; }

    public async Task RunTasks(CancellationToken cancellationToken)
    {
        //if (InitApp == null)
        //    return Task.CompletedTask;

        //return Task.Run(InitApp, cancellationToken);
        //await Task.Delay(10000);

        if (InitApp != null)
            await InitApp.Invoke();
    }
}
