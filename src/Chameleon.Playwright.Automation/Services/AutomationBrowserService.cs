using Chameleon.Common.Helpers;
using Chameleon.Interfaces.App.Automation.Entities;
using Chameleon.Interfaces.App.Automation.ExternalScript;
using Chameleon.Interfaces.App.Automation.Manager;
using Chameleon.Interfaces.App.Automation.Playwright;
using Chameleon.Interfaces.App.Automation.Services;
using Chameleon.Interfaces.UserProfiles;
using Chameleon.Interfaces.WebBrowser;

namespace Chameleon.Playwright.Automation.Services;
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
        SystemBrowserType browserType,
        IList<IUserProfile> userProfiles)
    {
        try
        {
            string scripBody = _automationService.GetScriptBody(script.Id);
            IExternalScript instance = _compileScriptService.CompileScript(scripBody);
            var browser = _playwrightBrowserManager.Get(browserType);

            IDictionary<string, string> parameters = script.Parameters
                .Select(x => KeyValuePair.Create(x.Name, x.Value))
                .ToDictionary();

            using (var playwright = await Microsoft.Playwright.Playwright.CreateAsync())
            {
                foreach (IUserProfile profile in userProfiles)
                {
                    var options = new PlaywrightBrowserLaunchOptions
                    {
                        UserProfile = profile,
                        Playwright = playwright
                    };

                    var browserInstance = await browser.Open(options);

                    try
                    {
                        await instance.Run(browserInstance.BrowserContext, parameters);
                    }
                    catch (Exception ex)
                    {
                        await MesageBoxHelper.ShowErrorAsync("Script error", ex.Message);
                    }
                }
            }
        }

        catch (Exception ex) 
        {
            await MesageBoxHelper.ShowErrorAsync("Automation error", ex.Message);
        }
    }
}
