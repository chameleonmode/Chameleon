using Avalonia.Collections;

namespace Chameleon.app.Avalonia.app;
public static class AvaloniaListExtensions {
	public static async Task AddNewRangeAsync<T>(this AvaloniaList<T> self, Func<Task<IEnumerable<T>>> values)
	{
		self.Clear();
		var vals = await values();
		if (vals != null)
			self?.AddRange(vals);
	}
}