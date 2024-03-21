using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Chameleon.Av.Fluent.Common.Controls;
using Chameleon.Av.Fluent.Common.Models;
using Chameleon.Av.Fluent.Common.Pages;
using Chameleon.Av.Fluent.Common.Services;
using Chameleon.Av.Fluent.Common.Startup;
using Chameleon.Av.Fluent.Dialogs;
using Chameleon.Av.Fluent.ViewModels;
using Chameleon.Avalonia.Controls.Dashboard.ViewModels;
using Chameleon.Avalonia.Controls.Settings.ViewModels;
using Chameleon.Avalonia.Prism.Infrastructure.Services;
using Chameleon.Common.Helpers;
using Chameleon.Interfaces.Auth;
using Chameleon.Interfaces.Dashboard;
using Chameleon.Interfaces.Services;
using Chameleon.Interfaces.Startup;
using FluentAvalonia.Core;
using FluentAvalonia.UI.Controls;
using FluentAvalonia.UI.Media.Animation;
using FluentAvalonia.UI.Navigation;
using FluentAvalonia.UI.Windowing;
using System.Xml.Linq;

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



        //FrameView.NavigationPageFactory = NavigationService.Instance.NavFactory;
        //NavigationService.Instance.SetFrame(FrameView);

        // On desktop, the window will call this during the splashscreen
        if (e.Root is AppWindow aw && aw.SplashScreen is MainAppSplashScreen mass)// && mass.SplashScreenContent is MainAppSplashContent mas)
        {
            mass.InitApp += async () =>
            {
                var waited = 0;
                while (!App.FrameworkInitComplete && waited++ < 5)
                    await Task.Delay(500);

                //ContainerServiceHelper.Current.ContainerProvider
                //   .Resolve<IDashboardViewModel>();
                if (ContainerServiceHelper.Current.ContainerProvider is not null)
                {
                    DataContext = ContainerServiceHelper.Resolve<MainViewViewModel>();

                    await ContainerServiceHelper.Current.ContainerProvider.Resolve<IApplicationStartup>().RunAsync();
                }

                InitializeNavigationPages();
            };
        }
        else
        {
            InitializeNavigationPages();
        }

        FrameView.Navigated += OnFrameViewNavigated;
        NavView.ItemInvoked += OnNavigationViewItemInvoked;
        NavView.BackRequested += OnNavigationViewBackRequested;
    }
    private void OnNavigationViewBackRequested(object? sender, NavigationViewBackRequestedEventArgs e)
    {
        FrameView.GoBack();
    }

    private void OnNavigationViewItemInvoked(object? sender, NavigationViewItemInvokedEventArgs e)
    {
        // Change the current selected item back to normal
        // SetNVIIcon(sender as NavigationViewItem, false);

        if (e.InvokedItemContainer is NavigationViewItem nvi)
        {
            NavigationTransitionInfo info;

            // Keep the frame navigation when not using connected animation but suppress it
            // if we have a connected animation binding two pages
            if (FrameView.Content is ChameleonPageBase cpb)
            {
                info = new SuppressNavigationTransitionInfo();
            }
            else
            {
                info = e.RecommendedNavigationTransitionInfo;
            }

            NavigationService.Instance.NavigateFromContext(nvi.Tag, info);
            //(ContainerServiceHelper.Resolve<INavigationService>() as NavigationService).NavigateFromContext(nvi.Tag, info);
        }
    }


    private void OnFrameViewNavigated(object sender, NavigationEventArgs e)
    {
        var page = e.Content as Control;
        var dc = page.DataContext;

        //MainPageViewModelBase mainPage = null;

        //if (dc is MainPageViewModelBase mpvmb)
        //{
        //    mainPage = mpvmb;
        //}
        //else if (dc is PageBaseViewModel pbvm)
        //{
        //    mainPage = pbvm.Parent;
        //}
        //else if (page is ControlsPageBase cpb)
        //{
        //    mainPage = cpb.CreationContext.Parent;
        //}

        foreach (NavigationViewItem nvi in NavView.MenuItemsSource)
        {
            if (nvi.Tag.GetType() == typeof(HomePageModel) && dc.GetType() == typeof(DashboardViewModel))
            {
                NavView.SelectedItem = nvi;
                SetNVIIcon(nvi, true);
            }
            else
            {
                SetNVIIcon(nvi, false);
            }
        }

        foreach (NavigationViewItem nvi in NavView.FooterMenuItemsSource)
        {
            if (nvi.Tag.GetType() == typeof(SettingsPageModel) && dc.GetType() == typeof(SettingsViewModel))
            {
                NavView.SelectedItem = nvi;
                SetNVIIcon(nvi, true);
            }
            else
            {
                SetNVIIcon(nvi, false);
            }
        }

        if (FrameView.BackStackDepth > 0 && !NavView.IsBackButtonVisible)
        {
            AnimateContentForBackButton(true);
        }
        else if (FrameView.BackStackDepth == 0 && NavView.IsBackButtonVisible)
        {
            AnimateContentForBackButton(false);
        }
    }

    private void SetNVIIcon(NavigationViewItem item, bool selected)
    {
        // Technically, yes you could set up binding and converters and whatnot to let the icon change
        // between filled and unfilled based on selection, but this is so much simpler 

        if (item == null)
            return;

        var t = item.Tag;

        if (t is HomePageModel)
        {
            item.IconSource = this.TryFindResource(selected ? "HomeIconFilled" : "HomeIcon", out var value) ?
                (IconSource)value : null;
        }
        //else if (t is CoreControlsPageViewModel)
        //{
        //    item.IconSource = this.TryFindResource(selected ? "CoreControlsIconFilled" : "CoreControlsIcon", out var value) ?
        //        (IconSource)value : null;
        //}
        //else if (t is FAControlsOverviewPageViewModel)
        //{
        //    item.IconSource = this.TryFindResource(selected ? "FAControlsIconFilled" : "FAControlsIcon", out var value) ?
        //        (IconSource)value : null;
        //}
        //else if (t is DesignPageViewModel)
        //{
        //    item.IconSource = this.TryFindResource(selected ? "DesignIconFilled" : "DesignIcon", out var value) ?
        //        (IconSource)value : null;
        //}
        else if (t is SettingsPageModel)
        {
            item.IconSource = this.TryFindResource(selected ? "SettingsIconFilled" : "SettingsIcon", out var value) ?
               (IconSource)value : null;
        }
    }

    private async void AnimateContentForBackButton(bool show)
    {
        if (!WindowIcon.IsVisible)
            return;

        if (show)
        {
            var ani = new Animation
            {
                Duration = TimeSpan.FromMilliseconds(250),
                FillMode = FillMode.Forward,
                Children =
                {
                    new KeyFrame
                    {
                        Cue = new Cue(0d),
                        Setters =
                        {
                            new Setter(MarginProperty, new Thickness(12, 4, 12, 4))
                        }
                    },
                    new KeyFrame
                    {
                        Cue = new Cue(1d),
                        KeySpline = new KeySpline(0,0,0,1),
                        Setters =
                        {
                            new Setter(MarginProperty, new Thickness(48,4,12,4))
                        }
                    }
                }
            };

            await ani.RunAsync(WindowIcon);

            NavView.IsBackButtonVisible = true;
        }
        else
        {
            NavView.IsBackButtonVisible = false;

            var ani = new Animation
            {
                Duration = TimeSpan.FromMilliseconds(250),
                FillMode = FillMode.Forward,
                Children =
                {
                    new KeyFrame
                    {
                        Cue = new Cue(0d),
                        Setters =
                        {
                            new Setter(MarginProperty, new Thickness(48, 4, 12, 4))
                        }
                    },
                    new KeyFrame
                    {
                        Cue = new Cue(1d),
                        KeySpline = new KeySpline(0,0,0,1),
                        Setters =
                        {
                            new Setter(MarginProperty, new Thickness(12,4,12,4))
                        }
                    }
                }
            };

            await ani.RunAsync(WindowIcon);
        }
    }


    private void InitializeNavigationPages()
    {
        FrameView.NavigationPageFactory = ContainerServiceHelper.Resolve<INavigationService>().NavFactory as NavigationFactory;
        ContainerServiceHelper.Resolve<INavigationService>().SetFrame(FrameView);

        HomePageModel homePageModel = new()
        {
            NavHeader = "Dashboard", 
            IconKey = "HomeIcon",
        };

        SettingsPageModel settingsPageModel = new()
        {
            NavHeader = "Settings",
            IconKey = "SettingsIcon",
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
                      Content = settingsPageModel.NavHeader,
                      Tag = settingsPageModel,
                      IconSource = (IconSource)this.FindResource(settingsPageModel.IconKey)
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