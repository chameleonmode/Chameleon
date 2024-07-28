using Chameleon.Interfaces.Settings;

namespace Chameleon.Infrastructure.Settings;

public class SettingsSettings  : ISettingsSettings
{
    public string CurrentAppTheme { get; set; } = "System";
    public string CustomAccentColor { get; set; }
    public bool UseCustomAccentColor { get; set; }
    public bool AutoLogin { get; set; } = true;
    public string CodesverifyApiKey { get; set; }
}