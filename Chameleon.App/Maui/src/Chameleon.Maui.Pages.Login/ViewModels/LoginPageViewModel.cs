using Chameleon.Interfaces.Auth;
using Chameleon.Interfaces.Dialogs;
using Chameleon.Interfaces.Services;
using Chameleon.Interfaces.Startup;
using Chameleon.Maui.Toolkit.Base;
using CommunityToolkit.Mvvm.Input;
using Prism.Events;

namespace Chameleon.Maui.Pages.Login.ViewModels;

public partial class LoginPageViewModel : BaseViewModel
{
    private readonly IPopupDialogService _popupDialogService;
    private readonly IEventAggregator _eventAggregator;
    public LoginPageViewModel(INavigationService navigationService,
        IPopupDialogService popupDialogService,
        IEventAggregator eventAggregator) 
        : base(navigationService)
    {
        _popupDialogService = popupDialogService;
        _eventAggregator = eventAggregator;
    }

    [RelayCommand]
    private async Task LoginPopup()
    {
        var result = await _popupDialogService.Create<AuthViewModel>();
        if (result != null)
        {
            if (result.ButtonResult != ButtonResult.OK)
            {
                // reject auth
                //OnAuthenticateCancel();
                //return Task.CompletedTask;
            }
            else
            {
                // get view model from result
                var viewModel = result.ResultObject as AuthViewModel;
                // call success
                //OnAuthenticateSuccess(viewModel);
            }
        }
    }
}