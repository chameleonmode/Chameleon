using Chameleon.Interfaces.App.Automation.Playwright;
using Chameleon.Interfaces.Environments;
using Chameleon.Interfaces.Settings;
using Chameleon.Interfaces.WebBrowser;
using Chameleon.Prism.Events;
using Chameleon.SystemBrowser.Browsers.Brave;
using Microsoft.Playwright;

namespace Chameleon.Avalonia.Playwright.Automation.Brave;
public class BravePlaywrightBrowserInstance
    : BraveSystemBrowserInstance
    , IPlaywrightBrowserInstanceWithContext
{
    protected IPlaywright _playwright;
    private readonly IPlaywrightBrowserLaunchOptions _playwritingOptions;

    private IBrowserContext _browserContext;
    public IBrowserContext BrowserContext => _browserContext;

    public BravePlaywrightBrowserInstance(
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
        _playwright = await Microsoft.Playwright.Playwright.CreateAsync();

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

        _browserContext = await _playwright.Chromium.LaunchPersistentContextAsync(
            _browserProfileFolderPath,
            new BrowserTypeLaunchPersistentContextOptions
            {
                Args = args,
                ExecutablePath = _browserExeFilePath,
                Headless = false,
            });
    }
}
