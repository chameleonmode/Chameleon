using Chameleon.Interfaces.Dialogs;
using Prism.Services.Dialogs;

using IDialogService = Prism.Services.Dialogs.IDialogService;
using System.ComponentModel;
using Avalonia.Controls;
using Avalonia;
using Chameleon.Core.Extensions;
using Avalonia.Controls.ApplicationLifetimes;
using Prism.Ioc;
using Chameleon.Avalonia.Prism.Interfaces.Dialogs;
using System.Xml.Linq;
using Tmds.DBus.Protocol;

namespace Chameleon.Avalonia.Prism.Infrastructure.Services;

public class PopupDialogManagerService : IPopupDialogWinowService
{
    private readonly IContainerExtension _containerExtension;
    private readonly IDialogService _dialogService;
    public PopupDialogManagerService(IDialogService dialogService, 
        IContainerExtension containerExtension)
    {
        _dialogService = dialogService;
        _containerExtension = containerExtension;
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

    public void ShowDialog(string wname, string message, Action<int?> result)
    {
        // PRO TIP: Use `nameof(DialogView)` instead of "DialogView" to catch errors early on
        ShowDialog(wname, new DialogParameters($"message={message}"), (r) => { result((int?)r?.Result); });
    }

    public void ShowDialog(string name, IDialogParameters parameters, Action<IDialogResult?> callback)
    {
        _dialogService.ShowDialog(
        name,
         parameters,
             r =>
             {
                 if (r is null)
                 {
                     callback(null);
                 }
                 else
                 {

                     callback(r);
                 }
             });
    }
    public void ShowDialog(Window owner, string name, IDialogParameters parameters, Action<IDialogResult> callback)
    {
        _dialogService.ShowDialog(
            owner,
         name,
          parameters,
              r =>
              {
                  if (r is null)
                  {
                      callback(null);
                  }
                  else
                  {

                      callback(r);
                  }
              });
    }

}
