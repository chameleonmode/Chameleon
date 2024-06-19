using Chameleon.Interfaces.App.Automation.Playwright;
using Chameleon.Interfaces.Environments;
using Chameleon.Interfaces.Settings;
using Chameleon.Interfaces.WebBrowser;
using Chameleon.Prism.Events;
using Chameleon.SystemBrowser.Browsers.Brave;
using Microsoft.Playwright;

namespace Chameleon.Playwright.Automation.Brave;
public class BravePlaywrightBrowserInstance
    : BraveSystemBrowserInstance
    , IPlaywrightBrowserInstance
{
    private readonly IPlaywrightBrowserLaunchOptions _playwrightOptions;
    private readonly IAutomationScriptHelper _automationScriptHelper;

    private IBrowserContext _browserContext;
    public IBrowserContext BrowserContext => _browserContext;

    public BravePlaywrightBrowserInstance(
        IEventAggregator eventAggregator,
        IPlaywrightBrowserLaunchOptions options,
        ISetPreferencesService setPreferencesService,
        IApplicationEnvironment applicationEnvironment,
        IUserDefaultSettingsService userDefaultsSettingsService,
        string browserExeFilePath,
        IAutomationScriptHelper automationScriptHelper
        )
        : base(eventAggregator,
            options,
            setPreferencesService,
            applicationEnvironment,
            userDefaultsSettingsService,
            browserExeFilePath)
    {
        _playwrightOptions = options;
        _automationScriptHelper = automationScriptHelper;
    }
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

        var options = _automationScriptHelper
            .CreateOptions(args, exts, _browserExeFilePath);

        _browserContext = await _playwrightOptions.Playwright.Chromium
            .LaunchPersistentContextAsync(_browserProfileFolderPath, options);

        await _automationScriptHelper.InitScriptAsync(_browserContext);
    }
}
