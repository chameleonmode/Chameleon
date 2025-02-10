using System.Text.RegularExpressions;

namespace Chameleon.app.Avalonia.Extensions;
public static class ValidationExtensions {

	public static bool IsValidPhoneNumber(this string? value) {

		if (string.IsNullOrEmpty(value)) return false;

		var pattern = @"^\s*(?:\+?(\d{1,3}))?[-. (]*(\d{3})[-. )]*(\d{3})[-. ]*(\d{4})(?: *x(\d+))?\s*$";
		var regex = new Regex(pattern, RegexOptions.IgnoreCase);

		return regex.IsMatch(value);
	}

	public static bool IsValidWebUrl(this string? value) {

		if(string.IsNullOrEmpty(value)) return false;

		var pattern = @"^((https?|ftp|smtp|http):\/\/)?(www.)?[a-z0-9]+\.[a-z]+(\/[a-zA-Z0-9#]+\/?)*$";

		var regex = new Regex(pattern, RegexOptions.IgnoreCase);

		return regex.IsMatch(value);
	}
}