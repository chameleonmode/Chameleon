using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Styling;
using Avalonia.Threading;
using Chameleon.Av.Fluent.Common.Models;
using Chameleon.Av.Fluent.Common.Pages;
using Chameleon.Av.Fluent.Common.Services;
using Chameleon.Av.Fluent.ViewModels;
using Chameleon.Common.Helpers;
using Chameleon.Interfaces;
using Chameleon.Interfaces.App.Automation.Views;
using Chameleon.Interfaces.App.UserProfiles;
using Chameleon.Interfaces.Dashboard;
using Chameleon.Interfaces.Dialogs;
using Chameleon.Interfaces.Settings;
using Chameleon.Interfaces.Startup;
using FluentAvalonia.UI.Controls;
using FluentAvalonia.UI.Media.Animation;
using FluentAvalonia.UI.Navigation;
using FluentAvalonia.UI.Windowing;
using System.Xml.Linq;
using System.Linq;

namespace Chameleon.Av.Fluent.Views;

public partial class MainView : UserControl
{
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

        //if (e.Root is AppWindow aw && aw.SplashScreen is MainAppSplashScreen mass)
        //{
        //    mass.InitApp += async () =>
        //    {
        //    };
        //}
        //else
        //{
        //}
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
            DataContext = ContainerServiceHelper.Resolve<IMainViewViewModel>() as MainViewViewModel;

            await ContainerServiceHelper.Resolve<IApplicationStartup>().RunAsync();
        }

        InitializeNavigationPages();

        FrameView.Navigated += OnFrameViewNavigated;
        NavView.ItemInvoked += OnNavigationViewItemInvoked;
        NavView.BackRequested += OnNavigationViewBackRequested;
    }
    private void InitializeNavigationPages()
    {
        FrameView.NavigationPageFactory = NavigationService.Instance.NavigationFactory;
        NavigationService.Instance.SetFrame(FrameView);

        Dispatcher.UIThread.Post(() =>
        {
            NavView.MenuItemsSource = pages.Where(p=> !p.ShowsInFooter).Select(a => a.GetNavigationViewItemBase(this)).ToList(); 
            NavView.FooterMenuItemsSource = pages.Where(p => p.ShowsInFooter).Select(a => a.GetNavigationViewItemBase(this)).ToList(); 

            FrameView.NavigateToType(pages[0].Tag, null, null);

            //if (_isDesktop || OperatingSystem.IsBrowser())
            //{
            //}
            //else
            //{
            //    NavView.PaneDisplayMode = NavigationViewPaneDisplayMode.LeftMinimal;
            //}


            //FrameView.NavigateFromObject((NavView.MenuItemsSource.ElementAt(0) as Control).Tag);
        });
    }
    private void OnNavigationViewBackRequested(object? sender, NavigationViewBackRequestedEventArgs e)
    {
        FrameView.GoBack();
    }

    private void OnNavigationViewItemInvoked(object? sender, NavigationViewItemInvokedEventArgs e)
    {
        // Change the current selected item back to normal
        SetNVIIcon(sender as NavigationViewItem, false);

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

            NavigationService.Instance.NavigateToType((nvi.Tag as MainPageModelBase).Tag, info);
            //NavigationService.Instance.NavigateFromContext(nvi.Tag, info);
            //(ContainerServiceHelper.Resolve<INavigationService>() as NavigationService).NavigateFromContext(nvi.Tag, info);
        }
    }


    private void OnFrameViewNavigated(object sender, NavigationEventArgs e)
    {
        var page = pages.SingleOrDefault(p => p.Tag.Name[1..] == (e.Content as Control).GetType().Name);
        page ??= e.Content.GetType().FullName.StartsWith("Chameleon.Avalonia.Controls.Settings") ? pages[2] : pages[1];
        foreach (var nvi in from NavigationViewItem nvi in ((List<NavigationViewItemBase>)NavView.MenuItemsSource).Concat((List<NavigationViewItemBase>)NavView.FooterMenuItemsSource)
                            let set = nvi.Tag == page
                            where set
                            select nvi//nvi.IconSource = this.TryFindResource(set ? $"{page.IconKey}Filled" : page.IconKey, out var value) ? (IconSource)value : null;
        )
        {
            NavView.SelectedItem = nvi;
            SetNVIIcon(nvi, true);
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

    private bool _isDesktop;
    readonly List<MainPageModelBase> pages =
        [
            new()
            {
                NavHeader = "Dashboard",
                IconKey = "HomeIcon",
                Tag = typeof(IDashboardView)
            },
            new()
            {
                NavHeader = "Profiles",
                IconKey = "ContactIcon",
                Tag = typeof(IProjectsView)
            },
            new()
            {
                NavHeader = "Automation",
                IconKey = "AutomationIcon",
                Tag = typeof(IAutomationView)
            },
            new()
            {
                NavHeader = "Settings",
                IconKey = "SettingsIcon",
                ShowsInFooter = true,
                Tag = typeof(ISettingsView)
            }
        ];

}