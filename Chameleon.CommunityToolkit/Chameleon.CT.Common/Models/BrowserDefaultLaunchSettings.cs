using Chameleon.Interfaces.Environments;
using Chameleon.Interfaces.Settings;
using System.Runtime;

namespace Chameleon.CT.Common.Models;
public partial class BrowserDefaultLaunchSettings : ObservableObject, IBrowserDefaultLaunchSettings
{
    public Config Config { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public object[] Excluded { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public Headers Headers { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public object[] IpRules { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public BrowserProfile Profile { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public IOptions Options { get; set; } = new Options();
    public Whitelist Whitelist { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    //make singleton
    private static BrowserDefaultLaunchSettings instance;
    private BrowserDefaultLaunchSettings() { }
    public static BrowserDefaultLaunchSettings Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new BrowserDefaultLaunchSettings();
                // Call the async initialization method
                instance.InitializeAsync().Wait();
            }
            return instance;
        }
    }


    // Async initialization method
    private async Task InitializeAsync()
    {
        // Load settings from a file or a remote source
       var settingsFilePath = Path.Combine(
           ContainerServiceHelper.Resolve<IApplicationEnvironment>().ApplicationDataFolderPath,
           "defaultBrowserLaunchSettings.json");
        if (!File.Exists(settingsFilePath))
            return;
        // var settings = await LoadSettingsFromFileAsync("settings.json");
        var json = await Task.Run(() => File.ReadAllText(settingsFilePath));
        var settings = System.Text.Json.JsonSerializer.Deserialize<BrowserDefaultLaunchSettings>(json);
        // Apply settings to properties
        // this.Config = settings.Config;
        // this.Excluded = settings.Excluded;
        // this.Headers = settings.Headers;
        // this.IpRules = settings.IpRules;
        // this.Profile = settings.Profile;
        this.Options = settings.Options;
        // this.Whitelist = settings.Whitelist;
    }

    public async Task Save()
    {
        // Save settings to a file or a remote source
        var settingsFilePath = Path.Combine(
            ContainerServiceHelper.Resolve<IApplicationEnvironment>().ApplicationDataFolderPath,
            "defaultBrowserLaunchSettings.json");
        var json = System.Text.Json.JsonSerializer.Serialize(this);
        await Task.Run(() => File.WriteAllText(settingsFilePath, json));
    }
}

public partial class Options : ObservableObject, IOptions
{
    [ObservableProperty]
    private bool cookieNotPersistent;
    [ObservableProperty]
    private string cookiePolicy;
    [ObservableProperty]
    private bool blockMediaDevices;
    [ObservableProperty]
    private bool blockCSSExfil;
    [ObservableProperty]
    private bool disableWebRTC;
    [ObservableProperty]
    private bool firstPartyIsolate;
    [ObservableProperty]
    private bool limitHistory;
    [ObservableProperty]
    private IProtectkbfingerprint protectKBFingerprint = new Protectkbfingerprint();
    [ObservableProperty]
    private bool protectWinName;
    [ObservableProperty]
    private bool resistFingerprinting;
    [ObservableProperty]
    private string screenSize;
    [ObservableProperty]
    private bool spoofAudioContext = true;
    [ObservableProperty]
    private bool spoofClientRects = true;
    [ObservableProperty]
    private bool spoofFontFingerprint = true;
    [ObservableProperty]
    private bool spoofMediaDevices = true;
    [ObservableProperty]
    private string timeZone;
    [ObservableProperty]
    private bool autoTimezone = true;
    [ObservableProperty]
    private string trackingProtectionMode;
    [ObservableProperty]
    private string webRTCPolicy;
    [ObservableProperty]
    private string webSockets;

    [ObservableProperty]
    private bool spoofCanvasFingerprint = true;
    [ObservableProperty]
    private bool spoofWebGLFingerprint = true;
    [ObservableProperty]
    private bool spoofWebGPUFingerprint = true;
    [ObservableProperty]
    private bool disableWebRtc = true;
}

public partial class Protectkbfingerprint : ObservableObject, IProtectkbfingerprint
{
    [ObservableProperty]
    private bool enabled;
    [ObservableProperty]
    int delay;
}
