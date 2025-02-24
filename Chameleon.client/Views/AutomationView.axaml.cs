using Avalonia.Interactivity;
using Chameleon.client.Features.AI.ChameleonAIR;
using Chameleon.client.Features.Assistants.UserTaskforce;
using Chameleon.client.Features.Automation.Playwright;
using Chameleon.client.ViewModels;

using Chameleon.Av.Fluent.Common.Pages;

using FluentAvalonia.UI.Media.Animation;

namespace Chameleon.client.Views;
public partial class AutomationView : ChameleonNavigationPage {
	public AutomationView() {
		InitializeComponent();
		TabStrip1.SelectionChanged += (s, e) => NavigateToIndex(TabStrip1.SelectedIndex, s);
	}
	protected override void OnLoaded(RoutedEventArgs e) {
		base.OnLoaded(e);

		NavigateToIndex(TabStrip1.SelectedIndex, null);
	}

	private void NavigateToIndex(int index, object? parameter) {
		if (DataContext is AutomationViewModel vm) {
			_ = InnerNavFrame.Navigate(
			 index switch {
				 0 => typeof(PlaywrightView),
				 1 => typeof(UserTaskforceView),
				 2 => typeof(ChameleonAIRView),
				 _ => throw new Exception()
			 },
				parameter,
				new SlideNavigationTransitionInfo {
					Effect = vm.LastSelectedIndex < 0 ? SlideNavigationTransitionEffect.FromBottom
						: vm.LastSelectedIndex > index ? SlideNavigationTransitionEffect.FromRight : SlideNavigationTransitionEffect.FromLeft,
					FromHorizontalOffset = 70
				}
			);

			vm.LastSelectedIndex = TabStrip1.SelectedIndex;
		}
	}
}