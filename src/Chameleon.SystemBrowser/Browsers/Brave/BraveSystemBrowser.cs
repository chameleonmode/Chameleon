namespace Chameleon.SystemBrowser.Browsers.Brave;
public class BraveSystemBrowser(
        IEventAggregator eventAggregator,
        IApplicationEnvironment applicationEnvironment,
        ISystemBrowserInfoManager systemBrowserInfoManager,
        ISetPreferencesService setPreferencesService,
        IUserDefaultSettingsService userDefaultsSettingsService) :
SystemBrowserBase(eventAggregator),  //TODO: fix?
IBraveSystemBrowser
{
    public override ISystemBrowserInstance InitializeBrowser(ISystemBrowserLaunchOptions o)
    {
        return new BraveSystemBrowserInstance(
             EventAggregator,
             o,
             setPreferencesService,
             applicationEnvironment,
             userDefaultsSettingsService,
             GetBrowserExePath());
    }

    protected string GetBrowserExePath()
    {
        return systemBrowserInfoManager
            .FindByName("brave")
            .Path;
    }
}