using Chameleon.Interfaces.Dialogs;
using Prism.Services.Dialogs;

using IDialogService = Prism.Services.Dialogs.IDialogService;
using System.ComponentModel;
using Avalonia.Controls;
using Avalonia;
using Chameleon.Core.Extensions;
using Avalonia.Controls.ApplicationLifetimes;

namespace Chameleon.Avalonia.Prism.Infrastructure.Services;

public class PopupDialogManagerService : IPopupDialogService
{
    private readonly IDialogService _dialogService;
    public PopupDialogManagerService(IDialogService dialogService)
    {
        _dialogService = dialogService;
    }

    public Task<IPopupDialogResult?> Create<T>() where T : INotifyPropertyChanged
    {
        throw new NotImplementedException();
    }

    public void Close(object? result = null)
    {
        throw new NotImplementedException();
    }

    public Task CloseAsync(object? result = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public void ShowDialog(string wname, string message, Action<int> result)
    {
        // PRO TIP: Use `nameof(DialogView)` instead of "DialogView" to catch errors early on
        _dialogService.ShowDialog(
            wname,
            new DialogParameters($"message={message}"),
            r =>
            {
                if (r is null)
                {
                }
                else
                {

                    result((int)r.Result);
                }
            });
    }
}
