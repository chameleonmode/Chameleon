using static Chameleon.lib.Common.Constants.Enums;

namespace Chameleon.app.Avalonia.Models;
public record AppSettings(string? CurrentAppTheme, string? CustomAccentColor, bool UseCustomAccentColor);
public record CreditPlan(decimal Amount, string Size, bool IsChecked = false);
public record SystemBrovserItem(SystemBrowserType SystemBrowserType) {
	public string IconName => SystemBrowserType.ToString().ToLower();
}