using Chameleon.Interfaces.Services;
using CommunityToolkit.Mvvm.Input;

namespace Chameleon.Maui.Interfaces.Base;
public interface IBaseViewModel : IQueryAttributable
{
    public INavigationService NavigationService { get; }

    public IAsyncRelayCommand InitializeAsyncCommand { get; }

    public bool IsBusy { get; }

    public bool IsInitialized { get; }

    Task InitializeAsync();
}
