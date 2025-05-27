using Avalonia.Controls;
using Chameleon.lib;
using FluentAvalonia.UI.Controls;
using FluentAvalonia.UI.Media.Animation;
using FluentAvalonia.UI.Navigation;

namespace Chameleon.app.Avalonia;
public interface INavigatorService {
	void NavigateTo(string viewKey, object? parameter = null);
	void NavigateToType(Type viewType, object? parameter = null);
	void RegisterView(string viewKey, Type viewType);
	bool IsCurrentView(string viewKey);
	bool CanGoBack { get; }
	void GoBack();
}

public class Navigator: INavigatorService {

	private readonly Dictionary<string, Type> registeredViews = new(StringComparer.OrdinalIgnoreCase);

	private Panel? overlayHost;

	public Frame? Frame { get; private set; }
	public bool CanGoBack => Frame?.CanGoBack ?? false;
	public void GoBack() => Frame?.GoBack();

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
		overlayHost = p;
	}

	public void ClearOverlay()
	{
		overlayHost?.Children.Clear();
	}

	public void ShowControlDefinitionOverlay(Type targetType)
	{
		if (overlayHost != null) {
			//(_overlayHost.Children[0] as ControlDefinitionOverlay).TargetType = targetType;
			//(_overlayHost.Children[0] as ControlDefinitionOverlay).Show();
		}
	}

	public void NavigateTo(string viewKey, object? parameter = null) {
		if (string.IsNullOrWhiteSpace(viewKey))
			throw new ArgumentNullException(nameof(viewKey));

		if (registeredViews.TryGetValue(viewKey, out var viewType)) {
			Navigator.NavigateToType(viewType, parameter);
		} else {
			throw new ArgumentException($"No view registered with the key: {viewKey}", nameof(viewKey));
		}
	}

	public void NavigateToType(Type viewType, object? parameter = null) {
		Navigator.NavigateToType(viewType, parameter);
	}

	public void RegisterView(string viewKey, Type viewType) {
		if (string.IsNullOrWhiteSpace(viewKey))
			throw new ArgumentNullException(nameof(viewKey));
		ArgumentNullException.ThrowIfNull(viewType);
		if (!typeof(Control).IsAssignableFrom(viewType))
			throw new ArgumentException($"{viewType.FullName} must be assignable to Avalonia.Controls.Control", nameof(viewType));

		registeredViews[viewKey] = viewType;
	}

	public bool IsCurrentView(string viewKey) {
		if (string.IsNullOrWhiteSpace(viewKey)) {
			return false;
		}

		var currentViewType = Frame?.Content?.GetType();

		return registeredViews.TryGetValue(viewKey, out var expectedViewType)
				&& currentViewType is not null
				&& currentViewType == expectedViewType;
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
