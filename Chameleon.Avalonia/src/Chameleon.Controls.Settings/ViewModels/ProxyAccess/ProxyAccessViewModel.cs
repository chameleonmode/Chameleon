using Chameleon.Avalonia.Common.Services;
using Chameleon.CT.Common.Base;
using Chameleon.Interfaces.Dialogs;
using Chameleon.Interfaces.Services;
using CommunityToolkit.Mvvm.Input;

namespace Chameleon.Avalonia.Controls.Settings.ViewModels.ProxyAccess;

public partial class ProxyAccessViewModel
    : SubPageViewModelBase
{
    private readonly IToastNotificationService _toastNotificationService;
    public ProxyAccessViewModel(
        IToastNotificationService toastNotificationService
        )
    {
        _toastNotificationService = toastNotificationService;
    }
    private string _url;
    public string Url
    {
        get => _url;
        set => SetProperty(ref _url, value);
    }

   [RelayCommand]
    private async Task CopyUrl()
    {
        if (_url == null)
        {
            return;
        }
        await IClipboardService.Instance.SetTextAsync( _url );
        _toastNotificationService.ShowSuccess("Copied to clipboard");
    }
}