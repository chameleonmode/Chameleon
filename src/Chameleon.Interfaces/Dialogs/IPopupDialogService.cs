using Chameleon.Interfaces.Ioc;
using System.ComponentModel;

namespace Chameleon.Interfaces.Dialogs;

public interface IPopupDialogService : ISingletonDependency
{
    Task<IPopupDialogResult?> Create<T>() where T : INotifyPropertyChanged;
    //Task<IDialog> Create<T>(Action<IDialogResult>? callback = null, IDialogParameters? parameters = null) where T : INotifyPropertyChanged;

    // Closes the currently displayed popup. Optionally returning a result.
    void Close(object? result = null);

    // Closes the currently displayed popup.  Optionally returning a result.
    Task CloseAsync(object? result = null, CancellationToken cancellationToken = default);
}
