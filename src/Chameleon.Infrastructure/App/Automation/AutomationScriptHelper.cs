using Chameleon.Core.Extensions;
using Chameleon.Interfaces.App.Automation.Playwright;
using Chameleon.Interfaces.UserProfiles;
using Microsoft.Playwright;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Chameleon.Infrastructure.App.Automation;
public class AutomationScriptHelper
    : IAutomationScriptHelper
{
    public BrowserTypeLaunchPersistentContextOptions CreateOptions(List<string> args, string exts, string browserExeFilePath, IProxySettings? proxy)
    {
        if (!string.IsNullOrEmpty(exts))
        {
            args.Add($"--disable-extensions-except={exts}");
            args.Add($"--load-extension={exts}");
        }

        var options = new BrowserTypeLaunchPersistentContextOptions
        {
            Args = args,
            ExecutablePath = browserExeFilePath,
            Headless = false,
            IgnoreDefaultArgs = new[] { "--enable-automation", "--no-sandbox", "--disable-extensions", "--disable-default-apps", "--disable-component-extensions-with-background-pages" }
        };

        if(proxy?.CanUse == true && proxy.Host.HasAny())
        {
            options.Proxy = new Proxy()
            {
                Server = $"http://{proxy.Host}:{proxy.Port}",
                Username = proxy.UserName,
                Password = proxy.Password,
            };
        }

        return options;
    }

    public Task InitScriptAsync(IBrowserContext browserContext)
        => browserContext.AddInitScriptAsync(
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
