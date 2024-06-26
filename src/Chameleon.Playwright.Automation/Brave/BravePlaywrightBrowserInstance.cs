using Chameleon.Interfaces.App.Automation.Playwright;
using Chameleon.Interfaces.Environments;
using Chameleon.Interfaces.Settings;
using Chameleon.Interfaces.WebBrowser;
using Chameleon.Prism.Events;
using Chameleon.SystemBrowser.Browsers.Brave;
using Microsoft.Playwright;

namespace Chameleon.Playwright.Automation.Brave;
public class BravePlaywrightBrowserInstance(IEventAggregator eventAggregator,
        IPlaywrightBrowserLaunchOptions options,
        ISetPreferencesService setPreferencesService,
        IApplicationEnvironment applicationEnvironment,
        IUserDefaultSettingsService userDefaultsSettingsService,
        string browserExeFilePath,
        IAutomationScriptHelper automationScriptHelper)
    : BraveSystemBrowserInstance(eventAggregator,
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

        var contexOptions = automationScriptHelper
            .CreateOptions(args, exts, browserExeFilePath, UserProfile.Proxy);

        _browserContext = await options.Playwright.Chromium
            .LaunchPersistentContextAsync(BrowserProfileFolderPath, contexOptions);
    }
}
