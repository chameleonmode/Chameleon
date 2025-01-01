using Avalonia.Controls;
using Avalonia.Interactivity;

using Chameleon.app.Avalonia.Models.Observable;
using Chameleon.app.Avalonia.ViewModels;
using Chameleon.Av.Fluent.Common.Pages;

using FluentAvalonia.UI.Media.Animation;

namespace Chameleon.app.Avalonia.Views;

[Chameleon.lib.Common.Attributes.ViewModel(typeof(FunctionalSettingsViewModel))]
public partial class FunctionalSettingsView : ChameleonNavigationPage {
	public FunctionalSettingsView()
	{
		InitializeComponent();
		TabStrip1.SelectionChanged += TabStrip1SelectionChanged!;
	}
	public override void OnAfterNavigatedToViewModel(object param)
	{
		base.OnAfterNavigatedToViewModel(param);

		if (param is ObsFolder) {
			TabStrip1.SelectionChanged -= TabStrip1SelectionChanged!;
			TabStrip1.SelectedIndex = 2;
			NavigateToIndex(2, param);
			TabStrip1.SelectionChanged += TabStrip1SelectionChanged!;
		}
	}

	protected override void OnLoaded(RoutedEventArgs e)
	{
		base.OnLoaded(e);

		TabStrip1SelectionChanged(null!, null!);
	}

	private void TabStrip1SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		NavigateToIndex(TabStrip1.SelectedIndex, null!);
	}

	private void NavigateToIndex(int idx, object param)
	{
		if (DataContext is not FunctionalSettingsViewModel vm)
			return;

		_ = InnerNavFrame.Navigate(idx switch {
			0 => typeof(UserDefaultSettingsView),
			1 => typeof(PhoneVerificationView),
			2 => typeof(UserProxySettingsView),
			3 => typeof(ProxyCreditView),
			4 => typeof(AssistanTaskforceView),
			_ => throw new Exception()
		}, param, GetTransitionInfo(vm.LastSelectedIndex, idx));

		vm.LastSelectedIndex = TabStrip1.SelectedIndex;
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