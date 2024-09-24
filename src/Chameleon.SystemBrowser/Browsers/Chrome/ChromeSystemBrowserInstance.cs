using Chameleon.lib.Common.Enums;

namespace Chameleon.SystemBrowser.Chrome;
public class ChromeSystemBrowserInstance(
    IEventAggregator eventAggregator,
    ISystemBrowserLaunchOptions options,
    IApplicationEnvironment applicationEnvironment,
    IUserDefaultSettingsService userDefaultsSettingsService,
    string browserExeFilePath) :
    SystemBrowserInstance(eventAggregator, options, userDefaultsSettingsService, applicationEnvironment.ApplicationDataFolderPath, browserExeFilePath)
{

    protected override SystemBrowserType BrowserType => SystemBrowserType.Chrome;
}