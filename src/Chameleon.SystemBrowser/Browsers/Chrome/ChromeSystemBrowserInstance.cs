namespace Chameleon.SystemBrowser.Chrome;
public class ChromeSystemBrowserInstance(
    IEventAggregator eventAggregator,
    ISystemBrowserLaunchOptions options,
    ISetPreferencesService setPreferencesService,
    IApplicationEnvironment applicationEnvironment,
    IUserDefaultSettingsService userDefaultsSettingsService,
    string browserExeFilePath) : 
    SystemBrowserInstance(eventAggregator, options, userDefaultsSettingsService, applicationEnvironment.ApplicationDataFolderPath, browserExeFilePath)
{

    protected override SystemBrowserType BrowserType => SystemBrowserType.Chrome;

    protected override async Task InitializeProfileFolder()
    {
        await Task.Run(() => setPreferencesService.SetPreferences(UserProfile.WebBrowser, BrowserProfileFolderPath, BrowserType));
    }
}

