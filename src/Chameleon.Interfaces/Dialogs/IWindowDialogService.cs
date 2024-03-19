using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.Dialogs;

public interface IWindowDialogService :
    ISingletonDependency
{
    Task ShowDialogAsync(Action<object, EventArgs>[] events);
}
