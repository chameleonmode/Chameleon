using Chameleon.Interfaces.Dialogs;

namespace Chameleon.CT.Common.Base;

public abstract partial class ObservableObjectBase : ObservableObject
{              
    private readonly IDispatcherService _dispatcherService;
    private readonly IContentDialogService _cntentDialogService;

    [ObservableProperty]
    public string title = "ObservableObjectBase";

    public ObservableObjectBase()
    {
        _dispatcherService = ContainerServiceHelper.Current.ContainerProvider.Resolve<IDispatcherService>();
        _cntentDialogService = ContainerServiceHelper.Current.ContainerProvider.Resolve<IContentDialogService>();
    }

    public IDispatcherService DispatcherService => _dispatcherService;
    public IContentDialogService ContentDialogService => _cntentDialogService;
}
