using Avalonia.Threading;
using Chameleon.lib.Services;

namespace Chameleon.client.Services;

public class DispatchService : IDispatchService {
	public void InvokeOnUiThread(Action callback) {
		Dispatcher.UIThread.Post(callback);
	}

	public T InvokeOnUiThread<T>(Func<T> action) {
		return Dispatcher.UIThread.Invoke(action);
	}
}