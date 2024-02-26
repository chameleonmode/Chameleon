using Chameleon.Interfaces.Dialogs;
using Chameleon.Maui.Toolkit.Helpers;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;

namespace Chameleon.Maui.Toolkit.Services;

public class PopupDialogManagerService : IPopupDialogService
{
    readonly IPopupService popupService;

    //theory: Might need to be Stack<Popup> or something else in order to track multiple popups being displayed.
    Popup? currentPopup; // Assigned in each of the ShowPopup calls

    public PopupDialogManagerService(IPopupService popupService)
    {
        this.popupService = popupService;
    }

    public async Task<IPopupDialogResult?> Create<T>() where T : INotifyPropertyChanged
    {
       return await popupService.ShowPopupAsync<T>() as IPopupDialogResult;
    }

    //theory: Closes the currently displayed popup. Optionally returning a result.
    //public async Task<IDialog> Create<T>(Action<IDialogResult>? callback = null, IDialogParameters? parameters = null) where T : INotifyPropertyChanged
    //{
    //    var pupupResult = await popupService.ShowPopupAsync<T>();

    //    return pupupResult as IDialog;
    //}

    //theory: Closes the currently displayed popup. Optionally returning a result.
    public void Close(object? result = null)
    {
        if (currentPopup is null)
        {
            return; // Should we throw?
        }

        currentPopup.Close(result);
    }

    //theory: Closes the currently displayed popup.  Optionally returning a result.
    public Task CloseAsync(object? result = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(currentPopup, nameof(currentPopup));
        //if (currentPopup is null)
        //{
        //    //return; // Should we throw?
        //}

        return currentPopup.CloseAsync(result, cancellationToken);
    }

    public void ShowDialog(string name, string message, Action<int?> result)
    {
        throw new NotImplementedException();
    }
}
