using Chameleon.Common.Helpers;
using Chameleon.Interfaces.App.Automation.Playwright;
using Chameleon.Interfaces.Environments;
using Chameleon.Interfaces.Settings;
using Chameleon.Interfaces.WebBrowser;
using Chameleon.Prism.Events;
using Chameleon.SystemBrowser;
using Chameleon.SystemBrowser.Browsers.Brave;

namespace Chameleon.Playwright.Automation.Brave;
public class BravePlaywrightBrowser
    : BraveSystemBrowser
    , IBravePlaywrightBrowser
{
    private readonly IAutomationScriptHelper _automationScriptHelper;

    public BravePlaywrightBrowser(
        IEventAggregator eventAggregator,
        IApplicationEnvironment applicationEnvironment,
        ISystemBrowserInfoManager systemBrowserInfoManager,
        ISetPreferencesService setPreferencesService,
        IUserDefaultSettingsService userDefaultsSettingsService,
        IAutomationScriptHelper automationScriptHelper
        )
        : base(eventAggregator,
            applicationEnvironment,
            systemBrowserInfoManager,
            setPreferencesService,
            userDefaultsSettingsService)
    {
        _automationScriptHelper = automationScriptHelper;
    }

    public IPlaywrightBrowserInstance InitializeBrowser(IPlaywrightBrowserLaunchOptions o)
    {
        return new BravePlaywrightBrowserInstance(
            _eventAggregator,
            o,
            _setPreferencesService,
            _applicationEnvironment,
            _userDefaultsSettingsService,
            GetBrowserExePath(),
            _automationScriptHelper
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
    public override async Task<ISystemBrowserInstance> Open(ISystemBrowserLaunchOptions o)
    {
        throw new NotImplementedException();
    }
}
