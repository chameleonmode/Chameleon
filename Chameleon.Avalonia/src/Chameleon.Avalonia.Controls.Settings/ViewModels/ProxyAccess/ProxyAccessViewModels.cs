using Chameleon.Interfaces.Dialogs;
using Chameleon.Interfaces.Ioc;

namespace Chameleon.Avalonia.Controls.Settings.ViewModels.ProxyAccess;



public class ProxyAccessViewModels
    : List<ProxyAccessViewModel>
    , IProxyAccessViewModels
{
    private readonly IToastNotificationService _toastNotificationService;
    public ProxyAccessViewModels(IToastNotificationService toastNotificationService)
    {
        _toastNotificationService = toastNotificationService;
    }

    public void AddItems(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Add(new ProxyAccessViewModel(_toastNotificationService));
        }
    }
}
