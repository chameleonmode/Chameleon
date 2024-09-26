using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.Services;

public interface IDispatcherService : Chameleon.lib.Common.Interfaces.Systemics.ISingletonDependency
{
    void InvokeOnUiThread(Action callback);
    T InvokeOnUiThread<T>(Func<T> action);
    Task InvokeOnUiThreadAsync(Action action, Action<bool> handler = null, Action @finally = null);
    Task InvokeOnUiThreadAsync<T>(Func<T> action, Action<T> handler = null, Action @finally = null);

}
