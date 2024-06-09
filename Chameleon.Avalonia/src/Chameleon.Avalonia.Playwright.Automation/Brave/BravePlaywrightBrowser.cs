using Chameleon.Avalonia.Playwright.Automation.Chrome;
using Chameleon.Common.Helpers;
using Chameleon.Interfaces.App.Automation.Playwright;
using Chameleon.Interfaces.Environments;
using Chameleon.Interfaces.Settings;
using Chameleon.Interfaces.WebBrowser;
using Chameleon.Prism.Events;
using Chameleon.SystemBrowser;
using Chameleon.SystemBrowser.Browsers.Brave;
using Chameleon.SystemBrowser.Services;

namespace Chameleon.Avalonia.Playwright.Automation.Brave;
public class BravePlaywrightBrowser
    : BraveSystemBrowser
    , IBravePlaywrightBrowser
{
    public BravePlaywrightBrowser(
        IEventAggregator eventAggregator,
        IApplicationEnvironment applicationEnvironment,
        ISystemBrowserInfoManager systemBrowserInfoManager,
        ISetPreferencesService setPreferencesService,
        IUserDefaultSettingsService userDefaultsSettingsService)
        : base(eventAggregator,
            applicationEnvironment,
            systemBrowserInfoManager,
            setPreferencesService,
            userDefaultsSettingsService)
    {
    }

    public IPlaywrightBrowserInstance InitializeBrowser(IPlaywrightBrowserLaunchOptions o)
    {
        return new BravePlaywrightBrowserInstance(
            _eventAggregator,
            o,
            _setPreferencesService,
            _applicationEnvironment,
            _userDefaultsSettingsService,
            GetBrowserExePath()
            );
    }

    public virtual async Task<IPlaywrightBrowserInstance> Open(IPlaywrightBrowserLaunchOptions o)
    {
        IPlaywrightBrowserInstance browser = null;
        try
        {
            browser = await Task.Run(() => InitializeBrowser(o));

            await browser.Open();
        }
        catch (Exception e)
        {
            await MesageBoxHelper.ShowErrorAsync("Error", e.Message);
        }

        return browser;
    }

    public override async Task<ISystemBrowserInstance> Open(ISystemBrowserLaunchOptions o)
    {
        throw new NotImplementedException();
    }
}
