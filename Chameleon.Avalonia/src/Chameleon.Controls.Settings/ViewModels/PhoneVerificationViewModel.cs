using Chameleon.Avalonia.Prism.Module.Base;
using Chameleon.CT.Common.Base;
using Chameleon.Interfaces.App.Settings;
using Chameleon.Interfaces.Settings;
using Chameleon.Prism.Events;
using Prism.Commands;

namespace Chameleon.Avalonia.Controls.Settings.ViewModels;

public class PhoneVerificationViewModel
       : SubPageViewModelBase
       , IPhoneVerificationViewModel
{
    private IUserSetting _userSetting;
    private readonly IUserSettingsService _userSettingsService;

    public PhoneVerificationViewModel(
        IUserSettingsService userSettingsService
        )
    {
        Title = "Phone Verification";            

        _userSettingsService = userSettingsService;

        SaveCommand = new DelegateCommand(Save)
            .ObservesCanExecute(() => IsChangeApiKey);
    }
    public override Task LoadAsync()
    {
        if (!base.Loaded)
            InitializeApiKey();
        return base.LoadAsync();
    }
    private void InitializeApiKey()
    {
        _userSetting = _userSettingsService.Get();
        _apiKey = _userSetting.SmsPvaApiKey;
    }

    public DelegateCommand SaveCommand { get; }
    public void Save()
    {
        _userSettingsService.Save(_userSetting);
        IsChangeApiKey = false;
    }

    private string _apiKey;
    public string ApiKey
    {
        get => _apiKey;
        set
        {
            if (SetProperty(ref _apiKey, value))
            {
                IsChangeApiKey = true;
                _userSetting.SmsPvaApiKey = _apiKey;
            }
        }
    }

    private bool _isChangeApiKey;
    public bool IsChangeApiKey
    {
        get => _isChangeApiKey;
        set => SetProperty(ref _isChangeApiKey, value);
    }
}
