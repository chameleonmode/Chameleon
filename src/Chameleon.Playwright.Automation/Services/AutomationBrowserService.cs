namespace Chameleon.Playwright.Automation.Services;
public class AutomationBrowserService(
    IPlaywrightBrowserManager playwrightBrowserManager,
    ICompileScriptService compileScriptService,
    IAutomationService automationService,
    IEventAggregator eventAggregator,
    IRunningAutomationBrowsers runningAutomationBrowsers,
    IToastNotificationService toastNotificationService)
    : IAutomationBrowserService
{
    public async Task RunScript(
        IAutomationScriptDescription script,
        SystemBrowserType browserType,
        IList<IUserProfileActionsViewModel> userProfiles,
        CancellationToken token)
    {
        try
        {
            string scripBody = await automationService.GetScriptBody(script.Id);
            IExternalScript instance = await compileScriptService.CompileScript(scripBody);
            var browser = playwrightBrowserManager.Get(browserType);

            IDictionary<string, string> parameters = script.Parameters
                .Select(x => KeyValuePair.Create(x.Name, x.Value))
                .ToDictionary();

            runningAutomationBrowsers.RefreshBrowsers();

            using var playwright = await Microsoft.Playwright.Playwright.CreateAsync();
            foreach (IUserProfileActionsViewModel profile in userProfiles)
            {
                if (profile.SBI == null)
                {
                    await profile.OpenSystemBrowser(browserType);
                    if(!await profile.SBI.OPtcs.Task)
                        throw new Exception($"Failed to open system browser {profile.Title}");
                }

                var options = new PlaywrightBrowserLaunchOptions
                {
                    UserProfile = profile.UserProfile,
                    UserProfileVM = profile,
                    Playwright = playwright
                };

                var browserInstance = await browser.Open(options);

                runningAutomationBrowsers.AddBrowser(browserInstance);

                var ctx = browserInstance.BrowserContext;

                ctx.Close += (_, __) =>
                {
                    runningAutomationBrowsers.RemoveBrowser(browserInstance);

                    if (runningAutomationBrowsers.IsAllClosed)
                    {
                        RiseFinishScriptExecutionEvent();
                    }
                };

                try
                {
                    await instance.Run(browserInstance.BrowserContext, parameters).WaitAsync(token);
                }
                catch (Exception ex)
                {
                    // await MesageBoxHelper.ShowErrorAsync("Script error", ex.Message);
                    toastNotificationService.ShowError(ex.Message);
                }

                // Stop loop if canceled
                if (token.IsCancellationRequested)
                {
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            await MesageBoxHelper.ShowErrorAsync("Automation error", ex.Message);
            //toastNotificationService.ShowError(ex.Message);
        }
        finally
        {
            RiseFinishScriptExecutionEvent();
        }
    }

    private void RiseFinishScriptExecutionEvent()
    {
        eventAggregator.GetEvent<FinishScriptExecutionEvent>().Publish();
    }
}
