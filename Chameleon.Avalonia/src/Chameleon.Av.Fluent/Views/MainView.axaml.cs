using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Chameleon.Av.Fluent.Common.Controls;
using Chameleon.Av.Fluent.Common.Models;
using Chameleon.Av.Fluent.Common.Pages;
using Chameleon.Av.Fluent.Common.Services;
using Chameleon.Av.Fluent.Common.Startup;
using Chameleon.Av.Fluent.ViewModels;
using FluentAvalonia.Core;
using FluentAvalonia.UI.Controls;
using FluentAvalonia.UI.Navigation;
using FluentAvalonia.UI.Windowing;

namespace Chameleon.Av.Fluent.Views;

public partial class MainView : UserControl
{
    private bool _isDesktop;

    public MainView()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        if (VisualRoot is AppWindow aw)
        {
            TitleBarHost.ColumnDefinitions[3].Width = new GridLength(aw.TitleBar.RightInset, GridUnitType.Pixel);
        }
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);

        _isDesktop = TopLevel.GetTopLevel(this) is Window;

        var vm = new MainViewViewModel();
        DataContext = vm;

        FrameView.NavigationPageFactory = NavigationService.Instance.NavFactory;
        NavigationService.Instance.SetFrame(FrameView);

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

       // FrameView.Navigated += OnFrameViewNavigated;
    }


    private void InitializeNavigationPages()
    {
        HomePageModel homePageModel = new HomePageModel()
        {
            NavHeader = "Dashboard", 
            IconKey = "HomeIcon",
        };
        //UserControlPageBase chameleonPageBase = new UserControlPageBase();
        //ChameleonContentControl s = new ChameleonContentControl();
        Dispatcher.UIThread.Post(() =>
        {

            NavView.MenuItemsSource = new List<NavigationViewItemBase>(1)
            {
              new NavigationViewItem
              {
                    Content = homePageModel.NavHeader,
                    Tag = homePageModel,
                    IconSource = (IconSource)this.FindResource(homePageModel.IconKey)
              }
            };

            NavView.FooterMenuItemsSource = new List<NavigationViewItemBase>(1)
            {
                new NavigationViewItem
                {
                      Content = "Settings",
                      Tag = null,
                      IconSource = (IconSource)this.FindResource("SettingsIcon")
                }
            };

            if (_isDesktop || OperatingSystem.IsBrowser())
            {
                NavView.Classes.Add("SampleAppNav");
            }
            else
            {
                NavView.PaneDisplayMode = NavigationViewPaneDisplayMode.LeftMinimal;
            }

            FrameView.NavigateFromObject((NavView.MenuItemsSource.ElementAt(0) as Control).Tag);
        });
    }
}