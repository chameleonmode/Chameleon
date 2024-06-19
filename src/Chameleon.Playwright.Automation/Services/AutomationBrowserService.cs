using Chameleon.Common.Helpers;
using Chameleon.Interfaces.App.Automation.Entities;
using Chameleon.Interfaces.App.Automation.Events;
using Chameleon.Interfaces.App.Automation.ExternalScript;
using Chameleon.Interfaces.App.Automation.Manager;
using Chameleon.Interfaces.App.Automation.Playwright;
using Chameleon.Interfaces.App.Automation.Services;
using Chameleon.Interfaces.Services;
using Chameleon.Interfaces.UserProfiles;
using Chameleon.Interfaces.WebBrowser;
using Chameleon.Prism.Events;

namespace Chameleon.Playwright.Automation.Services;
public class AutomationBrowserService 
    : IAutomationBrowserService
{
    private readonly IPlaywrightBrowserManager _playwrightBrowserManager;
    private readonly ICompileScriptService _compileScriptService;
    private readonly IAutomationService _automationService;
    private readonly IEventAggregator _eventAggregator;
    private readonly IDispatcherService _dispatcherService;

    public AutomationBrowserService(
        IPlaywrightBrowserManager playwrightBrowserManager,
        ICompileScriptService compileScriptService,
        IAutomationService automationService,
        IEventAggregator eventAggregator,
        IDispatcherService dispatcherService
        )
    {
        _playwrightBrowserManager = playwrightBrowserManager;
        _compileScriptService = compileScriptService;
        _automationService = automationService;
        _eventAggregator = eventAggregator;
        _dispatcherService = dispatcherService;
    }

    public async Task RunScript(
        IAutomationScriptDescription script,
        SystemBrowserType browserType,
        IList<IUserProfile> userProfiles,
        CancellationToken token)
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
                        await _dispatcherService.InvokeOnUiThread(async () =>
                        {
                            await MesageBoxHelper.ShowErrorAsync("Script error", ex.Message);
                        });
                    }

                    // Stop loop if canceled
                    if (token.IsCancellationRequested)
                    {
                        break;
                    }
                }
            }
        }
        catch (Exception ex) 
        {
            await _dispatcherService.InvokeOnUiThread(async () =>
            {
                await MesageBoxHelper.ShowErrorAsync("Automation error", ex.Message);
            });
        }
        finally
        {
            _dispatcherService.InvokeOnUiThread(() => _eventAggregator
                    .GetEvent<FinishScriptExecutionEvent>()
                    .Publish());
        }
    }
}
