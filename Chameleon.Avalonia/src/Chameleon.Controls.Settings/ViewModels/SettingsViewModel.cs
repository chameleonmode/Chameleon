using Chameleon.Authorization;
using Chameleon.Av.Fluent.Common.Services;
using Chameleon.Avalonia.Prism.Module.Base;
using Chameleon.Common.Helpers;
using Chameleon.Common.Regions;
using Chameleon.CT.Common.Base;
using Chameleon.Domain.Entities;
using Chameleon.Interfaces.App.ImportExport.Views;
using Chameleon.Interfaces.App.ProxyCredit.Views;
using Chameleon.Interfaces.App.Settings;
using Chameleon.Interfaces.App.UserSettings.View;
using Chameleon.Interfaces.Auth;
using Chameleon.Interfaces.Settings;
using Chameleon.Interfaces.UserSettings;
using Chameleon.Prism.Events;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Prism.Commands;
using Prism.Regions;

namespace Chameleon.Avalonia.Controls.Settings.ViewModels;

public partial class SettingsViewModel
       : ObservableObjectBase
       , ISettingsViewModel
{
    private readonly IApplicationUser _applicationUser;

    [ObservableProperty]
    private bool _hasProxySettingsView;
    [ObservableProperty]
    private bool _hasProxyCredit;
    [ObservableProperty]
    private bool _hasPhoneVerification;
    [ObservableProperty]
    private bool _hasAssistantUsers;
    [ObservableProperty]
    public bool _hasImport;
    [ObservableProperty]
    public bool _hasExport;

    public SettingsViewModel(IApplicationUser user)
    {
        _applicationUser = user;

        EventAggregator
          .GetEvent<LoginSuccessEvent>()
          .Subscribe(args => InitializeTabControl());

        Title = "Settings";
    }

    [RelayCommand]
    public void CmdNavigateToChild(string param)
    {
        var type = typeof(IUserDefaultSettingsView);
        switch (param)
        {
            case "DEFAULTS":
                type = typeof(IUserDefaultSettingsView);
                break;

            case "PROXY":
                type = typeof(IUserProxySettingsView);
                break;

            case "PROXYCREDIT":
                type = typeof(IProxyCreditView);
                break;

            case "PHONEVERIFICATION":
                type = typeof(IPhoneVerificationView);
                break;

            case "USERS":
                type = typeof(IAssistantUsersView);
                break;

            case "IMPORTPROFILES":
                type = typeof(IImportView);
                break;

            default:
                break;
        }

        NavigationService.Instance.NavigateToType(type,param);
    }

    private void InitializeTabControl()
    {
        //TODO: refactor
        if (_applicationUser.HasPemission(PermissionNames.Pages_Proxy))
        {
            HasProxySettingsView = true;
        }

        if (_applicationUser.HasPemission(PermissionNames.Pages_ProxyCredits))
        {
            HasProxyCredit = true;
        }

        if (_applicationUser.HasPemission(PermissionNames.Pages_Users_Primary))
        {
            HasPhoneVerification = true;
            HasAssistantUsers = true;
        }

        if (_applicationUser.HasPemission(PermissionNames.Pages_ImportExport))
        {
            HasImport = true;
            HasExport = true;
        }
    }
}