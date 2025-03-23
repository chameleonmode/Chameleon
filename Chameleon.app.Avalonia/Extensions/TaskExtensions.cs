namespace Chameleon.app.Avalonia.Extensions;
public static class TaskExtensions {
	public static Task RunInBackground<T>(this Task<T> task, CancellationToken cancellationToken = default) {
		return Task.Run(() => task,cancellationToken);
	}

	public static Task RunInBackground(this Task task, CancellationToken cancellationToken = default) {
		return Task.Run(() => task, cancellationToken);
	}

	public static async Task<T?> RunInBackgroundWithResult<T>(this Task<T> task, CancellationToken cancellationToken = default) {
		var result = default(T);
		await Task.Run(async () => {
			result = await task;
		}, cancellationToken);
		return result;
	}
}
