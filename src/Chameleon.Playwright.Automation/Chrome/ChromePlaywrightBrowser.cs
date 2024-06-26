using Chameleon.Common.Helpers;
using Chameleon.Interfaces.App.Automation.Playwright;
using Chameleon.Interfaces.Environments;
using Chameleon.Interfaces.Settings;
using Chameleon.Interfaces.WebBrowser;
using Chameleon.Prism.Events;
using Chameleon.SystemBrowser;
using Chameleon.SystemBrowser.Chrome;

namespace Chameleon.Playwright.Automation.Chrome;
public class ChromePlaywrightBrowser(
        IEventAggregator eventAggregator,
        IApplicationEnvironment applicationEnvironment,
        ISystemBrowserInfoManager systemBrowserInfoManager,
        ISetPreferencesService setPreferencesService,
        IUserDefaultSettingsService userDefaultsSettingsService,
        IAutomationScriptHelper automationScriptHelper)
    : ChromeSystemBrowser(eventAggregator,
            applicationEnvironment,
            systemBrowserInfoManager,
            setPreferencesService,
            userDefaultsSettingsService)
    , IChromePlaywrightBrowser
{

    public IPlaywrightBrowserInstance InitializeBrowser(IPlaywrightBrowserLaunchOptions o)
    {
        return new ChromePlaywrightBrowserInstance(
            EventAggregator,
            o,
            SetPreferencesService,
            ApplicationEnvironment,
            UserDefaultSettingsService,
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

    // use ChromeSystemBrowser for call this method
    public override Task<ISystemBrowserInstance> Open(ISystemBrowserLaunchOptions o)
    {
        throw new NotImplementedException();
    }
}
