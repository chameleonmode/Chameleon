using Chameleon.lib.Common.Enums;

namespace Chameleon.SystemBrowser.Browsers.Brave;
public class BraveSystemBrowser(
        IEventAggregator eventAggregator,
        IApplicationEnvironment applicationEnvironment,
        ISystemBrowserInfoManager systemBrowserInfoManager,
        IUserDefaultSettingsService userDefaultsSettingsService)
    : SystemBrowserBase(eventAggregator, applicationEnvironment, userDefaultsSettingsService),
    IBraveSystemBrowser
{
    public override SystemBrowserType BrowserType => SystemBrowserType.Brave;

    public override ISystemBrowserInstance InitializeBrowser(ISystemBrowserLaunchOptions o)
    {
        return new BraveSystemBrowserInstance(
             EventAggregator,
             o,
             ApplicationEnvironment,
             UserDefaultSettingsService,
             GetBrowserExePath());
    }

    protected override string GetBrowserExePath()
    {
        return systemBrowserInfoManager
            .FindByName("brave")
            .Path;
    }

    protected override string GetSystemBrowserExePath()
    {
        throw new NotImplementedException();
    }

    protected override string GetDirectoryPath()
    {
        throw new NotImplementedException();
    }
}