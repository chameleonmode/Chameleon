using Avalonia.Controls;

namespace Chameleon.app.Avalonia.Services;

public interface INavigatorService {
	void NavigateTo(string viewKey, object? parameter = null);
	void NavigateToType(Type viewType, object? parameter = null);
	void RegisterView(string viewKey, Type viewType);
	bool IsCurrentView(string viewKey);
}

public class NavigatorService : INavigatorService {

	private readonly Dictionary<string, Type> registeredViews = new(StringComparer.OrdinalIgnoreCase);

	public static bool CanGoBack => Navigator.Instance.Frame?.CanGoBack ?? false;
	public static ContentControl? Frame => Navigator.Instance.Frame;
	public static void GoBack() => Navigator.Instance.Frame?.GoBack();

	public void RegisterView(string viewKey, Type viewType) {
		if (string.IsNullOrWhiteSpace(viewKey))
			throw new ArgumentNullException(nameof(viewKey));
		ArgumentNullException.ThrowIfNull(viewType);
		if (!typeof(Control).IsAssignableFrom(viewType))
			throw new ArgumentException($"{viewType.FullName} must be assignable to Avalonia.Controls.Control", nameof(viewType));

		registeredViews[viewKey] = viewType;
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

	public bool IsCurrentView(string viewKey) {
		if (string.IsNullOrWhiteSpace(viewKey)) {
			return false;
		}

		var currentViewType = Navigator.Instance.Frame?.Content?.GetType();

		return registeredViews.TryGetValue(viewKey, out var expectedViewType)
				&& currentViewType is not null
				&& currentViewType == expectedViewType;
	}
}
