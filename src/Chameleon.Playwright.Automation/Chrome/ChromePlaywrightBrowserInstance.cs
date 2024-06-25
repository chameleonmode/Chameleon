using Chameleon.Interfaces.App.Automation.Playwright;
using Chameleon.Interfaces.Environments;
using Chameleon.Interfaces.Settings;
using Chameleon.Interfaces.WebBrowser;
using Chameleon.Prism.Events;
using Chameleon.SystemBrowser.Chrome;
using Microsoft.Playwright;

namespace Chameleon.Playwright.Automation.Chrome;
public class ChromePlaywrightBrowserInstance(
    IEventAggregator eventAggregator,
    IPlaywrightBrowserLaunchOptions options,
    ISetPreferencesService setPreferencesService,
    IApplicationEnvironment applicationEnvironment,
    IUserDefaultSettingsService userDefaultsSettingsService,
    string browserExeFilePath) : 
    ChromeSystemBrowserInstance(eventAggregator,
            options,
            setPreferencesService,
            applicationEnvironment,
            userDefaultsSettingsService,
            browserExeFilePath)
    , IPlaywrightBrowserInstance
{

    private IBrowserContext _browserContext;
    public IBrowserContext BrowserContext => _browserContext;
   
    public override async Task Open()
    {
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

        _browserContext = await options.Playwright.Chromium.LaunchPersistentContextAsync(
            BrowserProfileFolderPath,
            new BrowserTypeLaunchPersistentContextOptions
            {
                Args = args,
                ExecutablePath = browserExeFilePath,
                Headless = false,
            });
    }
}
