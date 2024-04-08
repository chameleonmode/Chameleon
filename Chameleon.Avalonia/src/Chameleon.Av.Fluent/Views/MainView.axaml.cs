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
using Chameleon.Avalonia.Controls.UserProfilesView.ViewModels;
using Chameleon.Avalonia.Prism.Infrastructure.Services;
using Chameleon.Common.Helpers;
using Chameleon.Interfaces.Auth;
using Chameleon.Interfaces.Dashboard;
using Chameleon.Interfaces.Dialogs;
using Chameleon.Interfaces.Services;
using Chameleon.Interfaces.Settings;
using Chameleon.Interfaces.Startup;
using Chameleon.Interfaces.UserProfiles;
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

        App.OnFramworkInitComplete += OnFrameworkInit;

        if (e.Root is AppWindow aw && aw.SplashScreen is MainAppSplashScreen mass)
        {
            mass.InitApp += async () =>
            {
            };
        }
        else
        {
        }
    }

    public async void OnFrameworkInit(AppWindow aw) 
    {
        App.OnFramworkInitComplete -= OnFrameworkInit;

        var top = TopLevel.GetTopLevel(this); 
        
        _isDesktop = top is Window;

        // Initialize the WindowNotificationManager with the "TopLevel". Previously (v0.10), MainWindow
        var notifyService = ContainerServiceHelper.Resolve<IToastNotificationService>();
        notifyService.SetHostWindow(top);


        if (ContainerServiceHelper.Current.ContainerProvider is not null)
        {
            DataContext = ContainerServiceHelper.Resolve<MainViewViewModel>();

            await ContainerServiceHelper.Current.ContainerProvider.Resolve<IApplicationStartup>().RunAsync();
        }

        InitializeNavigationPages();

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
        object? dc = page.DataContext;

        string mainPage = null;

        if (dc.GetType() == typeof(DashboardViewModel))
        {
            mainPage = "Dashboard";
        }
        else if (dc.GetType().FullName.Contains("Chameleon.Avalonia.Controls.UserProfilesView") || dc.GetType().FullName.Contains("Chameleon.Avalonia.Controls.UserProfileView"))
        {
            mainPage = "Profiles";
        }
        else if (dc.GetType() == typeof(SettingsViewModel) || dc.GetType().FullName.Contains("Chameleon.Avalonia.Controls.Settings"))
        {
            mainPage = "Settings";
        }

        SetNVI((List<NavigationViewItemBase>)NavView.MenuItemsSource, mainPage);
        SetNVI((List<NavigationViewItemBase>)NavView.FooterMenuItemsSource, mainPage);


        if (FrameView.BackStackDepth > 0 && !NavView.IsBackButtonVisible)
        {
            AnimateContentForBackButton(true);
        }
        else if (FrameView.BackStackDepth == 0 && NavView.IsBackButtonVisible)
        {
            AnimateContentForBackButton(false);
        }
    }

    void SetNVI(List<NavigationViewItemBase> source, string mainPage)
    {
        foreach (NavigationViewItem nvi in source)
        {
            var set = false;
            if (nvi.Content is string t && t == mainPage) 
            {
                set = true;
                NavView.SelectedItem = nvi;
            }
            SetNVIIcon(nvi, set);
        }
    }

    private void SetNVIIcon(NavigationViewItem item, bool selected)
    {
        // Technically, yes you could set up binding and converters and whatnot to let the icon change
        // between filled and unfilled based on selection, but this is so much simpler 

        if (item == null)
            return;

        var t = item.Tag;

        if (t is MainPageModelBase m)
        {
            item.IconSource = this.TryFindResource(selected ? $"{m.IconKey}Filled" : m.IconKey, out var value) ?
                (IconSource)value : null;
        }
        else
        {

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

        var pages = new List<MainPageModelBase>()
        {
            new()
            {
                NavHeader = "Dashboard",
                IconKey = "HomeIcon",
            },
            new ()
            {
                NavHeader = "Profiles",
                IconKey = "ContactIcon",
            },
            new()
            {
                NavHeader = "Settings",
                IconKey = "SettingsIcon",
                ShowsInFooter = true,
            }
        };

        //UserControlPageBase chameleonPageBase = new UserControlPageBase();
        //ChameleonContentControl s = new ChameleonContentControl();
        Dispatcher.UIThread.Post(() =>
        {
            var headeritems = new List<NavigationViewItemBase>(2);
            var footeritems = new List<NavigationViewItemBase>(1);
            foreach (var page in pages)
            {
                var nvi = new NavigationViewItem
                {
                    Content = page.NavHeader,
                    Tag = page,
                    IconSource = (IconSource)this.FindResource(page.IconKey),
                };
                nvi.Classes.Add("SampleAppNav");

                if (page.ShowsInFooter)
                    footeritems.Add(nvi);
                else
                    headeritems.Add(nvi);
            }

            if (_isDesktop || OperatingSystem.IsBrowser())
            {                    
                //NavView.Classes.Add("SampleAppNav");
            }
            else
            {
                NavView.PaneDisplayMode = NavigationViewPaneDisplayMode.LeftMinimal;
            }

            NavView.MenuItemsSource = headeritems;
            NavView.FooterMenuItemsSource = footeritems;


            FrameView.NavigateFromObject((NavView.MenuItemsSource.ElementAt(0) as Control).Tag);
        });
    }
}