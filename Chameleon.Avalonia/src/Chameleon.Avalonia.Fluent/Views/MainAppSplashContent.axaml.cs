using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Chameleon.Av.Fluent.Views;

public partial class MainAppSplashContent : UserControl
{
    public MainAppSplashContent()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        TargetProgressBar.IsIndeterminate = true;
    }
}