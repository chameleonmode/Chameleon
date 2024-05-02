using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.Dialogs;

public interface IWindowDialogService :
    ISingletonDependency
{
    Task ShowDialogAsync(Action<object, EventArgs>[] events);
    void ShowTopmost<TView, TViewModel>(Action<TViewModel> initialize, int width, Action<TViewModel>? OnClosed = null) where TViewModel : class;
}
