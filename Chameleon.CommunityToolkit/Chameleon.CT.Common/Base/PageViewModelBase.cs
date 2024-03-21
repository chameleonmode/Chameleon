namespace Chameleon.CT.Common.Base;

public class PageViewModelBase : ObservableObjectBase
{
    private readonly INavigationService _navigationService;
    public PageViewModelBase()
    {
        _navigationService = ContainerServiceHelper.Resolve<INavigationService>();
    }

    public INavigationService NavigationService => _navigationService;
}
