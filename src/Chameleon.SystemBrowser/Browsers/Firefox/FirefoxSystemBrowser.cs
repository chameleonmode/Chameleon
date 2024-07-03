namespace Chameleon.SystemBrowser.Firefox;
public class FirefoxSystemBrowser(
        IEventAggregator eventAggregator,
        IApplicationEnvironment applicationEnvironment,
         ISetPreferencesService setPreferencesService,
        ISystemBrowserInfoManager systemBrowserInfoManager,
        IUserDefaultSettingsService userDefaultsSettingsService)
    : SystemBrowserBase(eventAggregator, applicationEnvironment, setPreferencesService, userDefaultsSettingsService),
    IFirefoxSystemBrowser
{
    public const string FirefoxChameleonDirectory = "FirefoxChameleon";

    string DirectoryForCopy => OperatingSystem.IsMacOS() ?
    System.IO.Path.Combine(ApplicationEnvironment.ApplicationDataFolderPath, FirefoxChameleonDirectory, "firefox.app")
    : System.IO.Path.Combine(ApplicationEnvironment.ApplicationDataFolderPath, FirefoxChameleonDirectory);


    public override ISystemBrowserInstance InitializeBrowser(ISystemBrowserLaunchOptions o)
    {
        return new FirefoxSystemBrowserInstance(
            EventAggregator,
            o,
            UserDefaultSettingsService,
            ApplicationEnvironment.ApplicationDataFolderPath,
            GetBrowserExePath());
    }

    public override async Task<ISystemBrowserInstance> InitializeBrowserAsync(ISystemBrowserLaunchOptions o)
    {
        await CreateChameleonFirefoxCopy();

        return InitializeBrowser(o);
    }

    private async Task CreateChameleonFirefoxCopy()
    {
        if (IOtil.IsNeedUpdate(Path, ChamelonPath))
        {
            await IOtil.DeleteDExistsAsync(DirectoryForCopy);

            await IOtil.CopyFolderAsync(Directory, DirectoryForCopy);

            await Task.Delay(1000);
        }


        await AddonsUtil.AddAutoloadTemporaryAddonFF(System.IO.Path.Combine(DirectoryForCopy));
    }

    protected override string GetSystemBrowserExePath()
    {
        return systemBrowserInfoManager
            .FindByName("firefox")
            .Path;
    }
    protected override string GetBrowserExePath()
    {
        string path = OperatingSystem.IsMacOS()
            ? System.IO.Path.Combine(ApplicationEnvironment.ApplicationDataFolderPath, FirefoxChameleonDirectory, "firefox.app", "Contents", "MacOS", "firefox")
            : System.IO.Path.Combine(ApplicationEnvironment.ApplicationDataFolderPath, FirefoxChameleonDirectory, "firefox.exe");

        return path;
    }

    protected override string GetDirectoryPath() => OperatingSystem.IsMacOS() ?
    "Applications/firefox.app"
    : System.IO.Path.GetDirectoryName(Path);

    public override SystemBrowserType BrowserType => SystemBrowserType.Firefox;
}

