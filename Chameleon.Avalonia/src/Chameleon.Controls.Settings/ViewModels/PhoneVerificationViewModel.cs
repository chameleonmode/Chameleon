using Chameleon.CT.Common.Base;
using Chameleon.Interfaces.App.Settings;
using Chameleon.Interfaces.Settings;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Chameleon.Avalonia.Controls.Settings.ViewModels;

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
     
    public override Task InitAsync(object? param)
    {
        //if (!base.Loaded)
            InitializeApiKey();
        return base.InitAsync(param);
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
