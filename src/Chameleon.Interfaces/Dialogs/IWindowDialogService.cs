using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.Dialogs;

public interface IWindowDialogService :
    Chameleon.lib.Common.Interfaces.Systemics.ISingletonDependency
{
    void ShowTopmost<TView, TViewModel>(Action<TViewModel> initialize, Action<TViewModel>? OnClosed = null, string title = "Copy Pasta", int width = 256) where TView : new() where TViewModel : new();
    void ShowTopmost<TView, TViewModel>(TViewModel vm, Action<TViewModel> initialize, Action<TViewModel>? OnClosed = null, string title = "Copy Pasta", int width = 256) where TViewModel : class;
}
