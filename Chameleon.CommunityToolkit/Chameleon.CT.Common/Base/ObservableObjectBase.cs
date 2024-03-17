namespace Chameleon.CT.Common.Base;

public partial class ObservableObjectBase : ObservableObject
{              
    private readonly IDispatcherService _dispatcherService;

    [ObservableProperty]
    public string title = "ObservableObjectBase";

    public ObservableObjectBase()
    {
        _dispatcherService = ContainerServiceHelper.Current.ContainerProvider.Resolve<IDispatcherService>();
    }

    public IDispatcherService DispatcherService => _dispatcherService;
}
