using Chameleon.Auth.Api;
using Chameleon.Interfaces.Dialogs.ViewModels;
using Chameleon.Interfaces.Dialogs.Views;
using Chameleon.Interfaces.MessageBox;

namespace Chameleon.Av.Fluent.Dialogs.ViewModels;

public partial class AuthTaskDialogViewModel : DialogBase, IAuthTaskDialogViewModel
{
    private readonly IAuthApiClient _apiClient;
    private readonly IApplicationSettings _settings;
    private readonly IApplicationSettingsService _settingsService;
    public AuthTaskDialogViewModel(IAuthApiClient authService,
        IApplicationSettingsService settingsService)
    {
        title = "User Login";

        _apiClient = authService;
        _settingsService = settingsService;

        _settings = _settingsService.Get();
        UserName = _settings.Login.LoginName;
        LicenceKey = _settings.Login.LicenseKey;


        IsInputEnabled = true;
    }

    [ObservableProperty]
    private string? licenceKey;

    [ObservableProperty]
    //[NotifyCanExecuteChangedFor(nameof(SubmitCommand))]
    private string? userName;

    [ObservableProperty]
    private bool _isSubmiting;

    [ObservableProperty]
    private string? _errorMessage;

    //[RelayCommand(IncludeCancelCommand = true, CanExecute = nameof(CanSubmit))]
    private async Task<IContentDialogResult> SubmitAsync()
    {
        IContentDialogResult result = IContentDialogResult.None;
        IsSubmiting = true;
        IsInputEnabled = false;
        try
        {
            await DoSave();
            if (LicenceKey is not null && !LicenceKey.StartsWith("KEY") &&
                !await _apiClient.IsLicenseActiveAsync(LicenceKey))
            {
                result = await MesageBoxHelper.ShowAsync("Warning", "Do you want to activate another license? Current one will not be active anymore.") ? IContentDialogResult.Primary : IContentDialogResult.Secondary;
            }
            else 
                result = IContentDialogResult.Primary;
        }
        catch (AuthenticationException ex)
        {
            ErrorMessage = $"Login failed: Invalid email or licence key: {ex.Message}";
        }
        catch (WebException ex)
        {
            //TODO: ExceptionHandler.ShowException(ex);
            ErrorMessage = $"Login failed: {ex.Message}";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error with login: {ex.Message}";
        }
        finally
        {
            IsSubmiting = false;
            IsInputEnabled = true;
        }

        return result;
    }

    public override async Task<IContentDialogResult> ShowAsync()
    {
        var result = await ContentDialogService.ShowContentDialogAsync(typeof(ILoginContentDialogContent));
        if (result == IContentDialogResult.Primary)
        {
            result = await SubmitAsync();
        }
        return result;
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