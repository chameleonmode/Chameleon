using Chameleon.Interfaces.Services;

namespace Chameleon.SystemBrowser.Browsers.Brave;
public class BraveSystemBrowserInstance(
        IEventAggregator eventAggregator,
        ISystemBrowserLaunchOptions options,
        ISetPreferencesService setPreferencesService,
        IApplicationEnvironment applicationEnvironment,
        IUserDefaultSettingsService userDefaultsSettingsService) :
    SystemBrowserInstance(eventAggregator, options, userDefaultsSettingsService, applicationEnvironment.ApplicationDataFolderPath)
{
    protected override SystemBrowserType BrowserType => SystemBrowserType.Brave;


    protected override async Task InitializeProfileFolder()
    {
        await Task.Run(() => setPreferencesService.SetPreferences(UserProfile.WebBrowser, BrowserProfileFolderPath, BrowserType));
    }
}
