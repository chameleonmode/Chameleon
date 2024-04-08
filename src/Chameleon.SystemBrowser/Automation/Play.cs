using Microsoft.Playwright;

namespace Chameleon.SystemBrowser.Automation;

public class Play
{
    public static Play Instance { get; } = new Play();
    List<IPlaywright> playwrights = [];
    List<IBrowserContext> browserContexts = [];
    //IBrowserType browserType;
    private Play()
    {
       // Init();
    }

    private void Init()
    {
       
    }

    static readonly IEnumerable<string> ignoreDefaultArgs = new[] { "--enable-automation", "--no-sandbox", "--disable-extensions", "--disable-default-apps", "--disable-component-extensions-with-background-pages" };

    public async void SystemBrowserPresistLaunchWithCmdArgs(string userDataDirDefault, string exepath,string url, string args,string exts, string server, string username, string password)
    {
        var playwright = await Playwright.CreateAsync();
        playwrights.Add(playwright);

        var chromium = playwright.Chromium;
        var browser = await chromium.LaunchPersistentContextAsync(
            userDataDirDefault,
            new()
            {
                ExecutablePath = exepath,
                Headless = false,
                ViewportSize = ViewportSize.NoViewport,
                //IgnoreAllDefaultArgs = true,
                IgnoreDefaultArgs = ignoreDefaultArgs,
                Args = new[] { args, $"--load-extension={exts}" },
                Proxy = new Microsoft.Playwright.Proxy()
                {
                    Server = server,
                    Username = username,
                    Password = password
                }

            });
        browserContexts.Add(browser);

        //var page = await browser.NewPageAsync(); 
        //var client = await page.Context.NewCDPSessionAsync(page);
        //using var cplaywright = await Playwright.CreateAsync();
        //var cchromium = playwright.Chromium;
        //var cbrowser = await cchromium.ConnectOverCDPAsync("http://localhost:1000", new() { });
        //var cpage = await cbrowser.Contexts[0].NewPageAsync();

        //if (url != null)
        //    await page.GotoAsync(url);

        // other actions 
       // await page.PauseAsync();
    }
}
