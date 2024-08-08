using Chameleon.Interfaces.Dialogs;
using Chameleon.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chameleon.Common.Helpers;
public class ToasterHelper
{
    readonly IToastNotificationService ToastNotificationService;
    readonly IDispatcherService DispatcherService;

    private ToasterHelper()
    {
        ToastNotificationService = ContainerServiceHelper.Resolve<IToastNotificationService>();
        DispatcherService = ContainerServiceHelper.Resolve<IDispatcherService>();
    }
    public static ToasterHelper Current { get; } = new ToasterHelper();

    public static void ShowErr(string err)
        => Current.ToastNotificationService.ShowError(err);

    public static void ShowSuccess(string err)
    => Current.ToastNotificationService.ShowSuccess(err);
}
