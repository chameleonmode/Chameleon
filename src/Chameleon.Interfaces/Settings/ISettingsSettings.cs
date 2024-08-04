namespace Chameleon.Interfaces.Settings;

public interface ISettingsSettings
{
    string? CurrentAppTheme { get; set; }
    string? CustomAccentColor { get; set; }
    bool UseCustomAccentColor { get; set; }
    bool AutoLogin { get; set; }
    string CodesverifyApiKey { get; set; }
    string UserScriptsDirectory { get; set; }
}

public interface IBrowserDefaultLaunchSettings
{
    Config Config { get; set; }
    object[] Excluded { get; set; }
    Headers Headers { get; set; }
    object[] IpRules { get; set; }
    BrowserProfile Profile { get; set; }
    IOptions Options { get; set; }
    Whitelist Whitelist { get; set; }
}
public class Config
{
    public bool Enabled { get; set; }
    public bool NotificationsEnabled { get; set; }
    public string Theme { get; set; }
    public int ReloadIPStartupDelay { get; set; }
}

public class Headers
{
    public bool BlockEtag { get; set; }
    public bool EnableDNT { get; set; }
    public Referer Referer { get; set; }
    public Spoofacceptlang SpoofAcceptLang { get; set; }
    public Spoofip SpoofIP { get; set; }
}

public class Referer
{
    public bool Disabled { get; set; }
    public int Xorigin { get; set; }
    public int Trimming { get; set; }
}

public class Spoofacceptlang
{
    public bool Enabled { get; set; }
    public string Value { get; set; }
}

public class Spoofip
{
    public bool Enabled { get; set; }
    public int Option { get; set; }
    public string RangeFrom { get; set; }
    public string RangeTo { get; set; }
}

public class BrowserProfile
{
    public string Selected { get; set; }
    public Interval Interval { get; set; }
    public bool ShowProfileOnIcon { get; set; }
}

public class Interval
{
    public int Option { get; set; }
    public int Min { get; set; }
    public int Max { get; set; }
}

public interface IOptions
{
    bool CookieNotPersistent { get; set; }
    string CookiePolicy { get; set; }
    bool BlockMediaDevices { get; set; }
    bool DisableWebRtc { get; set; }
    bool BlockCSSExfil { get; set; }
    bool DisableWebRTC { get; set; }
    bool FirstPartyIsolate { get; set; }
    bool LimitHistory { get; set; }
    IProtectkbfingerprint ProtectKBFingerprint { get; set; }
    bool ProtectWinName { get; set; }
    bool ResistFingerprinting { get; set; }
    string ScreenSize { get; set; }
    bool SpoofAudioContext { get; set; }
    bool SpoofClientRects { get; set; }
    bool SpoofFontFingerprint { get; set; }
    bool SpoofMediaDevices { get; set; }
    bool SpoofCanvasFingerprint { get; set; }
    bool SpoofWebGLFingerprint { get; set; }
    bool SpoofWebGPUFingerprint { get; set; }
    string TimeZone { get; set; }
    bool AutoTimezone { get; set; }
    string TrackingProtectionMode { get; set; }
    string WebRTCPolicy { get; set; }
    string WebSockets { get; set; }
}

public interface IProtectkbfingerprint
{
    bool Enabled { get; set; }
    int Delay { get; set; }
}

public class Whitelist
{
    public bool enabledContextMenu { get; set; }
    public string defaultProfile { get; set; }
    public object[] rules { get; set; }
}
