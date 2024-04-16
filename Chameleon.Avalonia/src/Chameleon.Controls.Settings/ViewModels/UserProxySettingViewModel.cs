using AutoMapper;
using Chameleon.CT.Common.Base;
using Chameleon.Domain.Entities;
using Chameleon.Interfaces.UserProfiles;
using Chameleon.Prism.Events;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Chameleon.Avalonia.Controls.Settings.ViewModels;

public partial class UserProxySettingViewModel
       : SubPageViewModelBase
{         
    public static string InProject => "Profile";

    private readonly IMapper _mapper;
    private readonly IUserProfile _userProfile;
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

        //UserProfileModel = _mapper.Map<UserProfileBindable>(_userProfile);
        UserProfileModel = _mapper.Map<UserProfile>(_userProfile);
        _host = UserProfileModel.Proxy.Host;
        _port = ""+UserProfileModel.Proxy.Port;
        _userName = UserProfileModel.Proxy.UserName;
        _password = UserProfileModel.Proxy.Password;
    }
    public IUserProfile UserProfile => _userProfile;

    private UserProfile _userProfileModel;
    public UserProfile UserProfileModel
    {
        get => _userProfileModel;
        set
        {
            SetProperty(ref _userProfileModel, value);
        }
    }
    [ObservableProperty]
    private string? _host;
   // partial void OnHostChanged(string? oldValue, string? newValue) => UserProfileModel.Proxy.Host = newValue;
    [ObservableProperty]
    private string? _userName;
    //partial void OnUserNameChanged(string? oldValue, string? newValue) => UserProfileModel.Proxy.UserName = newValue;
    [ObservableProperty]
    private string? _password;
    //partial void OnPasswordChanged(string? oldValue, string? newValue) => UserProfileModel.Proxy.Password = newValue;

    [ObservableProperty]
    private string? _port;
  // partial void OnPortChanged(string? oldValue, string? newValue)
  // {
  //     if (int.TryParse(newValue, out var port)) { if (UserProfileModel.Proxy.Port != port) UserProfileModel.Proxy.Port = port; }
  // }
  //

    public string UserProfileTitle => _userProfile.Title ?? "<Title>";

    private bool _isSelected;
    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            SetProperty(ref _isSelected, value);

            if(!_openChangeProxies && value)
                ClickIconChangeProxies();

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

    public void SetProfile()
    {
        var host = Host;
        var port = int.TryParse(Port, out var po) ? po : _userProfile.Proxy.Port;
        var userName = UserName;
        var password = Password;

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
