namespace Chameleon.app.Avalonia.Models;
public record AppSettings(string? CurrentAppTheme, string? CustomAccentColor, bool UseCustomAccentColor);
public record LoginSettings(string LoginName, string LicenseKey, bool AutoLogin);
public record CreditPlan(decimal Amount, string Size, bool IsChecked = false);