using Avalonia.Animation;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Styling;
using Chameleon.Av.Fluent.Common.Pages;
using FluentAvalonia.UI.Controls;
using FluentAvalonia.UI.Media.Animation;
using FluentAvalonia.UI.Windowing;
using Chameleon.app.Avalonia;
using Chameleon.app.Avalonia.Helpers;
using Chameleon.app.Avalonia.MvvM;
using Chameleon.app.Avalonia.Views;
using Chameleon.client.Features.ProfilesAndFolders.Projects;
namespace Chameleon.client;

public partial class View : UserControl {
	//TODO: move to load from json maybe?
	private readonly Dictionary<string, PageModelBase> _pages = new() {
		{
			"Dashboard",
			new()
			{
				NavHeader = "Dashboard",
				IconKey = "HomeIcon",
				Tag = typeof(Features.Dashboard.View)
			}
		},
		{
			"Profiles",
			new()
			{
				NavHeader = "Profiles",
				IconKey = "ContactIcon",
				Tag = typeof(ProjectsView)
			}
		},
		{
			"Automation",
			new()
			{
				NavHeader = "Automation",
				IconKey = "AutomationIcon",
				Tag = typeof(Features.Automation.View)
			}
		},
		{
			"Tenants",
			new()
			{
				NavHeader = "Tenant",
				IconKey = "TenantsIcon",
				Tag = typeof(Features.Tenants.View)
			}
		},
		{
			"General",
			new()
			{
				NavHeader = "General",
				IconKey = "CoreControlsIcon",
				ShowsInFooter = true,
				Tag = typeof(FunctionalSettingsView)
			}
		},
		{
			"Settings",
			new()
			{
				NavHeader = "Settings",
				IconKey = "SettingsIcon",
				ShowsInFooter = true,
				Tag = typeof(Features.Settings.View)
			}
		}
	};

	public View() {
		InitializeComponent();
	}

	protected override void OnLoaded(RoutedEventArgs e) {
		base.OnLoaded(e);

		if (VisualRoot is AppWindow aw) {
			TitleBarHost.ColumnDefinitions[3].Width = new GridLength(aw.TitleBar.RightInset, GridUnitType.Pixel);
		}
	}

	protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e) {
		base.OnAttachedToVisualTree(e);

		//
		Navigator.SetFrame(FrameView);
		TooltipManager.Attach(Application.Current!, NavView);

		//
		NavView.MenuItemsSource = _pages
			.Where(p => !p.Value.ShowsInFooter)
			.Select(a => a.Value.GetNavigationViewItemBase(this))
			.ToList();
		NavView.FooterMenuItemsSource = _pages
			.Where(p => p.Value.ShowsInFooter)
			.Select(a => a.Value.GetNavigationViewItemBase(this))
			.ToList();

		NavView.ItemInvoked += (s, e) => {
			// Change the current selected item back to normal
			SetNVIIcon(s as NavigationViewItem, false);

			if (e.InvokedItemContainer is NavigationViewItem nvi) {
				// Keep the frame navigation when not using connected animation but suppress it
				// if we have a connected animation binding two pages
				Navigator.NavigateToType(
					(nvi.Tag as PageModelBase)?.Tag!,
					null,
					FrameView.Content is ChameleonPageBase ? new SuppressNavigationTransitionInfo()
					: e.RecommendedNavigationTransitionInfo);
			}
		};
		NavView.BackRequested += (s, e) => FrameView.GoBack();

		//
		FrameView.NavigationPageFactory = Features.ViewModel.Instance.NavigationFactory;
		FrameView.Navigated += (s, e) => {
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
		};

		_ = FrameView.NavigateToType(_pages["Dashboard"].Tag, null, null);
	}

	private void SetNVIIcon(NavigationViewItem? item, bool selected) {
		// Technically, yes you could set up binding and converters and whatnot to let the icon change
		// between filled and unfilled based on selection, but this is so much simpler 
		if (item == null)
			return;

		if (item.Tag is PageModelBase m) {
			item.IconSource = this.TryFindResource(selected ? $"{m.IconKey}Filled" : m.IconKey, out var value) ?
					(IconSource)value! : null;
		} else {
			//TODO: :P
		}
	}
	private async void AnimateContentForBackButton(bool show) {
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

