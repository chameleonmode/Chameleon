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

    public Task Close()
    {
        throw new NotImplementedException();
    }


    public override async Task Open()
    {
        await EnsureProfileFolderCreated();
        await InitializeProfileFolder();
        await InitializeExtensionPath();
        await StartProcess();
    }

    public Task Record()
    {
        throw new NotImplementedException();
    }


    protected override async Task StartProcess()
    {
        List<string> args = GetClearCommandLineArgumentsList();
        string exts = GetLoadExtensionsArgument();

        var contexOptions = automationScriptHelper
            .CreateOptions(args, exts, BrowserExeFilePath, UserProfile.Proxy);

        _browserContext = await options.Playwright.Chromium
            .LaunchPersistentContextAsync(BrowserProfileFolderPath, contexOptions);
    }
}
