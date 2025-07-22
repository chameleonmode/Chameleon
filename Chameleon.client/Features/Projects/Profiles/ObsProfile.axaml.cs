using System.Globalization;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Input;
using Avalonia.VisualTree;
using Chameleon.lib.Util;
using Chameleon.lib.Browzio;
using Avalonia.Media;
using Avalonia.Platform;
using System.Diagnostics;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Chameleon.client.Features.Projects.Profiles;

public partial class UserProfileUserControl : UserControl {
	public UserProfileUserControl() {
		InitializeComponent();

		DoubleTapped += OnDoubleTapped;
	}

	private void OnDoubleTapped(object? sender, TappedEventArgs e) {
		if (e.Source is Visual v && v.FindAncestorOfType<Button>(true) is null && DataContext is ObsProfile up)
			up.Navigate();
	}
}

// 1. ExecutablePathToIconConverter - Basic converter for executable paths
// public class ExecutablePathToIconConverter : IValueConverter {
// 	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
// 		if (value is not string exePath || string.IsNullOrWhiteSpace(exePath))
// 			return BindingNotification.UnsetValue;

// 		if (!File.Exists(exePath))
// 			return BindingNotification.UnsetValue;

// 		try {
// 			// Extract icon data using your existing IconExtractor
// 			var iconData = IconExtractor.ExtractIcon(exePath);
// 			if (iconData == null || iconData.Length == 0)
// 				return BindingNotification.UnsetValue;

// 			// Convert byte array to Avalonia Bitmap
// 			using var stream = new MemoryStream(iconData);
// 			return new Bitmap(stream);
// 		} catch {
// 			return BindingNotification.UnsetValue;
// 		}
// 	}

// 	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
// 		return BindingNotification.UnsetValue;
// 	}
// }

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
		// // Load from embedded resources
		// // Make sure to add these icons to your Assets/Icons folder and set Build Action to "AvaloniaResource"
		// var uri = new Uri($"avares://Chameleon.client/Assets/pngs/{iconName}");
		// return new Bitmap(AssetLoader.Open(uri));
		// var uri = $"avares://Chameleon.client/Assets/Svgs/browsers/{icon}.svg";
		// using var stream = AssetLoader.Open(new Uri(uri));
		// using var reader = new StreamReader(stream);
		// var data = reader.ReadToEnd();
		// return data;
	}

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
		return BindingNotification.UnsetValue;
	}
}

// 3. CachedExecutablePathToIconConverter - Performance-optimized converter with caching
// public class CachedExecutablePathToIconConverter : IValueConverter {
// 	private static readonly Dictionary<string, Bitmap?> _iconCache = new();
// 	private static readonly object _cacheLock = new();
// 	private static readonly Dictionary<string, DateTime> _cacheTimestamps = new();
// 	private static readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(30); // Cache expires after 30 minutes

// 	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
// 		if (value is not string exePath || string.IsNullOrWhiteSpace(exePath))
// 			return BindingNotification.UnsetValue;

// 		if (!File.Exists(exePath))
// 			return BindingNotification.UnsetValue;

// 		lock (_cacheLock) {
// 			// Check if cached entry exists and is not expired
// 			if (_iconCache.TryGetValue(exePath, out var cachedIcon) &&
// 					_cacheTimestamps.TryGetValue(exePath, out var timestamp) &&
// 					DateTime.Now - timestamp < _cacheExpiration) {
// 				return cachedIcon != null ? cachedIcon : BindingNotification.UnsetValue;
// 			}

// 			try {
// 				// Extract icon data
// 				var iconData = IconExtractor.ExtractIcon(exePath);
// 				if (iconData == null || iconData.Length == 0) {
// 					_iconCache[exePath] = null;
// 					_cacheTimestamps[exePath] = DateTime.Now;
// 					return BindingNotification.UnsetValue;
// 				}

// 				// Convert to Avalonia Bitmap
// 				using var stream = new MemoryStream(iconData);
// 				var bitmap = new Bitmap(stream);

// 				// Cache the result with timestamp
// 				_iconCache[exePath] = bitmap;
// 				_cacheTimestamps[exePath] = DateTime.Now;

// 				return bitmap;
// 			} catch {
// 				_iconCache[exePath] = null;
// 				_cacheTimestamps[exePath] = DateTime.Now;
// 				return BindingNotification.UnsetValue;
// 			}
// 		}
// 	}

// 	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
// 		return BindingNotification.UnsetValue;
// 	}

// 	// Method to clear cache if needed
// 	public static void ClearCache() {
// 		lock (_cacheLock) {
// 			foreach (var bitmap in _iconCache.Values) {
// 				bitmap?.Dispose();
// 			}
// 			_iconCache.Clear();
// 			_cacheTimestamps.Clear();
// 		}
// 	}

// 	// Method to remove expired entries from cache
// 	public static void CleanupExpiredEntries() {
// 		lock (_cacheLock) {
// 			var expiredKeys = new List<string>();
// 			var now = DateTime.Now;

// 			foreach (var kvp in _cacheTimestamps) {
// 				if (now - kvp.Value > _cacheExpiration) {
// 					expiredKeys.Add(kvp.Key);
// 				}
// 			}

// 			foreach (var key in expiredKeys) {
// 				if (_iconCache.TryGetValue(key, out var bitmap)) {
// 					bitmap?.Dispose();
// 					_iconCache.Remove(key);
// 				}
// 				_cacheTimestamps.Remove(key);
// 			}
// 		}
// 	}

// 	// Property to get cache statistics
// 	public static (int CachedItems, int TotalRequests) GetCacheStats() {
// 		lock (_cacheLock) {
// 			return (_iconCache.Count, _iconCache.Count + _cacheTimestamps.Count);
// 		}
// 	}
// }

public class EngineToBrushConverter : IValueConverter {
	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
		if (value is not BrowserEngine engine)
			return BindingNotification.UnsetValue;

		return engine switch {
			BrowserEngine.Chromium => new SolidColorBrush(Color.FromRgb(66, 133, 244)), // Google Blue
			BrowserEngine.Gecko => new SolidColorBrush(Color.FromRgb(255, 95, 31)),     // Firefox Orange
			BrowserEngine.WebKit => new SolidColorBrush(Color.FromRgb(0, 122, 255)),    // Safari Blue
			BrowserEngine.Other => new SolidColorBrush(Color.FromRgb(128, 128, 128)),   // Gray
			_ => new SolidColorBrush(Color.FromRgb(169, 169, 169))                      // Light Gray
		};
	}

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
		return BindingNotification.UnsetValue;
	}
}

// Usage Instructions:
/*
1. Add these converter classes to your project in a "Converters" folder or namespace

2. In your App.axaml, reference them like this:
   xmlns:converters="using:YourApp.Converters"

3. In Application.Resources:
   <converters:BrowserInfoToIconConverter x:Key="BrowserToIconConverter"/>
   <converters:CachedExecutablePathToIconConverter x:Key="CachedExeToIconConverter"/>
   <converters:ExecutablePathToIconConverter x:Key="ExeToIconConverter"/>

4. Use in XAML:
   <!-- For BrowserInfo objects (recommended) -->
   <Image Source="{Binding BrowserInfo, Converter={StaticResource BrowserToIconConverter}}" Width="32" Height="32"/>
   
   <!-- For executable paths with caching (best performance) -->
   <Image Source="{Binding ExecutablePath, Converter={StaticResource CachedExeToIconConverter}}" Width="32" Height="32"/>
   
   <!-- For simple executable paths -->
   <Image Source="{Binding ExecutablePath, Converter={StaticResource ExeToIconConverter}}" Width="32" Height="32"/>

5. Optional: Add default browser icons to Assets/Icons/ folder in your project and set Build Action to "AvaloniaResource"

6. Optional: Periodically clean up the cache by calling CachedExecutablePathToIconConverter.CleanupExpiredEntries()
*/
