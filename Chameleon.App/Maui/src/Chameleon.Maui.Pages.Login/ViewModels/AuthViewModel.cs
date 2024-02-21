using Chameleon.Core.Extensions;
using Chameleon.Interfaces.Auth.Events;
using Chameleon.Interfaces.Auth;
using Chameleon.Interfaces.Settings;
using CommunityToolkit.Maui.Markup;
using System.Net;
using System.Security.Authentication;
using Chameleon.Maui.Toolkit.Base;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using Chameleon.Interfaces.Services;
using Chameleon.Interfaces.MessageBox;
using Chameleon.Interfaces.Alerts;
using Prism.Events;

namespace Chameleon.Maui.Pages.Login.ViewModels;

public partial class AuthViewModel : BaseViewModel, IAuthViewModel
{
    private readonly IAuthService _authService;
    private readonly IApplicationSettings _appSettings;
    private readonly IApplicationSettingsService _settingsService;
    private readonly IEventAggregator _eventAggregator;
    private readonly IAlertService _alertsService;

    public AuthViewModel(
        IAuthService authService,
        IApplicationSettingsService settingsService,
        IEventAggregator eventAggregator,
        INavigationService navigationService,
        IAlertService alertsService
        ) : base(navigationService)
    {
        _authService = authService;
        _settingsService = settingsService;
        _alertsService = alertsService;

        _appSettings = _settingsService.Get();
        UserName = _appSettings.Login.LoginName;
        LicenceKey = _appSettings.Login.LicenseKey;

        _eventAggregator = eventAggregator;
        _eventAggregator
            .GetEvent<SubmitAsyncEvent>()
            .Subscribe(async () => { await Auth(); });

    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AuthCommand))]
    private string licenceKey;


    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AuthCommand))]
    private string userName;


    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AuthCommand))]
    private bool isSubmiting;

    public bool IsInputEnabled => !IsSubmiting;

    [ObservableProperty]
    private string? errorMessage;


    [RelayCommand(CanExecute = nameof(CanSubmit))]
    private async Task Auth()
    {
        // confirmation dialog when switching licenses
        if (!LicenceKey.StartsWith("KEY")
            && _authService?.IsLicenseActive(LicenceKey) == false)
        {
            if (!await _alertsService.ShowConfirmationAsync(
                "Warning",
                "Do you want to activate another license? Current one will not be active anymore",
                "Yes",
                "No"))
                return;
        }
        await Task.Run(Submit);
    }
    private bool CanSubmit()
    {
        return !string.IsNullOrEmpty(LicenceKey)
            && !string.IsNullOrEmpty(UserName)
            && !IsSubmiting;
    }

    [RelayCommand]
    private void Cancel()
    {
        _eventAggregator
                  .GetEvent<LoginCancelEvent>()
                  .Publish();
    }
    private async void Submit()
    {
        string errorMessage = string.Empty;

        try
        {
            IsSubmiting = true;

            // try login with entered user name and password
            //AuthResult = _authService.Login(UserName, LicenceKey);

            // store auth info to reuse next startup
            _appSettings.Login.Set(UserName, LicenceKey);
            await _settingsService.Save();

            await _authService.Login();
            // close dialog
            //CloseDialog(ButtonResult.OK);

            // hiding spinner in case of successfull reconnection
            //if (_mainWindow != null)
            //    Application.Current?.Dispatcher.Invoke((d) => _mainWindow.HideWaitIndicator());

            return;
        }
        catch (AuthenticationException ex)
        {
            errorMessage = $"Login failed: Invalid email or licence key + {ex.Message}";
        }
        catch (WebException ex)
        {
            //ExceptionHandler.ShowException(ex);
            errorMessage = $"Error with login + {ex.Message}";
        }
        catch (Exception ex)
        {
            errorMessage = $"Error with login + {ex.Message}";
        }
        finally
        {
            IsSubmiting = false;
        }

        ErrorMessage = errorMessage;
    }
}
