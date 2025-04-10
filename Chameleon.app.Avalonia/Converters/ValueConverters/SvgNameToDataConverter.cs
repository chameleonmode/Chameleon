using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Platform;
using Avalonia.Styling;

using System.Globalization;

using Chameleon.lib.Util;
using BN = Avalonia.Data.BindingNotification;

namespace Chameleon.app.Avalonia.Converters.ValueConverters;

public class SvgNameToDataConverter : IValueConverter {
	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		if (value == null || value.ToString().Is()) value = "x";

		var uri = $"avares://Chameleon.app.Avalonia/Assets/Svgs/{value}.svg";
		using var stream = AssetLoader.Open(new Uri(uri));
		using var reader = new StreamReader(stream);
		var data = reader.ReadToEnd();

		if (Application.Current?.TryGetResource("AccentFillColorDefaultBrush", Application.Current.ActualThemeVariant, out var accentbrush) == true) {
			var acc = accentbrush?.ToString()?.Replace("#ff", "#");
			data = data.Replace("#5D25A6", acc);
			data = data.Replace("#8094AE", acc);
			data = data.Replace("#B5B5B5", acc);
			if (Application.Current.ActualThemeVariant == ThemeVariant.Dark)
				data = data.Replace("fill=\"black\"", "fill=\"white\"");
		}
		return data;
	}

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		return BN.UnsetValue;
	}
}
