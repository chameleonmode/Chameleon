using Chameleon.Common.Icons;
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

    [ObservableProperty]
    public string glyph = FontIcons.FontIconsInfos[0].Glyph;

    public ObservableObjectBase()
    {
        _dispatcherService = ContainerServiceHelper.Resolve<IDispatcherService>();// ?? new DispatcherService();
        _cntentDialogService = ContainerServiceHelper.Resolve<IContentDialogService>();
        eventAggregator = ContainerServiceHelper.Resolve<IEventAggregator>() ?? new EventAggregator();
    }

    public IDispatcherService DispatcherService => _dispatcherService;
    public IContentDialogService ContentDialogService => _cntentDialogService;
    public IEventAggregator EventAggregator => eventAggregator;
}
