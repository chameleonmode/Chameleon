using Chameleon.Avalonia.Playwright.Automation.ExternalScript;
using Chameleon.Interfaces.App.Automation.Entities;
using Chameleon.Interfaces.App.Automation.Manager;
using Chameleon.Interfaces.App.Automation.Playwright;
using Chameleon.Interfaces.App.Automation.Services;
using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Avalonia.Playwright.Automation.Services;
public class AutomationBrowserService 
    : IAutomationBrowserService
{
    private readonly IPlaywrightBrowserManager _playwrightBrowserManager;
    private readonly ICompileScriptService _compileScriptService;
    private readonly IAutomationService _automationService;

    public AutomationBrowserService(
        IPlaywrightBrowserManager playwrightBrowserManager,
        ICompileScriptService compileScriptService,
        IAutomationService automationService
        )
    {
        _playwrightBrowserManager = playwrightBrowserManager;
        _compileScriptService = compileScriptService;
        _automationService = automationService;
    }

    public async void RunScript(
        IAutomationScriptDescription script, 
        IList<IUserProfile> userProfiles)
    {
        string scripBody = _automationService.GetScriptBody(script.Id);

        IExternalScript instance = (IExternalScript)_compileScriptService.CompileScript(scripBody);
        var browser = _playwrightBrowserManager.Get(instance.BrowserType);

        IDictionary<string, string> parameters = script.Parameters
            .Select(x => KeyValuePair.Create(x.Name, x.Value))
            .ToDictionary();

        foreach (IUserProfile profile in userProfiles)
        {
            var options = new PlaywrightBrowserLaunchOptions
            {
                UserProfile = profile
            };

            var browserInstance = (IPlaywrightBrowserInstanceWithContext) await browser.Open(options);

            await instance.Run(browserInstance.BrowserContext, parameters);
        }
    }
}
