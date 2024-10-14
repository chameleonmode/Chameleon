using Chameleon.Interfaces;
using Chameleon.lib.Common.Interfaces.Services;
using Chameleon.lib.Common.ServiceManagers;

namespace Chameleon.CT.Common.Base;

public class PageViewModelBase : ObservableObjectBase
{
    private readonly IMainViewViewModel? mvvm;
    public PageViewModelBase()
    {                                                            
        mvvm = ContainerServiceHelper.Resolve<IMainViewViewModel>();
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
    public INavigationService? NavigationService => Navigator.Instance.NavigationService;

}
