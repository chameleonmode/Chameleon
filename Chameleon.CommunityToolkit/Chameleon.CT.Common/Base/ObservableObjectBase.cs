using Chameleon.Interfaces.Dialogs;
using Chameleon.Prism.Events;

namespace Chameleon.CT.Common.Base;

public abstract partial class ObservableObjectBase : ObservableObject
{              
    private readonly IDispatcherService _dispatcherService;
    private readonly IEventAggregator eventAggregator;
    private readonly IContentDialogService _cntentDialogService; 

    [ObservableProperty]
    public string title = "ObservableObjectBase";

    public ObservableObjectBase()
    {
        _dispatcherService = ContainerServiceHelper.Current.ContainerProvider.Resolve<IDispatcherService>();
        _cntentDialogService = ContainerServiceHelper.Current.ContainerProvider.Resolve<IContentDialogService>();
        eventAggregator = ContainerServiceHelper.Current.ContainerProvider.Resolve<IEventAggregator>();
    }

    public IDispatcherService DispatcherService => _dispatcherService;
    public IContentDialogService ContentDialogService => _cntentDialogService;
    public IEventAggregator EventAggregator => eventAggregator;
}
