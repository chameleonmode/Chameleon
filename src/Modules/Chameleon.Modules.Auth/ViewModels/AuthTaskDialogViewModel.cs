using Avalonia;
using Chameleon.Auth.Services;
using Chameleon.Common.Base;
using Chameleon.Core.Extensions;
using Chameleon.Interfaces.Auth;
using Chameleon.Interfaces.Auth.Events;
using Chameleon.Interfaces.Dialogs;
using Chameleon.Interfaces.MessageBox;
using Chameleon.Interfaces.Services;
using Chameleon.Interfaces.Settings;
using Chameleon.Prism.Events;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Drawing;
using System.Net;
using System.Net.Http.Headers;
using System.Runtime;
using System.Security.Authentication;

namespace Chameleon.Modules.Auth.ViewModels;

public partial class AuthTaskDialogViewModel : TaskDialogBase, IAuthTaskDialogViewModel
{
    private readonly IAuthService _authService;
    private readonly IApplicationSettings _settings;
    private readonly IApplicationSettingsService _settingsService;
    private readonly IEventAggregator _eventAggregator;
    private readonly ITaskDialogService _tasksDialogService;
    public AuthTaskDialogViewModel(IAuthService authService,
        IApplicationSettingsService settingsService,
        IEventAggregator eventAggregator,
        ITaskDialogService messageBoxService)
    {
        _authService = authService;
        _settingsService = settingsService;

        _settings = _settingsService.Get();
        UserName = _settings.Login.LoginName;
        LicenceKey = _settings.Login.LicenseKey;

        //CancelCommand = new DelegateCommand(CloseDialog);

        _eventAggregator = eventAggregator;
        _eventAggregator
            .GetEvent<SubmitAsyncEvent>()
            .Subscribe(async() => { await SubmitAsync(new CancellationToken()); });

        _tasksDialogService = messageBoxService;

        IsInputEnabled = true;
    }

    //private string _title = string.Empty;
    //public override string Title
    //{
    //    get => _title;
    //    set => SetProperty(ref _title, value);
    //}

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SubmitCommand))]
    private string? licenceKey;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SubmitCommand))]
    private string? userName;

    [ObservableProperty]
    private bool _isSubmiting;

    [ObservableProperty]
    private bool isInputEnabled;

    [ObservableProperty]
    private string? _errorMessage;

    //public IAuthResult AuthResult { get; private set; }


    [RelayCommand(IncludeCancelCommand = true, CanExecute = nameof(CanSubmit))]
    private async Task SubmitAsync(CancellationToken token)
    {
        string errorMessage = string.Empty;
        try
        {
             await DoSave();
            if (await NeedsConfirmActivation())
            {
                //await _tasksDialogService.ShowTaskDialog(typeof(IAuthDialogView));
                // new PrismMessageBoxOptions
                // {
                //     Owner = ParentWindow,
                //     Title = "Warning",
                //     Text = "Do you want to activate another license? Current one will not be active anymore.",
                //     Icon = SystemIcons.Warning,
                //     Buttons = MessageBoxButton.OKCancel,
                //     ContentButtons = new MessageBoxContentButtonsViewModel
                //     {
                //         ContentOkButton = "Activate"
                //     }
                // }, (r) => { CloseDialog(r); });
            }
            else
                Close(TaskDialogResul.OK);
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
    async Task<bool> NeedsConfirmActivation()
    {
        await Task.Delay(0);
        return LicenceKey is not null && !LicenceKey.StartsWith("KEY") &&
                !_authService.IsLicenseActive(LicenceKey);
    } 
    async Task DoSave()
    {
        // store auth info to reuse next startup
        _settings.Login.Set(userName, licenceKey);
        await _settingsService.Save();
    }
    async Task DoRequest()
    {
        var url = "https://api.chameleonmode.com/api/TokenAuth/IsLicenseActive?key=HHTQ-QJYS-ZMWX-CO5U";
        //var url = "https://github.com/explore";
        using HttpClient client = new HttpClient();
        var res = await client.GetAsync(url);
        res.EnsureSuccessStatusCode();
        var sr = await res.Content.ReadAsStringAsync();
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
        Close();
    }
}
