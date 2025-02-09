namespace Chameleon.app.Avalonia.Extensions;
public static class TagsExtensions {
	public static async Task<string> ToStringAsync(this Task<IEnumerable<string>> fetchTags) {
		var tags = await fetchTags;
		return string.Join(",", tags);
	}

	public static IEnumerable<string> ToTagsList(this string? tags) {
		return string.IsNullOrEmpty(tags) ? [] : tags.Split(",").Select(x => x.Trim());
	}
}
