
using Avalonia.Interactivity;
using Avalonia.Controls.Primitives;

using FluentAvalonia.UI.Controls;
using FluentAvalonia.UI.Media.Animation;
using Avalonia.Controls;

namespace Chameleon.client.UI.Pages;

public abstract class TabStripNavigationPage : ChameleonNavigationPage {
	public abstract TabStrip Strip { get; }
	public abstract Frame Frame { get; }
	public abstract Type GetNavigationTarget(int index);

	public int LastSelectedIndex { get; set; } = -1;

	private void Selected_Changed(object? sender, SelectionChangedEventArgs e) {
		Navigate(Strip.SelectedIndex);
	}

	public override void OnAfterNavigatedToViewModel(object param) {
		base.OnAfterNavigatedToViewModel(param);
		Frame.NavigationPageFactory ??= client.ViewModel.Instance.NavigationFactory;
		if (LastSelectedIndex == -1) Navigate(0, param);
		else Strip.SelectedIndex = LastSelectedIndex;
		Strip.SelectionChanged += Selected_Changed;
 }
	protected override void OnLoaded(RoutedEventArgs e) {
		base.OnLoaded(e);
	}

	public override void NavigatingFrom(object? param) {
		base.NavigatingFrom(param);
		Strip.SelectionChanged -= Selected_Changed;
	}

	private void Navigate(int index, object? param = null) {
		_ = Frame.Navigate(GetNavigationTarget(index), param,
			new SlideNavigationTransitionInfo {
				Effect = LastSelectedIndex == 0 ? SlideNavigationTransitionEffect.FromBottom
				: LastSelectedIndex > index ? SlideNavigationTransitionEffect.FromRight
				: SlideNavigationTransitionEffect.FromLeft,
				FromHorizontalOffset = 70
			}
		);

		LastSelectedIndex = Strip.SelectedIndex;
	}
}