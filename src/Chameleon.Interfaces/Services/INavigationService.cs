namespace Chameleon.Interfaces.Services;
public interface INavigationService
{
    Task InitializeAsync();

    Task NavigateToAsync(string route, IDictionary<string, object> routeParameters = null);

    Task NavigateToAsync(Type viewModel);

    Task PopAsync();
}
