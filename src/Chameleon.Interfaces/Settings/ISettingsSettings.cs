namespace Chameleon.Interfaces.Settings;

public interface ISettingsSettings
{
    string? CurrentAppTheme { get; set; }
    string? CustomAccentColor { get; set; }
    bool UseCustomAccentColor { get; set; }
    bool AutoLogin { get; set; }
    string CodesverifyApiKey { get; set; }
}
