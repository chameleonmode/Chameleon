namespace Chameleon.Avalonia.Controls.Settings.Functional.ViewModels;

public partial class PhoneVerificationViewModel
       : SubPageViewModelBase
       , IPhoneVerificationViewModel
{
    private IUserSetting _userSetting;
    private readonly IUserSettingsService _userSettingsService;

    [ObservableProperty]
    private string? _apiKey;

    public PhoneVerificationViewModel(
        IUserSettingsService userSettingsService
        )
    {
        Title = "Phone Verification";            

        _userSettingsService = userSettingsService;
    }
     
    public override async Task InitAsync(object? param)
    {                       
        await base.InitAsync(param);

        if (!Loaded)
            InitializeApiKey();
    }
    private void InitializeApiKey()
    {
        _userSetting = _userSettingsService.Get();
        ApiKey = _userSetting.SmsPvaApiKey;
    }

    [RelayCommand]
    public void Save()
    {
        _userSettingsService.Save(_userSetting);
        IsChangeApiKey = false;
    }
    partial void OnApiKeyChanged(string? value)
    {
        if (ApiKey != value)
        {
            IsChangeApiKey=true;
            _userSetting.SmsPvaApiKey = value;
        }
    }

    private bool _isChangeApiKey;
    public bool IsChangeApiKey
    {
        get => _isChangeApiKey;
        set => SetProperty(ref _isChangeApiKey, value);
    }
}
