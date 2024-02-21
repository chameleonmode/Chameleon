using Chameleon.Core.Extensions;
using Chameleon.Interfaces.Auth;
using Chameleon.Interfaces.Services;
using Chameleon.Interfaces.Settings;
using Chameleon.Maui.Toolkit.Base;
using Chameleon.Maui.Toolkit.Models;

namespace Chameleon.Maui.Pages.Login.ViewModels;

public class LoginGalleryViewModel : BaseGalleryViewModel
{
    private readonly IAuthService _authService;

    public LoginGalleryViewModel(INavigationService navigationService,
        IAuthService authService) : 
        base([
            SectionModel.Create<LoginPageViewModel>("Activate", "Login with email and product key"),
        ],
        navigationService)
    {               
        _authService = authService;
    }

    public override async Task InitializeAsync()
    {
        await _authService.Login();
    }
}
