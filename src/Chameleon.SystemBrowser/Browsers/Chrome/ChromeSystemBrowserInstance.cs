namespace Chameleon.SystemBrowser.Chrome;
public class ChromeSystemBrowserInstance(
    IEventAggregator eventAggregator,
    ISystemBrowserLaunchOptions options,
    ISetPreferencesService setPreferencesService,
    IApplicationEnvironment applicationEnvironment,
    IUserDefaultSettingsService userDefaultsSettingsService) : 
    SystemBrowserInstance(eventAggregator, options, userDefaultsSettingsService, applicationEnvironment.ApplicationDataFolderPath)
{

    protected override SystemBrowserType BrowserType => SystemBrowserType.Chrome;

    protected override async Task InitializeProfileFolder()
    {
        await Task.Run(() => setPreferencesService.SetPreferences(UserProfile.WebBrowser, BrowserProfileFolderPath, BrowserType));
    }
}