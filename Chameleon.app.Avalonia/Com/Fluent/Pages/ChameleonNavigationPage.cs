using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;

using Chameleon.Av.Fluent.Common.Controls;
using Chameleon.lib.Common.Models.Dto;
using Chameleon.lib.Common.Util;
using Chameleon.lib.CommunityToolkit.MvvM;

using FluentAvalonia.UI.Controls;
using FluentAvalonia.UI.Controls.Experimental;
using FluentAvalonia.UI.Navigation;

namespace Chameleon.Av.Fluent.Common.Pages;

public class ChameleonNavigationPage : AutoViewModelLocatorControl {
	private Visual? _animationPageParent;
	private Visual? _animationPage;
	private object? _navParam;

	public ChameleonNavigationPage()
	{
		// Use the frame events here to ensure ConnectedAnimations still work with
		// Back/Forward navigation and not just explicit page invokes
		AddHandler(Frame.NavigatingFromEvent, OnNavigatingFrom, RoutingStrategies.Direct);
		AddHandler(Frame.NavigatedToEvent, OnNavigatedTo, RoutingStrategies.Direct);

		//Tapped += OnPageTapped;
		//DoubleTapped += OnPageTapped;
	}

	public virtual void OnAfterNavigatedToViewModel(object param) { }
	public virtual void OnAfterNavigatedTo() { }
	private async void OnNavigatedTo(object? sender, NavigationEventArgs e)
	{
		if (DataContext is ViewModelObjectBase pageViewModel) {
			await Task.Delay(64);
			await pageViewModel.OnNavigatedToAsync(e.Parameter);
			OnAfterNavigatedToViewModel(e.Parameter);
		}
		if (_animationPage != null && _animationPageParent != null) {
			_ = ExUtil.TryCatch(() => {
				var svc = ConnectedAnimationService.GetForView(TopLevel.GetTopLevel(this));
				var anim = svc.GetAnimation("BackAnimation");

				if (anim == null)
					return false;

				GetNavAnimationVisuals(_navParam);

				if (_animationPage == null) return false;

				// In WinUI, ConnectedAnimation is somehow exempt from all clipping behaviors
				// Here, we are not, so disable ClipToBounds on all elements in the SettingsExpander
				// The rest are taken care of in the xaml.
				// NOTE: The ScrollViewer is not changed here as that's important for scrolling - thus
				// the animation will be cut off, but the back animation is pretty fast and mostly is
				// only visible closer to the element so we're ok, I think
				var x = _animationPage.GetVisualParent();
				while (x is not ScrollContentPresenter && x != null) {
					x.ClipToBounds = false;
					x = x.GetVisualParent();
				}

				anim.Configuration = new DirectConnectedAnimationConfiguration();
				_ = anim.TryStart(_animationPage);
				return true;
			});
		}

		OnAfterNavigatedTo();
	}

	private void OnNavigatingFrom(object? sender, NavigatingCancelEventArgs e)
	{
		_navParam = e.Parameter;

		GetNavAnimationVisuals(_navParam);

		if (_animationPage is not null) {
			var svc = ConnectedAnimationService.GetForView(TopLevel.GetTopLevel(this));
			try {
				_ = svc.PrepareToAnimate("ForwardAnimation", _animationPage);
			} catch {
				_ = svc.GetAnimation("ForwardAnimation");
				_animationPage = _animationPageParent = null;
			}
		}
	}

	private void GetNavAnimationVisuals(object? navParam)
	{
		_animationPage = _animationPageParent = null;
		
		if (navParam is string command) {
			_animationPageParent = this
				.GetVisualDescendants()?
				.Where(x => x is ICommandSource { CommandParameter: string cmd } && cmd == command)?
				.FirstOrDefault();
			_animationPage = _animationPageParent?
				.GetVisualDescendants()?
				.Where(x => x.Name == "IconHost")?
				.FirstOrDefault();
		} else if (navParam is UserProfileDto iprofile) {
			_animationPageParent = this
					.GetVisualDescendants()?
					.Where(x => x is ListBox && x.Name == "lbProfiles")?
					.FirstOrDefault();
			_animationPage = _animationPageParent?
					.GetVisualDescendants()?
					.Where(x => x is ListBoxItem b && b.DataContext is Vim<UserProfileDto> dc && dc.Dto?.id == iprofile.id)?
					.FirstOrDefault();
			if (_animationPage == null && _animationPageParent is ListBox l && l.Items.Count >= 10) {
					_animationPage = _animationPageParent?
						.GetVisualDescendants()?
						.Where(x => x is ListBoxItem b && b.DataContext is Vim<UserProfileDto>)?
					  .FirstOrDefault();
			}
			_animationPage ??= _animationPageParent;
		} else if (navParam is UPFolderDto f) {
			_animationPageParent = this
					.GetVisualDescendants()
					.Where(x => x.DataContext is Vim<UPFolderDto>)?
					.FirstOrDefault();
			_animationPage = _animationPageParent?
						.GetVisualDescendants()?
					 .Where(x => x.Name == "IconHost" && ((x as Control)?.Tag as UPFolderDto)?.id == f.id)?
					 .FirstOrDefault();
		}
	}
}
