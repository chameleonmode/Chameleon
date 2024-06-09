using Chameleon.Avalonia.Playwright.Automation.ExternalScript;
using Chameleon.Interfaces.WebBrowser;
using Microsoft.Playwright;
using System.Collections.Generic;  // required for "IDictionary<string, string>"
using System;
using System.Threading.Tasks; // required for "async Task"

public class ExternalScript : IExternalScript
{
    public SystemBrowserType BrowserType => SystemBrowserType.Brave;

    public async Task Run(IBrowserContext context, IDictionary<string, string> args)
    {
        IPage page = context.Pages[0];

        await page.GotoAsync(args["param 1-1"]);
        // other actions 
        await page.PauseAsync();
    }
}