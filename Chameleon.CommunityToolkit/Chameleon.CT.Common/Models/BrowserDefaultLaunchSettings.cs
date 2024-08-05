using Chameleon.Interfaces.Environments;
using Chameleon.Interfaces.Settings;
using Chameleon.Common.Json;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Chameleon.CT.Common.Models;
public partial class BrowserDefaultLaunchSettings : ObservableObject, IBrowserDefaultLaunchSettings
{
    public const string Filename = "defaultBrowserSettings.json";
    [JsonIgnore]
    private static readonly JsonSerializerOptions options = new JsonSerializerOptions
    {
        Converters = 
        { 
            new DynamicJsonConverter<Options, IOptions>(),
            new DynamicJsonConverter<Protectkbfingerprint, IProtectkbfingerprint>()
        }
    };
    public Config Config { get; set; }
    public object[] Excluded { get; set; }
    public Headers Headers { get; set; }
    public object[] IpRules { get; set; }
    public BrowserProfile Profile { get; set; }
    public IOptions Options { get; set; } = new Options();
    public Whitelist Whitelist { get; set; }

    //make singleton
    private static BrowserDefaultLaunchSettings instance;
    //private BrowserDefaultLaunchSettings() { }
    public static async Task<BrowserDefaultLaunchSettings> Instance()
    {
        
            if (instance == null)
            {
                instance = new BrowserDefaultLaunchSettings();
                // Call the async initialization method
                await instance.InitializeAsync();
            }
            return instance;
    }


    // Async initialization method
    private async Task InitializeAsync()
    {
        // Load settings from a file or a remote source
        // var settings = await LoadSettingsFromFileAsync("settings.json");
        var json = await ConfigHelper.ReadFromAppDir(Filename);
        if (json == null)
            return;

        var settings = JsonSerializer.Deserialize<BrowserDefaultLaunchSettings>(json, options);
        // Apply settings to properties
        // this.Config = settings.Config;
        // this.Excluded = settings.Excluded;
        // this.Headers = settings.Headers;
        // this.IpRules = settings.IpRules;
        // this.Profile = settings.Profile;
        this.Options = settings.Options;
        // this.Whitelist = settings.Whitelist;
    }

    public static async Task Save()
    {
        // Save settings to a file or a remote source
        await ConfigHelper.WriteToAppDir(Filename, JsonSerializer.Serialize(await Instance(), options));
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
    private bool disableWebRTC = true;
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
}

public partial class Protectkbfingerprint : ObservableObject, IProtectkbfingerprint
{
    [ObservableProperty]
    private bool enabled;
    [ObservableProperty]
    int delay = 1;
}
