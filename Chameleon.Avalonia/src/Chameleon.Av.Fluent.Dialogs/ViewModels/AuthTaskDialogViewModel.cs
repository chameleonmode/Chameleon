namespace Chameleon.Av.Fluent.Dialogs.ViewModels;

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
        Title = "User Login";

        _authService = authService;
        _settingsService = settingsService;

        _settings = _settingsService.Get();
        UserName = _settings.Login.LoginName;
        LicenceKey = _settings.Login.LicenseKey;

        //CancelCommand = new DelegateCommand(CloseDialog);

        _eventAggregator = eventAggregator;
        _eventAggregator
            .GetEvent<SubmitAsyncEvent>()
            .Subscribe(async () => { await SubmitAsync(new CancellationToken()); });

        _tasksDialogService = messageBoxService;

        IsInputEnabled = true;
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SubmitCommand))]
    private string? licenceKey;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SubmitCommand))]
    private string? userName;

    [ObservableProperty]
    private bool _isSubmiting;

    [ObservableProperty]
    private string? _errorMessage;

    //public IAuthResult AuthResult { get; private set; }


    [RelayCommand(IncludeCancelCommand = true, CanExecute = nameof(CanSubmit))]
    private async Task SubmitAsync(CancellationToken token)
    {
        try
        {
            await DoSave();
            if (NeedsConfirmActivation())
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
            return;
        }
        catch (AuthenticationException ex)
        {
            ErrorMessage = $"Login failed: Invalid email or licence key";
        }
        catch (WebException ex)
        {
            //TODO: ExceptionHandler.ShowException(ex);
            ErrorMessage = $"Login failed: {ex.Message}";
        }
        catch (Exception ex)
        {
            ErrorMessage = "Error with login";
        }
        finally
        {
            IsSubmiting = false;
        }
    }
    bool NeedsConfirmActivation()
    {
        return LicenceKey is not null && !LicenceKey.StartsWith("KEY") &&
                !_authService.IsLicenseActive(LicenceKey);
    }
    async Task DoSave()
    {
        // store auth info to reuse next startup
        _settings.Login.Set(UserName, LicenceKey);
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
}