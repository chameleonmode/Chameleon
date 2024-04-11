using Chameleon.Interfaces;

namespace Chameleon.CT.Common.Base;

public class PageViewModelBase : ObservableObjectBase
{
    private readonly IMainViewViewModel mvvm;
    private readonly INavigationService _navigationService;
    public PageViewModelBase()
    {                                                            
        mvvm = ContainerServiceHelper.Resolve<IMainViewViewModel>();
        _navigationService = ContainerServiceHelper.Resolve<INavigationService>();
    }
                                      
    public IMainViewViewModel MVVM => mvvm;
    public INavigationService NavigationService => _navigationService;

}
