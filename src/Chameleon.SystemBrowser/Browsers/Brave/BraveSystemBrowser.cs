namespace Chameleon.SystemBrowser.Browsers.Brave;
public class BraveSystemBrowser(
        IEventAggregator eventAggregator,
        IApplicationEnvironment applicationEnvironment,
        ISystemBrowserInfoManager systemBrowserInfoManager,
        ISetPreferencesService setPreferencesService,
        IUserDefaultSettingsService userDefaultsSettingsService) :
SystemBrowserBase,
IBraveSystemBrowser
{
    public override ISystemBrowserInstance InitializeBrowser(ISystemBrowserLaunchOptions o)
    {
        return new BraveSystemBrowserInstance(
             eventAggregator,
             o,
             setPreferencesService,
             applicationEnvironment,
             userDefaultsSettingsService,
             GetBrowserExePath());
    }

    private string GetBrowserExePath()
    {
        return systemBrowserInfoManager
            .FindByName("brave")
            .Path;
    }
}