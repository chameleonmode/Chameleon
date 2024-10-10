namespace Chameleon.app.lib.Models.Settings;
public class AppSettings {
	public string CurrentAppTheme { get; set; } = "System";
	public string? CustomAccentColor { get; set; }
	public bool UseCustomAccentColor { get; set; }
	public bool AutoLogin { get; set; } = true;
}
