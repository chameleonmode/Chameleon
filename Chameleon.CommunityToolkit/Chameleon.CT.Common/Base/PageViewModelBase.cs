using Chameleon.Interfaces;
using Chameleon.Prism.Events;

namespace Chameleon.CT.Common.Base;

public class PageViewModelBase : ObservableObjectBase
{
    private readonly IMainViewViewModel? mvvm;
    private readonly INavigationService? _navigationService;
    public PageViewModelBase()
    {                                                            
        mvvm = ContainerServiceHelper.Resolve<IMainViewViewModel>();
        _navigationService = ContainerServiceHelper.Resolve<INavigationService>();
    }

    public PageViewModelBase(string title) : this()
    {
        Title = title;
    }

    public PageViewModelBase(string title, Action init) : this(title)
    {
        init();
    }

    public IMainViewViewModel? MVVM => mvvm;
    public INavigationService? NavigationService => _navigationService;

}
