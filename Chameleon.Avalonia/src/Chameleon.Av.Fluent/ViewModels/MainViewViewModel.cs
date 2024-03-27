using Avalonia.Controls;
using Chameleon.Auth.Services;
using Chameleon.CT.Common.Base;
using Chameleon.Interfaces.Auth;
using Chameleon.Prism.Events;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Diagnostics;

namespace Chameleon.Av.Fluent.ViewModels;

public partial class MainViewViewModel:ObservableObjectBase
{
    [ObservableProperty]
    private bool isSplashVisible = true;

    private readonly IAuthService _authService;

    public MainViewViewModel(IAuthService authService)
    {
        _authService = authService;

        EventAggregator
            .GetEvent<LoginFailEvent>()
            .SubscribeOnce(LoginFailEventMethod);

        EventAggregator
            .GetEvent<LoginSuccessEvent>()
            .SubscribeOnce(LoginSuccessEventMethod);
    }

    private void LoginSuccessEventMethod()
    {
        IsSplashVisible = false;
    }

    private async void LoginFailEventMethod()
    {
        if (!Design.IsDesignMode)
        {
            IsSplashVisible = true;
            await _authService.ShowLoginDialogAsync();
        }
    }
}
