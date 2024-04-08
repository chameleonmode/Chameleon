using Microsoft.Playwright;

namespace Chameleon.SystemBrowser.Automation;

public class Play
{

    public static Play Instance { get; } = new Play();
    Dictionary<string, Tuple<IPlaywright, IBrowserContext>> createdContext = [];
    public bool IsBusy => Interlocked.Read(ref _isBusy) > 0;
    private Play()
    {
       // Init();
    }

    private void Init()
    {
       
    }

    static readonly IEnumerable<string> ignoreDefaultArgs = new[] { "--enable-automation", "--no-sandbox", "--disable-extensions", "--disable-default-apps", "--disable-component-extensions-with-background-pages" };

    public async Task LaunchPersistentContextAsync(string userDataDirDefault, string exepath, string url, string args, string exts, string server, string username, string password)
    {
        while (IsBusy)
            await Task.Delay(500);

        if (createdContext.ContainsKey(userDataDirDefault))
            return;

        Interlocked.Increment(ref _isBusy);

        try
        {

            var playwright = await Playwright.CreateAsync();
            var chromium = playwright.Chromium;


            BrowserTypeLaunchPersistentContextOptions options = new()
            {
                ExecutablePath = exepath,
                Headless = false,
                ViewportSize = ViewportSize.NoViewport,
                //IgnoreAllDefaultArgs = true,
                IgnoreDefaultArgs = ignoreDefaultArgs,
                Args = new[] { args, $"--load-extension={exts}" }
            };
            if (server != null)
            {
                options.Proxy = new Microsoft.Playwright.Proxy()
                {
                    Server = server
                };
                if (username != null)
                    options.Proxy.Username = username;

                if (password != null)
                    options.Proxy.Password = password;
            }

            var browser = await chromium.LaunchPersistentContextAsync(userDataDirDefault, options);

            if (url != null)
            {
                var page = await browser.NewPageAsync();
                await page.GotoAsync(url);
            }

            createdContext.Add(userDataDirDefault, new Tuple<IPlaywright, IBrowserContext>(playwright, browser));
            browser.Close += (s, e) =>
            {
                createdContext.Remove(userDataDirDefault);
            };
        }
        finally
        {
            Interlocked.Decrement(ref _isBusy);
        }
    }

    private long _isBusy;
}
