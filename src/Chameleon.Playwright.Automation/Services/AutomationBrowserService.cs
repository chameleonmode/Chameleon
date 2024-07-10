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
    public static readonly string recordcontent = @"
using Microsoft.Playwright;
using System.Collections.Generic;  // required for ""IDictionary<string, string>""
using System;
using System.Threading.Tasks;
using Chameleon.Interfaces.App.Automation.ExternalScript; // required for ""async Task""

public class ExternalScript : IExternalScript
{
    public async Task Run(IBrowserContext context, IDictionary<string, string> args)
    {
        IPage page = await context.NewPageAsync(); 

        // other actions 
        await page.PauseAsync();
    }
}
";
    public async Task RunScript(
        IAutomationScriptDescription script,
        SystemBrowserType browserType,
        IList<IUserProfileActionsViewModel> userProfiles,
        CancellationToken token,
        bool record)
    {
        try
        {
            string scripBody = record ? recordcontent : script.Id < 0 ?
                await automationService.GetScriptBody(script.FilePath) :
                await automationService.GetScriptBody(script.Id);                            
            
            IDictionary<string, string> parameters = record ? [] : script.Parameters
                .Select(x => KeyValuePair.Create(x.Name, x.Value))
                .ToDictionary();

            var instance = await compileScriptService.CompileScript(scripBody);
            var browser = playwrightBrowserManager.Get(browserType);

            runningAutomationBrowsers.RefreshBrowsers();

            using var playwright = await Microsoft.Playwright.Playwright.CreateAsync();
            foreach (IUserProfileActionsViewModel profile in userProfiles)
            {
                var browserWasNotOpened = profile.SBI == null;
                if (browserWasNotOpened)
                {
                    await profile.OpenSystemBrowser(browserType).WaitAsync(token);
                    if (!await profile.SBI.OPtcs.Task.WaitAsync(token))
                        continue;
                }

                var options = new PlaywrightBrowserLaunchOptions
                {
                    UserProfile = profile.UserProfile,
                    UserProfileVM = profile,
                    Playwright = playwright
                };

                var browserInstance = await browser.Open(options);

                runningAutomationBrowsers.AddBrowser(browserInstance);
                try
                {
                    if (record)
                        await new ExternalScript().Run(browserInstance.BrowserContext, parameters).WaitAsync(token); //await browserInstance.Record().WaitAsync(token);
                    else
                        await instance.Run(browserInstance.BrowserContext, parameters).WaitAsync(token);
                }
                catch (Exception ex)
                {
                    // await MesageBoxHelper.ShowErrorAsync("Script error", ex.Message);
                    toastNotificationService.ShowError(ex.Message);
                }
                // Check if the browser process is not null and hasn't exited
                if (browserWasNotOpened &&
                    profile.SBI != null &&
                    profile.SBI.Brocess != null &&
                    !profile.SBI.Brocess.HasExited)
                {
                    try
                    {
                        await browserInstance.Close();
                        // Attempt to close the browser gracefully
                        profile.SBI.Brocess.CloseMainWindow();

                        // Give the process some time to exit gracefully
                        bool exitedGracefully = profile.SBI.Brocess.WaitForExit(2500); // Wait for 2.5 seconds

                        if (!exitedGracefully)
                        {
                            // If the process hasn't exited within 5 seconds, kill it
                            profile.SBI.Brocess.Kill();
                            // Wait for the process to be killed
                            profile.SBI.Brocess.WaitForExit();
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log or handle the exception if closing the process fails
                        toastNotificationService.ShowError($"Failed to close the browser process: {ex.Message}");
                    }
                    finally
                    {
                        // Ensure the process is disposed
                        //profile.SBI.Brocess.Dispose();
                        //profile.SBI.Cleanup();
                        runningAutomationBrowsers.RemoveBrowser(browserInstance);
                    }
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
            // RiseFinishScriptExecutionEvent();
        }
    }

    private void RiseFinishScriptExecutionEvent()
    {
        eventAggregator.GetEvent<FinishScriptExecutionEvent>().Publish();
    }
}
