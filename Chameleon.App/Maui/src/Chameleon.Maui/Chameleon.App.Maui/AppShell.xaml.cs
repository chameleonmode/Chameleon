using Chameleon.Interfaces.Services;
using Chameleon.Interfaces.Startup;
using Chameleon.Maui.Pages.Settings.ViewModels;
using Chameleon.Maui.Pages.Settings.Views;
using Chameleon.Maui.Toolkit.Base;

namespace Chameleon.App.Maui;

public partial class AppShell : Shell
{
    private readonly INavigationService _navigationService;
    private readonly IApplicationStartup _applicationStartup;

    public AppShell(INavigationService navigationService, IApplicationStartup applicationStartup)
    {                   
        InitializeComponent();

        _navigationService = navigationService;
        _applicationStartup = applicationStartup;
    }

    protected override async void OnHandlerChanged()
    {
        base.OnHandlerChanged();

        if (Handler is not null)
        {                                     
            await _navigationService.InitializeAsync();
            //await _applicationStartup.Run();
        }
    }
}
