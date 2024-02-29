using Avalonia.Threading;
using Chameleon.Interfaces.Services;

namespace Chameleon.Avalonia.Common.Services;

public class DispatcherService : IDispatcherService
{
    public void InvokeOnUiThread(Action callback)
    {
        Dispatcher.UIThread.Post(callback);
    }

    public T InvokeOnUiThread<T>(Func<T> action)
    {
        return Dispatcher.UIThread.Invoke(action);
    }

    public void InvokeOnUiThread(object self, EventHandler handler, EventArgs args = null)
    {
        InvokeOnUiThread(() =>
        {
            handler?.Invoke(self, args ?? new EventArgs());
        });
    }
}
