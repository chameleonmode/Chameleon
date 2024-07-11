using Chameleon.CT.Common.Base;
using Chameleon.Core.Util;
using Chameleon.Interfaces.Settings;
using Chameleon.Interfaces.UserProfiles;
using Chameleon.Interfaces.UserSettings;
using Chameleon.Prism.Events;
using CommunityToolkit.Mvvm.Input;

namespace Chameleon.Avalonia.Controls.Settings.ViewModels;

public partial class UserDefaultSettingViewModel : SubPageViewModelBase
{
    private readonly IEventAggregator _eventAggregator;
    private readonly IUserDefaultSetting _userDefaultSetting;
    private readonly IUserDefaultSettingsService _userDefaultsSettingsService;

    public UserDefaultSettingViewModel(
        IEventAggregator eventAggregator,
        IUserDefaultSetting userDefaultSetting,
        IUserDefaultSettingsService userDefaultsSettingsService
        )
    {
        _eventAggregator = eventAggregator;
        _userDefaultSetting = userDefaultSetting;
        _userDefaultsSettingsService = userDefaultsSettingsService;

        _defaultUrl = _userDefaultSetting.DefaultUrl;
    }


    private bool _hasChanged;

    public bool HasChanged
    {
        get { return _hasChanged; }
        set { _hasChanged = value; }
    }

    public string _defaultUrl;
    public string DefaultUrl
    {
        get => _defaultUrl;
        set
        {
            if (SetProperty(ref _defaultUrl, value))
            {
                _hasChanged = true;
            }
        }
    }

    private bool _isChecked;
    public bool IsChecked
    {
        get => _isChecked;
        set
        {
            if (SetProperty(ref _isChecked, value))
            {
                ChangeSelected();
            }
        }
    }

    private void ChangeSelected()
    {
        _eventAggregator
                    .GetEvent<SelectedUserDefaultSettingEvent>()
                    .Publish(new SelectedUserDefaultSettingEventArgs(_isChecked));
    }

    [RelayCommand]
    public void SaveUrlFromViewText()
    {
        if (string.IsNullOrWhiteSpace(DefaultUrl))
        {
            return;
        }

        HasChanged = false;

        _userDefaultSetting.DefaultUrl = DefaultUrl;
        _userDefaultsSettingsService.Save(_userDefaultSetting);
    }

    [RelayCommand]
    public void DeleteDefaultSettings()
    {
        _eventAggregator
               .GetEvent<DeleteUserDefaultSettingsEvent>()
               .Publish(new UserDefaultSettingsEventArgs(_userDefaultSetting));
        ChangeSelected();
    }
}
