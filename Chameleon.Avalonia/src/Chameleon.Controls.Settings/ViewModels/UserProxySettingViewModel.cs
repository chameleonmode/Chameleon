using AutoMapper;
using Chameleon.Avalonia.Controls.UserProfileView.Models.Profile;
using Chameleon.CT.Common.Base;
using Chameleon.Interfaces.UserProfiles;
using Chameleon.Prism.Events;
using CommunityToolkit.Mvvm.Input;

namespace Chameleon.Avalonia.Controls.Settings.ViewModels;

public partial class UserProxySettingViewModel
       : SubPageViewModelBase
{         
    public static string InProject => "Profile";

    private readonly IMapper _mapper;
    public readonly IUserProfile _userProfile;
    private readonly IEventAggregator _eventAggregator;

    public UserProxySettingViewModel(
        IMapper mapper,
        IUserProfile userProfile,
        IEventAggregator eventAggregator
        )
    {
        _mapper = mapper;
        _userProfile = userProfile;
        _eventAggregator = eventAggregator;

        UserProfileModel = _mapper.Map<UserProfileBindable>(_userProfile);
    }

    private UserProfileBindable _userProfileModel;
    public UserProfileBindable UserProfileModel
    {
        get => _userProfileModel;
        set
        {
            SetProperty(ref _userProfileModel, value);
        }
    }

    public string UserProfileTitle => _userProfile.Title ?? "<Title>";

    private bool _isSelected;
    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            SetProperty(ref _isSelected, value);
            ChangeSelected();
        }
    }

    public new string Title => $"{InProject} \"{UserProfileTitle}\"";

    private void ChangeSelected()
    {
        _eventAggregator
                    .GetEvent<SelectedChangeUserProfileEvent>()
                    .Publish(new SelectedUserProfileEventArgs(_userProfile, IsSelected));

        _eventAggregator
                    .GetEvent<SelectedUserProxySettingEvent>()
                    .Publish(new SelectedUserProxySettingEventArgs(IsSelected, _openChangeProxies));
        _openChangeProxies = false;
    }

    private string _code;
    public string Code
    {
        get
        {
            if (string.IsNullOrEmpty(_code))
            {
                var list = UserProfileTitle.Split(" ")
                    .Select(a => a.Trim().ToUpper()[0])
                    .ToList();

                if (list.Count > 2)
                {
                    list = list.Take(2).ToList();
                }

                _code = string.Join("", list);
            }

            return _code;
        }
    }

    public void SetProfile(IProxySettings proxySettings)
    {
        var host = proxySettings.Host;
        var port = proxySettings.Port;
        var userName = proxySettings.UserName;
        var password = proxySettings.Password;

        var profileProxy = _userProfile.Proxy;
        profileProxy.Host = host;
        profileProxy.Port = port;
        profileProxy.UserName = userName;
        profileProxy.Password = password;

        var modelProxy = UserProfileModel.Proxy;
        modelProxy.Host = host;
        modelProxy.Port = port;
        modelProxy.UserName = userName;
        modelProxy.Password = password;
    }

    private bool _openChangeProxies = false;
    [RelayCommand]
    private void ClickIconChangeProxies()
    {
        _openChangeProxies = true;
        IsSelected = true;
    }
}
