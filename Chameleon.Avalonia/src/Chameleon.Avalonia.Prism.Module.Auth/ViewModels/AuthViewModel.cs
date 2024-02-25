using Avalonia;
using Chameleon.Auth.Services;
using Chameleon.Avalonia.Prism.Module.Base;
using Chameleon.Core.Extensions;
using Chameleon.Interfaces.Auth;
using Chameleon.Interfaces.Auth.Events;
using Chameleon.Interfaces.MessageBox;
using Chameleon.Interfaces.Services;
using Chameleon.Interfaces.Settings;
using Chameleon.Prism.Events;
using Prism.Commands;
using Prism.Services.Dialogs;
using System.Net;
using System.Runtime;
using System.Security.Authentication;

namespace Chameleon.Avalonia.Prism.Module.Auth.ViewModels;

public class AuthViewModel : DialogViewModelBase, IAuthViewModel
{
    private readonly IAuthService _authService;
    private readonly IApplicationSettings _settings;
    private readonly IApplicationSettingsService _settingsService;
    private readonly IEventAggregator _eventAggregator;
   // private readonly IMessageBoxService _messageBoxService;
    public AuthViewModel(
    IAuthService authService,
        IApplicationSettingsService settingsService,
        IEventAggregator eventAggregator
        )
    {
        _authService = authService;
        _settingsService = settingsService;

        _settings = _settingsService.Get();
        _userName = _settings.Login.LoginName;
        _licenceKey = _settings.Login.LicenseKey;

        AuthCommand = new DelegateCommand(SubmitAsync, CanSubmit);
        CancelCommand = new DelegateCommand(CloseDialog);

        _eventAggregator = eventAggregator;
        _eventAggregator
            .GetEvent<SubmitAsyncEvent>()
            .Subscribe(SubmitAsync);

        //_messageBoxService = messageBoxService;
    }

    private string _title = string.Empty;
    public override string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    private string _licenceKey;
    public string LicenceKey
    {
        get => _licenceKey;
        set
        {
            ErrorMessage = string.Empty;
            SetProperty(ref _licenceKey, value.Trim(), RaiseCanExecuteChanged);
        }
    }

    private string _userName;
    public string UserName
    {
        get => _userName;
        set
        {
            ErrorMessage = string.Empty;
            SetProperty(ref _userName, value.Trim(), RaiseCanExecuteChanged);
        }
    }

    private bool _isSubmiting;
    public bool IsSubmiting
    {
        get => _isSubmiting;
        set
        {
            ErrorMessage = string.Empty;
            if (SetProperty(ref _isSubmiting, value, RaiseCanExecuteChanged))
            {
                RaisePropertyChanged(nameof(IsInputEnabled));
            }
        }
    }

    public bool IsInputEnabled => !IsSubmiting;

    private string _errorMessage;
    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public DelegateCommand AuthCommand { get; }
    public DelegateCommand CancelCommand { get; }

    public IAuthResult AuthResult { get; private set; }

    private void RaiseCanExecuteChanged()
    {
        AuthCommand.RaiseCanExecuteChanged();
    }

    private void SubmitAsync()
    {
        Task.Run(Submit);
    }

    private async void Submit()
    {
        string errorMessage = string.Empty;

        try
        {
            IsSubmiting = true;

            // confirmation dialog when switching licenses
            if (!_licenceKey.StartsWith("KEY") &&
                !_authService.IsLicenseActive(_licenceKey))
            {
                //var modalResult = this.InvokeOnUiThread(() =>
                //{
                //    return _messageBoxService.ShowDialog(
                //        new MessageBox.Services.MessageBoxOptions
                //        {
                //            Title = "Warning",
                //            Text = "Do you want to activate another license? Current one will not be active anymore.",
                //            Icon = SystemIcons.Warning,
                //            Buttons = MessageBoxButton.OKCancel,
                //            ContentButtons = new MessageBoxContentButtonsViewModel
                //            {
                //                ContentOkButton = "Activate"
                //            }
                //        });
                //});
                //if (modalResult != ButtonResult.OK)
                //    return;
            }

            // try login with entered user name and password
            AuthResult = _authService.Login(_userName, _licenceKey);

            // store auth info to reuse next startup
            _settings.Login.Set(_userName, _licenceKey);
            await _settingsService.Save();
            //await _authService.LoginAsync();

            // close dialog
            CloseDialog(ButtonResult.OK);

            // hiding spinner in case of successfull reconnection
            //if (_mainWindow != null)
            //    Application.Current.Dispatcher.Invoke(() => _mainWindow.HideWaitIndicator());

            return;
        }
        catch (AuthenticationException ex)
        {
            errorMessage = $"Login failed: Invalid email or licence key";
        }
        catch (WebException ex)
        {
            //ExceptionHandler.ShowException(ex);
            errorMessage = $"Login failed: {ex.Message}";
        }
        catch (Exception ex)
        {
            errorMessage = "Error with login";
        }
        finally
        {
            IsSubmiting = false;
        }

        ErrorMessage = errorMessage;
    }

    private bool CanSubmit()
    {
        return !string.IsNullOrEmpty(_licenceKey)
            && !string.IsNullOrEmpty(_userName)
            && !_isSubmiting;
    }
}
