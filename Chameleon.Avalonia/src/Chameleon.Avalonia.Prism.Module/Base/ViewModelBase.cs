using Chameleon.Interfaces.Services;
using Chameleon.Interfaces.Views;

using Prism.Regions;
using Prism.Ioc;
using Prism.Commands;

namespace Chameleon.Avalonia.Prism.Module.Base;

public class ViewModelBase : BindableBase, INavigationAware ,IViewModel
{
                    
    private string _title= "ViewModelBase";
    private readonly IDispatcherService _dispatcherService;
    private readonly IRegionManager _regionManager;

    public ViewModelBase()
    {
        _dispatcherService = ContainerLocator.Current.Resolve<IDispatcherService>(); //TODO: ??? ContainerProviderServiceLocator.Current.ContainerProvider.Resolve<IDispatcherService>();
        _regionManager = ContainerLocator.Current.Resolve<IRegionManager>();
    }

    public IDispatcherService DispatcherService => _dispatcherService;
    public IRegionManager RegionManager => _regionManager;


    public virtual string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    /// <summary>
    ///   Handles Prism's request to navigate to.
    ///   Don't call this directly, use OnNavigatingTo
    ///   to comply with Prism v8.x
    /// </summary>
    /// <param name="navigationContext">Navigation Context.</param>
    /// <returns>Return True to allow navigation, False to deny it.</returns>
    public bool IsNavigationTarget(NavigationContext navigationContext)
    {
        // Auto-allow navigation
        return OnNavigatingTo(navigationContext);
    }

    /// <summary>Perform any (event) cleanup, we're navigating away.</summary>
    /// <param name="navigationContext">Navigation parameters.</param>
    public virtual void OnNavigatedFrom(NavigationContext navigationContext)
    {
    }

    /// <summary>Navigated to view.</summary>
    /// <param name="navigationContext">Navigation parameters.</param>
    public virtual void OnNavigatedTo(NavigationContext navigationContext)
    {
    }

    public virtual bool OnNavigatingTo(NavigationContext navigationContext)
    {
        return true;
    }

    #region Commands
    private readonly List<DelegateCommandBase> _commands
        = new List<DelegateCommandBase>();

    protected void AddCommand(DelegateCommandBase command)
    {
        _commands.Add(command);
    }

    protected DelegateCommand CreateCommand(
        Action executeMethod,
        Func<bool> canExecuteMethod = null)
    {
        var command = canExecuteMethod != null
            ? new DelegateCommand(executeMethod, canExecuteMethod)
            : new DelegateCommand(executeMethod);
        AddCommand(command);
        return command;
    }

    protected DelegateCommand<T> CreateCommand<T>(
        Action<T> executeMethod,
        Func<T, bool> canExecuteMethod = null)
    {
        var command = canExecuteMethod != null
            ? new DelegateCommand<T>(executeMethod, canExecuteMethod)
            : new DelegateCommand<T>(executeMethod);
        AddCommand(command);
        return command;
    }

    protected virtual void RaiseCanExecuteChanged()
    {
        foreach (var command in _commands)
        {
            command.RaiseCanExecuteChanged();
        }
    }
    #endregion
}

