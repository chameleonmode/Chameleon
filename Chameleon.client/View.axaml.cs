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
using Chameleon.client.Features.Settings.Featured;
namespace Chameleon.client;
public class PageModelBase {
	public Type? Tag { get; set; }
	public string? NavHeader { get; set; }
	public string IconKey { get; set; } = "HomeIcon"; // Default to "HomeIcon
	public bool ShowsInFooter { get; set; }

	public NavigationViewItemBase GetNavigationViewItemBase(UserControl c)
	{
		var nvi = new NavigationViewItem {
			Content = NavHeader,
			Tag = this,
			IconSource = (IconSource)c.FindResource(IconKey)!,
		};
		nvi.Classes.Add("MainAppNav");
		return nvi;
	}
}
public partial class View : UserControl {
	//TODO: move to load from json maybe?
	private readonly Dictionary<string, PageModelBase> pages = new() {
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
				Tag = typeof(Features.Projects.ProjectsView)
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

		// Initialize the frame and navigation view
		Navigator.SetFrame(FrameView);
		// TooltipManager.Attach(Application.Current!, NavView);

		//
		NavView.MenuItemsSource = pages
		.Where(p => !p.Value.ShowsInFooter)
		.Select(a => a.Value.GetNavigationViewItemBase(this))
		.ToArray();
		NavView.FooterMenuItemsSource = pages
		.Where(p => p.Value.ShowsInFooter)
		.Select(a => a.Value.GetNavigationViewItemBase(this))
		.ToArray();

		NavView.ItemInvoked += (s, e) => {
			if (e.InvokedItemContainer is not NavigationViewItem nvi || nvi.Tag is not PageModelBase pageModel) return;

			// Change the current selected item back to normal
			SetNVIIcon(nvi);

			// Keep the frame navigation when not using connected animation but suppress it
			// if we have a connected animation binding two pages
			Navigator.NavigateToType(
				pageModel.Tag!,
				null,
				FrameView.Content is ChameleonPageBase
					? new SuppressNavigationTransitionInfo()
					: e.RecommendedNavigationTransitionInfo);
		};
		NavView.BackRequested += (s, e) => FrameView.GoBack();

		//
		FrameView.NavigationPageFactory = Features.ViewModel.Instance.NavigationFactory;
		FrameView.Navigated += (s, e) => {
			var page = pages
			.SingleOrDefault(p => p.Value.Tag?.FullName == e.Content.GetType().FullName).Value;

			if (page != null) {
				var nvi = ((IEnumerable<NavigationViewItemBase>)NavView.MenuItemsSource)
				.Concat((IEnumerable<NavigationViewItemBase>)NavView.FooterMenuItemsSource)
				.OfType<NavigationViewItem>()
				.FirstOrDefault(item => item.Tag == page);

				if (nvi != null) {
					NavView.SelectedItem = nvi;
					SetNVIIcon(nvi, true);
				}
			}

			NavView.IsBackButtonVisible = FrameView.BackStackDepth > 0;
			if (WindowLogoIcon.IsVisible) AnimateContentForBackButton();
		};

		_ = FrameView.NavigateToType(pages["Dashboard"].Tag, null, null);
	}

	private void SetNVIIcon(NavigationViewItem item, bool selected = true) {
		// Technically, yes you could set up binding and converters and whatnot to let the icon change
		// between filled and unfilled based on selection, but this is so much simpler 

		if (item.Tag is not PageModelBase m) return;

		item.IconSource = this.TryFindResource(selected ? $"{m.IconKey}Filled" : m.IconKey, out var value)
		? value as IconSource
		: null;
	}
	private async void AnimateContentForBackButton() {
		var startMargin = NavView.IsBackButtonVisible ? new Thickness(12, 4, 12, 4) : new Thickness(48, 4, 12, 4);
		var endMargin = NavView.IsBackButtonVisible ? new Thickness(48, 4, 12, 4) : new Thickness(12, 4, 12, 4);

		await new Animation {
			Duration = TimeSpan.FromMilliseconds(250),
			FillMode = FillMode.Forward,
			Children = {
				new KeyFrame {
					Cue = new Cue(0d),
					Setters = { new Setter(MarginProperty, startMargin) }
				},
				new KeyFrame {
					Cue = new Cue(1d),
					KeySpline = new KeySpline(0, 0, 0, 1),
					Setters = { new Setter(MarginProperty, endMargin) }
				}
			}
		}.RunAsync(WindowLogoIcon);
	}
}

