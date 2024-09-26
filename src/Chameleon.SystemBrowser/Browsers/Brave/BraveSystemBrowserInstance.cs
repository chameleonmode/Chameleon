using Chameleon.lib.Common.Enums;
using Chameleon.SystemBrowser.Services;

namespace Chameleon.SystemBrowser.Browsers.Brave;
public class BraveSystemBrowserInstance(
        IEventAggregator eventAggregator,
        ISystemBrowserLaunchOptions options,
        IApplicationEnvironment applicationEnvironment,
        IUserDefaultSettingsService userDefaultsSettingsService,
        string browserExeFilePath) :
    SystemBrowserInstance(eventAggregator, options, userDefaultsSettingsService, applicationEnvironment.ApplicationDataFolderPath, browserExeFilePath)
{
    protected override SystemBrowserType BrowserType => SystemBrowserType.Brave;

}

//public class BraveSystemBrowserInstance(
//				IEventAggregator eventAggregator,
//				ISystemBrowserLaunchOptions options,
//				ISetPreferencesService setPreferencesService,
//				IApplicationEnvironment applicationEnvironment,
//				IUserDefaultSettingsService userDefaultsSettingsService) :
//		SystemBrowserInstance(eventAggregator, options, userDefaultsSettingsService, applicationEnvironment.ApplicationDataFolderPath) {
//	protected override SystemBrowserType BrowserType => SystemBrowserType.Brave;


//	protected override async Task InitializeProfileFolder()
//	{
//		await Task.Run(() => setPreferencesService.SetPreferences(UserProfile.WebBrowser, BrowserProfileFolderPath, BrowserType));
//	}
//}
