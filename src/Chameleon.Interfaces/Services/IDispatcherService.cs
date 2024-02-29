using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.Services;

public interface IDispatcherService : ISingletonDependency
{
    void InvokeOnUiThread(Action callback);
    T InvokeOnUiThread<T>(Func<T> action);
    void InvokeOnUiThread(object self, EventHandler handler, EventArgs args = null);
}
