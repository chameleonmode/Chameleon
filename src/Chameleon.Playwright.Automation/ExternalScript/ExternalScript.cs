using Microsoft.Playwright;
using System.Collections.Generic;  // required for "IDictionary<string, string>"
using System;
using System.Threading.Tasks;
using Chameleon.Interfaces.App.Automation.ExternalScript; // required for "async Task"

public class ExternalScript : IExternalScript
{
    public async Task Run(IBrowserContext context, IDictionary<string, string> args)
    {
        // use to run script in a new tab 
        // IPage page = await context.NewPageAsync(); 
        // use to run script in the first tab 
        IPage page = context.Pages[0];

        // __________paste the recorded content under here____________________

        // __________paste the recorded content above here____________________

        // use this anywhere in the script to pause the script 
        await page.PauseAsync(); 
        // use this anywhere in the script to add delay
        // await Task.Delay(1000); 
    }
}