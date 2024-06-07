using Chameleon.Common.WinApiBridge;
using Chameleon.Interfaces.App.Automation.Playwright;
using Chameleon.Interfaces.Environments;
using Chameleon.Interfaces.Settings;
using Chameleon.Interfaces.WebBrowser;
using Chameleon.Prism.Events;
using Chameleon.SystemBrowser.Chrome;
using Microsoft.Playwright;
using System.Diagnostics;

namespace Chameleon.Avalonia.Playwright.Automation.Chrome;
public class ChromePlaywrightBrowserInstance
    : ChromeSystemBrowserInstance
    , IPlaywrightBrowserInstance
{
    protected IPlaywright _playwright;
    private readonly IPlaywrightBrowserLaunchOptions _playwritingOptions;
    public ChromePlaywrightBrowserInstance(
        IEventAggregator eventAggregator,
        IPlaywrightBrowserLaunchOptions options, 
        ISetPreferencesService setPreferencesService, 
        IApplicationEnvironment applicationEnvironment, 
        IUserDefaultSettingsService userDefaultsSettingsService, 
        string browserExeFilePath) 
        : base(eventAggregator, 
            options, 
            setPreferencesService, 
            applicationEnvironment, 
            userDefaultsSettingsService, 
            browserExeFilePath)
    {
        _playwritingOptions = options;
    }

    public override async Task Open()
    {
        _playwright = await Playwright.CreateAsync();

        await EnsureProfileFolderCreated();
        await InitializeProfileFolder();
        await InitializeExtensionPath();
        await StartProcess();
    }

    protected override async Task StartProcess()
    {
        List<string> args = GetClearCommandLineArgumentsList();
        string exts = GetLoadExtensionsArgument();

        if (!string.IsNullOrEmpty(exts))
        {
            args.Add($"--disable-extensions-except={exts}");
            args.Add($"--load-extension={exts}");
        }

        var browser = await _playwright.Chromium.LaunchPersistentContextAsync(
            _browserProfileFolderPath,
            new BrowserTypeLaunchPersistentContextOptions
            {
                Args = args,
                ExecutablePath = _browserExeFilePath,
                Headless = false,
            });
        
        var page = await browser.NewPageAsync();

        await page.WaitForLoadStateAsync();
        await page.GotoAsync("https://demo.playwright.dev/todomvc/");
        await page.WaitForLoadStateAsync();
        
    }
}
