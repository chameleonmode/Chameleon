
using Avalonia.Interactivity;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

using FluentAvalonia.UI.Controls;
using FluentAvalonia.UI.Media.Animation;

using Chameleon.Av.Fluent.Common.Pages;

namespace Chameleon.client.FluentUI.Pages;

public abstract class TabStripNavigationPage : ChameleonNavigationPage {
	public abstract TabStrip Strip { get; }
	public abstract Frame Frame { get; }
	public abstract Type GetNavigationTarget(int index);

	public int LastSelectedIndex { get; set; } = -1;

	public void SetEvents() {
		Strip.SelectionChanged += (sender, e) => {
			Navigate(Strip.SelectedIndex, null);
		};
	}

	protected override void OnLoaded(RoutedEventArgs e) {
		base.OnLoaded(e);
		Navigate(Strip!.SelectedIndex, null);
	}

	private void Navigate(int index, object? parameter) {
		_ = Frame?.Navigate(GetNavigationTarget(index), parameter,
			new SlideNavigationTransitionInfo {
				Effect = LastSelectedIndex < 0 ? SlideNavigationTransitionEffect.FromBottom
					: LastSelectedIndex > index ? SlideNavigationTransitionEffect.FromRight : SlideNavigationTransitionEffect.FromLeft,
				FromHorizontalOffset = 70
			}
		);

		LastSelectedIndex = Strip!.SelectedIndex;
	}
}