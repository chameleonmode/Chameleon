namespace Chameleon.SystemBrowser.Browsers.Brave;
public class BraveSystemBrowser(
        IEventAggregator eventAggregator,
        IApplicationEnvironment applicationEnvironment,
        ISystemBrowserInfoManager systemBrowserInfoManager,
        ISetPreferencesService setPreferencesService,
        IUserDefaultSettingsService userDefaultsSettingsService)
    : SystemBrowserBase(eventAggregator, applicationEnvironment,setPreferencesService,userDefaultsSettingsService), 
    IBraveSystemBrowser
{
    public override ISystemBrowserInstance InitializeBrowser(ISystemBrowserLaunchOptions o)
    {
        return new BraveSystemBrowserInstance(
             EventAggregator,
             o,
             SetPreferencesService,
             ApplicationEnvironment,
             UserDefaultSettingsService);
    }

    protected override string GetBrowserExePath()
    {
        return systemBrowserInfoManager
            .FindByName("brave")
            .Path;
    }

    protected override string GetDirectoryPath()
    {
        throw new NotImplementedException();
    }

    protected override string GetSystemBrowserExePath() =>
        GetBrowserExePath();
}