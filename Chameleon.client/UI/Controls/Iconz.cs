using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Platform;
using Avalonia.Styling;

using System.Globalization;

using Chameleon.lib.Util;
using Avalonia.Data;
using Avalonia.Controls.Primitives;
using FluentAvalonia.UI.Controls;
using Chameleon.lib.Browzio;

namespace Chameleon.client.UI.Controls;

public class SvgIcon : TemplatedControl {
	public static readonly StyledProperty<string?> IconNameProperty =
	AvaloniaProperty.Register<SvgIcon, string?>(nameof(IconName));

	public string? IconName {
		get => GetValue(IconNameProperty);
		set => SetValue(IconNameProperty, value);
	}
}

public class SvgNameToDataConverter : IValueConverter {
	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
		if (value == null || value.ToString().Is()) value = "x";

		var uri = $"avares://Chameleon.client/Assets/svgs/{value}.svg";
		using var stream = AssetLoader.Open(new Uri(uri));
		using var reader = new StreamReader(stream);
		var data = reader.ReadToEnd();
		if(uri.Contains("browsers", StringComparison.OrdinalIgnoreCase)) return data;
		else if (Application.Current?.TryGetResource("AccentFillColorDefaultBrush", Application.Current.ActualThemeVariant, out var accentbrush) == true) {
			var acc = accentbrush?.ToString()?.Replace("#ff", "#");
			data = data.Replace("#5D25A6", acc);
			data = data.Replace("#8094AE", acc);
			data = data.Replace("#B5B5B5", acc);
			if (Application.Current.ActualThemeVariant == ThemeVariant.Dark)
				data = data.Replace("fill=\"black\"", "fill=\"white\"");
		}
		return data;
	}

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
		return BindingNotification.UnsetValue;
	}
}


// 2. BrowserInfoToIconConverter - Enhanced converter for BrowserInfo objects
public class BrowserInfoToIconConverter : IValueConverter {
	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
		if (value is not BrowserInfo info)
			return BindingNotification.UnsetValue;

		// You can embed default icons as resources and load them here
		var icon = info.Type switch {
			BrowserType.Chrome => "chrome",
			BrowserType.Firefox => "firefox",
			BrowserType.Edge => "edge",
			BrowserType.Safari => "safari",
			BrowserType.Brave => "brave",
			BrowserType.Opera => "opera",
			BrowserType.Vivaldi => "vivaldi",
			BrowserType.Chromium => "chromium",
			BrowserType.Waterfox => "waterfox",
			BrowserType.LibreWolf => "librewolf",
			BrowserType.Yandex => "yandex",
			BrowserType.Arc => "arc",
			BrowserType.InternetExplorer => "ie",
			_ => "browser"
		};
		return $"browsers/{icon}";
	}

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
		return BindingNotification.UnsetValue;
	}
}

public class IconSourceToString : IValueConverter {
	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
		return value == null || value is not SymbolIconSource iconSource ? "x" : iconSource.Symbol.ToString();
	}

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
		return BindingNotification.UnsetValue;
	}
}
