using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Chameleon.Av.Fluent.Common.Startup;
using Chameleon.Av.Fluent.ViewModels;
using FluentAvalonia.UI.Controls;
using FluentAvalonia.UI.Windowing;

namespace Chameleon.Av.Fluent.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);

        var vm = new MainViewViewModel();
        DataContext = vm;

        // On desktop, the window will call this during the splashscreen
        if (e.Root is AppWindow aw && aw.SplashScreen is MainAppSplashScreen mass)
        {
            mass.InitApp += () =>
            {
                InitializeNavigationPages();
            };
        }
        else
        {
            InitializeNavigationPages();
        }
    }

    private void InitializeNavigationPages()
    {

    }
}