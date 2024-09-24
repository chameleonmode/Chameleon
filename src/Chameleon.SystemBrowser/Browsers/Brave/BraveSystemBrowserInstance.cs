using Chameleon.lib.Common.Enums;

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
