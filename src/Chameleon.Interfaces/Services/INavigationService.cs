namespace Chameleon.Interfaces.Services;
public interface INavigationService
{
    Task InitializeAsync();

    Task NavigateToAsync(string route, IDictionary<string, object> routeParameters = null);

    Task NavigateToAsync(Type viewModel);

    Task PopAsync();

    object? NavFactory { get; }
    object? PreviousPage { get; set; }
    void SetFrame(object f); //TODO: change to actual
    void SetOverlayHost(object p); //TODO: change to actual
    void Navigate(Type t);
    void NavigateToType(Type t, object? parameter = null);
    void NavigateFromContext(object dataContext);
    void ClearOverlay();
}
