
using Chameleon.Interfaces.Services;
using Chameleon.Maui.Toolkit.Helpers;

namespace Chameleon.Maui.Toolkit.Services;
public class MauiNavigationService : INavigationService
{
    private readonly ISettingsService _settingsService;

    public MauiNavigationService(ISettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    public Task InitializeAsync() =>
        NavigateToAsync(
            string.IsNullOrEmpty(_settingsService.AuthAccessToken)
                ? "//Login"
                : "//Main/Catalog");

    public Task NavigateToAsync(string route, IDictionary<string, object>? routeParameters = null)
    {
        var shellNavigation = new ShellNavigationState(route);

        return routeParameters != null
            ? Shell.Current.GoToAsync(shellNavigation, routeParameters)
            : Shell.Current.GoToAsync(shellNavigation);
    }

    public async Task NavigateToAsync(Type viewModel)
    {
        await NavigateToAsync(PageViewModelRouting.Instance.GetPageRoute(viewModel));
    }

    public Task PopAsync() =>
        Shell.Current.GoToAsync("..");

}
