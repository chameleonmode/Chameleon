using Avalonia;
using Avalonia.Controls;
using Avalonia.Animation;
using Avalonia.Interactivity;
using Avalonia.Styling;
using Avalonia.Threading;
using Chameleon.Av.Fluent.Common.Models;
using Chameleon.Av.Fluent.Common.Pages;
using FluentAvalonia.UI.Controls;
using FluentAvalonia.UI.Media.Animation;
using FluentAvalonia.UI.Navigation;
using FluentAvalonia.UI.Windowing;
using Chameleon.Avalonia.Common.Helpers;
using Avpplication = Avalonia.Application;
using Chameleon.lib.Common.ServiceManagers;
using Chameleon.app.Avalonia.ViewModels;
using Chameleon.lib.Common;
using Chameleon.Av.Fluent.Common.Startup;

namespace Chameleon.app.Avalonia.Views.Main;

public partial class MainView : UserControl {
	private readonly Dictionary<string, MainPageModelBase> _pages = new()
	{
		{
				"Dashboard",
				new()
				{
						NavHeader = "Dashboard",
						IconKey = "HomeIcon",
						Tag = typeof(Chameleon.app.Avalonia.Views.DashboardView)
				}
		},
		{
				"Profiles",
				new()
				{
						NavHeader = "Profiles",
						IconKey = "ContactIcon",
						Tag = typeof(Chameleon.app.Avalonia.Views.ProjectsView)
				}
		},
		{
				"Automation",
				new()
				{
						 NavHeader = "Automation",
						IconKey = "AutomationIcon",
						Tag = typeof(Chameleon.app.Avalonia.Views.PlaywrightView)
				}
		},
		{
				"General",
				new()
				{
						NavHeader = "General",
						IconKey = "CoreControlsIcon",
						ShowsInFooter = true,
						Tag = typeof(Chameleon.app.Avalonia.Views.FunctionalSettingsView)
				}
		},
		{
				"Settings",
				new()
				{
						NavHeader = "Settings",
						IconKey = "SettingsIcon",
						ShowsInFooter = true,
						Tag = typeof(Chameleon.app.Avalonia.Views.SettingsView)
				}
		}
	};
	public MainView()
	{
		InitializeComponent();
	}

	protected override void OnLoaded(RoutedEventArgs e)
	{
		base.OnLoaded(e);

		if (VisualRoot is AppWindow aw) {
			TitleBarHost.ColumnDefinitions[3].Width = new GridLength(aw.TitleBar.RightInset, GridUnitType.Pixel);
		}
	}

	protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
	{
		base.OnAttachedToVisualTree(e);
		TooltipManager.Attach(Avpplication.Current!, NavView);

		//if (e.Root is AppWindow aw && aw.SplashScreen is MainAppSplashScreen mass) {
		//	mass.InitApp += OnFrameworkInit;
		//} else {
		//	_ = OnFrameworkInit();
		//}
		OnFrameworkInit();
	}

	public void OnFrameworkInit()
	{
		DataContext = AppMainViewViewModel.Instance;

		Toaster.ShowSuccess("Welcome to Chameleon!");
		FrameView.NavigationPageFactory = AppMainViewViewModel.Instance.NavigationFactory;
		Navigator.SetFrame(FrameView);

		NavView.MenuItemsSource = _pages.Where(p => !p.Value.ShowsInFooter).Select(a => a.Value.GetNavigationViewItemBase(this)).ToList();
		NavView.FooterMenuItemsSource = _pages.Where(p => p.Value.ShowsInFooter).Select(a => a.Value.GetNavigationViewItemBase(this)).ToList();

		FrameView.Navigated += OnFrameViewNavigated;
		NavView.ItemInvoked += OnNavigationViewItemInvoked;
		NavView.BackRequested += OnNavigationViewBackRequested;

		_ = FrameView.NavigateToType(_pages["Dashboard"].Tag, null, null);
	}

	private void OnNavigationViewBackRequested(object? sender, NavigationViewBackRequestedEventArgs e)
	{
		FrameView.GoBack();
	}
	private void OnNavigationViewItemInvoked(object? sender, NavigationViewItemInvokedEventArgs e)
	{
		// Change the current selected item back to normal
		SetNVIIcon(sender as NavigationViewItem, false);

		if (e.InvokedItemContainer is NavigationViewItem nvi) {
			// Keep the frame navigation when not using connected animation but suppress it
			// if we have a connected animation binding two pages
			var info = FrameView.Content is ChameleonPageBase ?
					new SuppressNavigationTransitionInfo() :
					e.RecommendedNavigationTransitionInfo;

			Navigator.NavigateToType((nvi.Tag as MainPageModelBase)?.Tag!, null, info);
		}
	}
	private void OnFrameViewNavigated(object sender, NavigationEventArgs e)
	{
		var page = _pages
			.SingleOrDefault(p => p.Value.Tag?.Name == e.Content.GetType().Name).Value;

		foreach (var nvi in from NavigationViewItem nvi in 
													((List<NavigationViewItemBase>)NavView.MenuItemsSource).Concat((List<NavigationViewItemBase>)NavView.FooterMenuItemsSource)
												let set = nvi.Tag == page
												where set
												select nvi
		) {
			NavView.SelectedItem = nvi;
			SetNVIIcon(nvi, true);
		}

		if (FrameView.BackStackDepth > 0 && !NavView.IsBackButtonVisible) {
			AnimateContentForBackButton(true);
		} else if (FrameView.BackStackDepth == 0 && NavView.IsBackButtonVisible) {
			AnimateContentForBackButton(false);
		}
	}

	private void SetNVIIcon(NavigationViewItem? item, bool selected)
	{
		// Technically, yes you could set up binding and converters and whatnot to let the icon change
		// between filled and unfilled based on selection, but this is so much simpler 
		if (item == null)
			return;

		if (item.Tag is MainPageModelBase m) {
			item.IconSource = this.TryFindResource(selected ? $"{m.IconKey}Filled" : m.IconKey, out var value) ?
					(IconSource)value! : null;
		} else {
			//TODO: :P
		}
	}
	private async void AnimateContentForBackButton(bool show)
	{
		if (!WindowLogoIcon.IsVisible)
			return;

		if (show) {
			var ani = new Animation {
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

			await ani.RunAsync(WindowLogoIcon);

			NavView.IsBackButtonVisible = true;
		} else {
			NavView.IsBackButtonVisible = false;

			var ani = new Animation {
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

			await ani.RunAsync(WindowLogoIcon);
		}
	}
}