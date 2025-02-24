using Avalonia.Controls;
using Avalonia.Interactivity;
using Chameleon.app.Features.Assistants.UserTaskforce;
using Chameleon.app.Features.Automation.Playwright;
using Chameleon.app.ViewModels;

using Chameleon.Av.Fluent.Common.Pages;

using FluentAvalonia.UI.Media.Animation;

namespace Chameleon.app.Views;
public partial class AutomationView : ChameleonNavigationPage {
	public AutomationView()
	{
		InitializeComponent();
		TabStrip1.SelectionChanged += TabStrip1SelectionChanged!;
	}
	public override void OnAfterNavigatedToViewModel(object param)
	{
		base.OnAfterNavigatedToViewModel(param);
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
		if (DataContext is not AutomationViewModel vm)
			return;

		_ = InnerNavFrame.Navigate(idx switch {
			0 => typeof(PlaywrightView),
			1 => typeof(UserTaskforceView),
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