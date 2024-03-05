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

    public Task InvokeOnUiThreadAsync(Action action, Action<bool> handler = null, Action @finally = null)
    {
        return Task.Run(() =>
        {
            try
            {
                var success = TryExecute(action);

                if (handler != null)
                {
                    InvokeOnUiThread(() =>
                    {
                        handler(success);
                    });
                }
            }
            finally
            {
                @finally?.Invoke();
            }
        });
    }

    private bool TryExecute<T>(Func<T> action, out T result)
    {
        try
        {
            result = action();
            return true;
        }
        catch (Exception ex)
        {
            //TODO: ExceptionHandler.ShowException(ex);
        }
        result = default(T);
        return false;
    }

    private bool TryExecute(Action action)
    {
        return TryExecute(() =>
        {
            action();
            return true;
        }, out var result);
    }
}
