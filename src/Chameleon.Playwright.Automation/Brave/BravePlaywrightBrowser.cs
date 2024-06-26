using Chameleon.Common.Helpers;
using Chameleon.Interfaces.App.Automation.Playwright;
using Chameleon.Interfaces.Environments;
using Chameleon.Interfaces.Settings;
using Chameleon.Interfaces.WebBrowser;
using Chameleon.Prism.Events;
using Chameleon.SystemBrowser;
using Chameleon.SystemBrowser.Browsers.Brave;

namespace Chameleon.Playwright.Automation.Brave;

public class BravePlaywrightBrowser(
        IEventAggregator eventAggregator,
        IApplicationEnvironment applicationEnvironment,
        ISystemBrowserInfoManager systemBrowserInfoManager,
        ISetPreferencesService setPreferencesService,
        IUserDefaultSettingsService userDefaultsSettingsService,
        string browserExeFilePath,
        IAutomationScriptHelper automationScriptHelper)
        : BraveSystemBrowser(eventAggregator,
            applicationEnvironment,
            systemBrowserInfoManager,
            setPreferencesService,
            userDefaultsSettingsService)

    , IBravePlaywrightBrowser
{
    public IPlaywrightBrowserInstance InitializeBrowser(IPlaywrightBrowserLaunchOptions o)
    {
        return new BravePlaywrightBrowserInstance(
            EventAggregator,
            o,
            SetPreferencesService,
            ApplicationEnvironment,
            UserDefaultSettingsService,
            browserExeFilePath,
            automationScriptHelper
            );
    }

    public virtual async Task<IPlaywrightBrowserInstance> Open(IPlaywrightBrowserLaunchOptions o)
    {
        IPlaywrightBrowserInstance browser = null;
        try
        {
            browser = InitializeBrowser(o);

            await browser.Open();
        }
        catch (Exception e)
        {
            await MesageBoxHelper.ShowErrorAsync("Error", e.Message);
        }

        return browser;
    }

    // use BraveSystemBrowser for call this method
    public override Task<ISystemBrowserInstance> Open(ISystemBrowserLaunchOptions o)
    {
        throw new NotImplementedException();
    }
}
