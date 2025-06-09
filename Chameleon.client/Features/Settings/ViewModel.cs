using Avalonia;
using Avalonia.Media;
using Avalonia.Styling;
using System.Reflection;

using Chameleon.lib;
using Chameleon.client.MvvM;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using FluentAvalonia.Styling;
using Chameleon.lib.Auth;

namespace Chameleon.client.Features.Settings;

public record AppSettings(string? CurrentAppTheme, string? CustomAccentColor, bool UseCustomAccentColor);

public partial class ViewModel : ViewModelObjectBase {
	private const string _system = "System";
	private const string _dark = "Dark";
	private const string _light = "Light";

	private readonly FluentAvaloniaTheme? _faTheme = Application.Current?.Styles[0] as FluentAvaloniaTheme;

	//TODO: refactor
	public string CurrentVersion { get; } = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "2024.x.x.x";
	public string[] AppThemes => [_system, _light, _dark];
	public List<Color> PredefinedColors => [
		Color.FromRgb(255,185,0),
		Color.FromRgb(255,140,0),
		Color.FromRgb(247,99,12),
		Color.FromRgb(202,80,16),
		Color.FromRgb(218,59,1),
		Color.FromRgb(239,105,80),
		Color.FromRgb(209,52,56),
		Color.FromRgb(255,67,67),
		Color.FromRgb(231,72,86),
		Color.FromRgb(232,17,35),
		Color.FromRgb(234,0,94),
		Color.FromRgb(195,0,82),
		Color.FromRgb(227,0,140),
		Color.FromRgb(191,0,119),
		Color.FromRgb(194,57,179),
		Color.FromRgb(154,0,137),
		Color.FromRgb(0,120,212),
		Color.FromRgb(0,99,177),
		Color.FromRgb(142,140,216),
		Color.FromRgb(107,105,214),
		Color.FromRgb(135,100,184),
		Color.FromRgb(116,77,169),
		Color.FromRgb(177,70,194),
		Color.FromRgb(136,23,152),
		Color.FromRgb(0,153,188),
		Color.FromRgb(45,125,154),
		Color.FromRgb(0,183,195),
		Color.FromRgb(3,131,135),
		Color.FromRgb(0,178,148),
		Color.FromRgb(1,133,116),
		Color.FromRgb(0,204,106),
		Color.FromRgb(16,137,62),
		Color.FromRgb(122,117,116),
		Color.FromRgb(93,90,88),
		Color.FromRgb(104,118,138),
		Color.FromRgb(81,92,107),
		Color.FromRgb(86,124,115),
		Color.FromRgb(72,104,96),
		Color.FromRgb(73,130,5),
		Color.FromRgb(16,124,16),
		Color.FromRgb(118,118,118),
		Color.FromRgb(76,74,72),
		Color.FromRgb(105,121,126),
		Color.FromRgb(74,84,89),
		Color.FromRgb(100,124,100),
		Color.FromRgb(82,94,84),
		Color.FromRgb(132,117,69),
		Color.FromRgb(126,115,95)
	];

	[ObservableProperty] bool hasProxySettingsView;
	[ObservableProperty] bool hasProxyCredit;
	[ObservableProperty] bool hasPhoneVerification;
	[ObservableProperty] bool hasAssistantUsers;
	[ObservableProperty] bool hasImport;
	[ObservableProperty] bool hasExport;
	[ObservableProperty] string currentAppTheme = _system;
	[ObservableProperty] bool useCustomAccentColor = false;
	[ObservableProperty] Color customAccentColor = Colors.SlateBlue;
	[ObservableProperty] Color? listBoxColor;
	[ObservableProperty] string liscencedTo = "xxx";

	public void InitializSettings() {
		if (IoC.GetJsonValue<AppSettings>(nameof(AppSettings)) is AppSettings appSettings) {
			if (appSettings.UseCustomAccentColor && appSettings.CustomAccentColor is string coler) {
				UpdateAppAccentColor(Color.Parse(coler));
			}
			CurrentAppTheme = appSettings.CurrentAppTheme ?? _system;
			UseCustomAccentColor = appSettings.UseCustomAccentColor;
		}

		if (IoC.GetJsonValue<LoginSettings>(nameof(LoginSettings)) is LoginSettings login)
			LiscencedTo = $"Licensed to: {login.LoginName}";
	}

	[RelayCommand]
	public async Task Logout() {
		await Session.Instance.Logout();
		Environment.Exit(0);
	}

	partial void OnUseCustomAccentColorChanged(bool oldValue, bool newValue) {
		if (newValue) {
			//SetCustomAccentColorFromSystem();
			if (_faTheme?.TryGetResource("SystemAccentColor", null, out var curColor) == true) {
				CustomAccentColor = (Color)curColor;
				ListBoxColor = CustomAccentColor;
			}
		} else {
			//ResetCustomAccentColor();
			CustomAccentColor = default;
			ListBoxColor = default;
			UpdateAppAccentColor(null);
		}
	}

	partial void OnCustomAccentColorChanged(Color oldValue, Color newValue) {
		UpdateAppAccentColor(newValue);
	}
	partial void OnListBoxColorChanged(Color? oldValue, Color? newValue) {
		UpdateAppAccentColor(newValue);
	}

	partial void OnCurrentAppThemeChanged(string? oldValue, string newValue) {
		//ApplyThemeVariant(newValue);
		static ThemeVariant? GetThemeVariant(string value) => value switch {
			_light => ThemeVariant.Light,
			_dark => ThemeVariant.Dark,
			_system or _ => null,
		};

		var newTheme = GetThemeVariant(newValue);
		if (newTheme != null && Application.Current != null) {
			Application.Current.RequestedThemeVariant = newTheme;
		}

		if (_faTheme != null) {
			_faTheme.PreferSystemTheme = newValue == _system;
		}
		SaveIfNeeded();
	}

	private void UpdateAppAccentColor(Color? color) {
		if (_faTheme != null && _faTheme.CustomAccentColor != color) {
			_faTheme.CustomAccentColor = color;
		}

		SaveIfNeeded();
	}

	private void SaveIfNeeded() {
		if (Loaded) {
			IoC.SetJsonValue(new AppSettings(CurrentAppTheme, _faTheme?.CustomAccentColor?.ToString(), UseCustomAccentColor), nameof(AppSettings));
		}
	}
}
