using Chameleon.Avalonia.Prism.Module.Base;
using Chameleon.Interfaces.Dialogs;
using Prism.Commands;

namespace Chameleon.Avalonia.Controls.Settings.ViewModels.ProxyAccess;

public class ProxyAccessViewModel
    : ViewModelBase
{
    private readonly IToastNotificationService _toastNotificationService;
    public ProxyAccessViewModel(
        IToastNotificationService toastNotificationService
        )
    {
        CopyUrlCommand = new DelegateCommand(CopyUrl);
        _toastNotificationService = toastNotificationService;
    }
    private string _url;
    public string Url
    {
        get => _url;
        set => SetProperty(ref _url, value);
    }

    public DelegateCommand CopyUrlCommand { get; set; }

    private void CopyUrl()
    {
        if (_url == null)
        {
            return;
        }

        //TODO: Clipboard.SetText(_url);
        _toastNotificationService.ShowSuccess("Copied to clipboard");
    }
}