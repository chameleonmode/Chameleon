using Avalonia.Controls;

using Chameleon.lib.Common;

using FluentAvalonia.UI.Controls;
using FluentAvalonia.UI.Media.Animation;
using FluentAvalonia.UI.Navigation;

namespace Chameleon.app.Avalonia;
public class Navigator {
	private Panel? _overlayHost;

	public Frame? Frame { get; set; }

	public static void SetFrame(Frame f)
	{
		Instance.Frame = f;
	}

	public static void NavigateToType(Type t, object? parameter = null, NavigationTransitionInfo? transitionInfo = null)
	{
		Instance.NavigateToThisType(t, parameter, transitionInfo);
	}

	public static void Pop()
	{
		//TODO: implement other back possibilitys when they come up
		if (Instance.Frame?.CanGoBack == true && Instance.Frame.Content?.GetType().Name == "UserProfileIdentityView")
			Instance.Frame?.GoBack();
	}

	public void Navigate(Type t)
	{
		_ = (Frame?.Navigate(t));
	}
	public void NavigateToThisType(Type t, object? parameter = null, NavigationTransitionInfo? transitionInfo = null)
	{
		_ = (Frame?.NavigateToType(t, parameter, BuildTransitionInfo(transitionInfo)));
	}
	public void NavigateFromContext(object dataContext, NavigationTransitionInfo? transitionInfo = null)
	{
		_ = (Frame?.NavigateFromObject(dataContext, BuildTransitionInfo(transitionInfo)));
	}
	private static FrameNavigationOptions BuildTransitionInfo(NavigationTransitionInfo? transitionInfo = null)
	{
		return new FrameNavigationOptions {
			IsNavigationStackEnabled = true,
			TransitionInfoOverride = transitionInfo ?? new SuppressNavigationTransitionInfo()
		};
	}

	public void SetOverlayHost(Panel p)
	{
		_overlayHost = p;
	}
	public void ClearOverlay()
	{
		_overlayHost?.Children.Clear();
	}
	public void ShowControlDefinitionOverlay(Type targetType)
	{
		if (_overlayHost != null) {
			//(_overlayHost.Children[0] as ControlDefinitionOverlay).TargetType = targetType;
			//(_overlayHost.Children[0] as ControlDefinitionOverlay).Show();
		}
	}

	private Navigator()
	{
	}
	public static Navigator Instance { get; } = new Navigator();
}

public class NavigationFactory : INavigationPageFactory {
	public Control GetPage(Type srcType)
	{
		return IoC.GetService(srcType) as Control ?? throw new ArgumentNullException(nameof(srcType), "Could not resolve page from type");
	}

	public Control? GetPageFromObject(object target)
	{
		throw new NotImplementedException();
	}
}
