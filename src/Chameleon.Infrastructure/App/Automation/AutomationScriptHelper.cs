using Chameleon.Interfaces.App.Automation.Playwright;
using Microsoft.Playwright;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Chameleon.Infrastructure.App.Automation;
public class AutomationScriptHelper
    : IAutomationScriptHelper
{
    private List<string> AddExtensionsArguments(List<string> args, string exts)
    {
        if (!string.IsNullOrEmpty(exts))
        {
            args.Add($"--disable-extensions-except={exts}");
            args.Add($"--load-extension={exts}");
        }

        return args;
    }

    private BrowserTypeLaunchPersistentContextOptions CreateOptions(List<string> args, string browserExeFilePath)
    {
        var options = new BrowserTypeLaunchPersistentContextOptions
        {
            Args = args,
            ExecutablePath = browserExeFilePath,
            Headless = false,
            IgnoreDefaultArgs = new[] { "--enable-automation" }
        };

        return options;
    } 
    
    public BrowserTypeLaunchPersistentContextOptions CreateOptions(List<string> args, string exts, string browserExeFilePath)
    {
        return CreateOptions(AddExtensionsArguments(args, exts), browserExeFilePath);
    }

    public async Task InitScriptAsync(IBrowserContext browserContext)
    {
        await browserContext.AddInitScriptAsync(
               @"const defaultGetter = Object.getOwnPropertyDescriptor(
              Navigator.prototype,
              ""webdriver""
            ).get;
            defaultGetter.apply(navigator);
            defaultGetter.toString();
            Object.defineProperty(Navigator.prototype, ""webdriver"", {
              set: undefined,
              enumerable: true,
              configurable: true,
              get: new Proxy(defaultGetter, {
                apply: (target, thisArg, args) => {
                  Reflect.apply(target, thisArg, args);
                  return false;
                },
              }),
            });
            const patchedGetter = Object.getOwnPropertyDescriptor(
              Navigator.prototype,
              ""webdriver""
            ).get;
            patchedGetter.apply(navigator);
            patchedGetter.toString();");
    }
}
