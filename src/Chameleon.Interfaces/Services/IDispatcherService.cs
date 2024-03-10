using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.Services;

public interface IDispatcherService : ISingletonDependency
{
    void InvokeOnUiThread(Action callback);
    T InvokeOnUiThread<T>(Func<T> action);
    void InvokeOnUiThread(object self, EventHandler handler, EventArgs args = null);
    Task InvokeOnUiThreadAsync(Action action, Action<bool> handler = null, Action @finally = null);
    Task InvokeOnUiThreadAsync<T>(Func<T> action, Action<T> handler = null, Action @finally = null);

}
