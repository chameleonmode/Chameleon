using Avalonia.Controls;
using Avalonia.Interactivity;

using Chameleon.Av.Fluent.Common.Pages;
using Chameleon.client.Features.Projects.Folders;

using FluentAvalonia.UI.Media.Animation;

namespace Chameleon.client.Features.Settings.Featured;

[Chameleon.lib.Common.Attributes.ViewModel(typeof(FunctionalSettingsViewModel))]
public partial class FunctionalSettingsView : ChameleonNavigationPage {
	public FunctionalSettingsView()
	{
		InitializeComponent();
		ActiveTabStrip.SelectionChanged += TabStrip1SelectionChanged!;
	}
	public override void OnAfterNavigatedToViewModel(object param)
	{
		base.OnAfterNavigatedToViewModel(param);

		if (param is ObsFolder) {
			ActiveTabStrip.SelectionChanged -= TabStrip1SelectionChanged!;
			ActiveTabStrip.SelectedIndex = 2;
			NavigateToIndex(2, param);
			ActiveTabStrip.SelectionChanged += TabStrip1SelectionChanged!;
		}
	}

	protected override void OnLoaded(RoutedEventArgs e)
	{
		base.OnLoaded(e);

		TabStrip1SelectionChanged(null!, null!);
	}

	private void TabStrip1SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		NavigateToIndex(ActiveTabStrip.SelectedIndex, null!);
	}

	private void NavigateToIndex(int idx, object param)
	{
		if (DataContext is not FunctionalSettingsViewModel vm)
			return;

		_ = NavigationFrame.Navigate(idx switch {
			0 => typeof(UserDefaultSettingsView),
			1 => typeof(PhoneVerificationView),
			2 => typeof(UserProxySettingsView),
			3 => typeof(ProxyCreditView),
			_ => throw new Exception()
		}, param, GetTransitionInfo(vm.LastSelectedIndex, idx));

		vm.LastSelectedIndex = ActiveTabStrip.SelectedIndex;
	}

	private NavigationTransitionInfo GetTransitionInfo(int oldIndex, int newIndex)
	{
		SlideNavigationTransitionEffect GetEffect(int oldIndex, int index)
		{
			if (oldIndex < 0)
				return SlideNavigationTransitionEffect.FromBottom;

			if (oldIndex > index)
				return SlideNavigationTransitionEffect.FromRight;
			else
				return SlideNavigationTransitionEffect.FromLeft;
		}

		if (oldIndex == -1) {
			return new SuppressNavigationTransitionInfo();
		} else {
			return new SlideNavigationTransitionInfo {
				Effect = GetEffect(oldIndex, newIndex),
				FromHorizontalOffset = 70
			};
		}
	}
}