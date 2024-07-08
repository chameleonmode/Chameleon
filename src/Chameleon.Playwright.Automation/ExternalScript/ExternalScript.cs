using Microsoft.Playwright;
using System.Collections.Generic;  // required for "IDictionary<string, string>"
using System;
using System.Threading.Tasks;
using Chameleon.Interfaces.App.Automation.ExternalScript; // required for "async Task"

public class ExternalScript : IExternalScript
{
    public async Task Run(IBrowserContext context, IDictionary<string, string> args)
    {
        IPage page = await context.NewPageAsync(); // or context.Pages[0];

        // once satisfied with recording copy the recorded content for pages create a new file and paste under here you can remove the pause 
        await page.PauseAsync();
    }
}