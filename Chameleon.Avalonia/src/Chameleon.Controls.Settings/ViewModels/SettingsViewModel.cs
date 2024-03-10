using Chameleon.Authorization;
using Chameleon.Avalonia.Prism.Module.Base;
using Chameleon.Common.Helpers;
using Chameleon.Common.Regions;
using Chameleon.Domain.Entities;
using Chameleon.Interfaces.App.Settings;
using Chameleon.Interfaces.Auth;
using Chameleon.Interfaces.Settings;
using Chameleon.Prism.Events;
using Prism.Commands;
using Prism.Regions;

namespace Chameleon.Avalonia.Controls.Settings.ViewModels;

public class SettingsViewModel
       : ViewModelBase
       , ISettingsViewModel
{
    private readonly IEventAggregator _eventAggregator;


    public SettingsViewModel(IEventAggregator eventAggregator, 
        //for init
        IUserDefaultSettingsViewModel userDefaultSettingsViewModel)
    {
        _eventAggregator = eventAggregator;

        _eventAggregator
            .GetEvent<ChangeSelectedTabIndexEvent>()
            .Subscribe(args => SelectedIndex = args.SelectedIndex);

        _eventAggregator
          .GetEvent<LoginSuccessEvent>()
          .Subscribe(args => InitializeTabControl());

        CustomTabs = new CustomTabs();

        Title = "Settings";
    }

    public DelegateCommand<string> CmdNavigateToChild => new ((param) =>
    {
        var source = nameof(UserDefaultSettingsView); 
        switch (param)
        {
            case "DEFAULTS":
                source = nameof(UserDefaultSettingsView);
                break;

            case "PROXY":
                source = nameof(UserProxySettingsView);
                break;

            case "PROXYCREDIT":
                source = nameof(ProxyCreditView);
                break;

            case "PHONEVERIFICATION":
                source = nameof(PhoneVerificationView);
                break;

            default:
                break;
        }

        RegionManager.RequestNavigate(RegionNames.ContentRegion, source);
    });

    public override void OnNavigatedFrom(NavigationContext navigationContext)
    {
        base.OnNavigatedFrom(navigationContext);
    }

    public EventHandler ChangeSettingTab { get; set; }
    public CustomTabs CustomTabs { get; set; }

    private int _selectedIndex;
    public int SelectedIndex
    {
        get => _selectedIndex;
        set
        {
            if (SetProperty(ref _selectedIndex, value))
            {
                ChangeSettingTab?.Invoke(this, null);
            }
        }
    }

    private void InitializeTabControl()
    {
        //TODO: refactor
        var currentUser = CurrentApplicationUser.Current.GetCurrentUser();

        if (currentUser.HasPemission(PermissionNames.Pages_Proxy))
        {
            CustomTabs.HasProxySettingsView = true;
        }

        if (currentUser.HasPemission(PermissionNames.Pages_ProxyCredits))
        {
            CustomTabs.HasProxyCredit = true;
        }

        if (currentUser.HasPemission(PermissionNames.Pages_Users_Primary))
        {
            CustomTabs.HasPhoneVerification = true;
            CustomTabs.HasAssistantUsers = true;
        }

        if (currentUser.HasPemission(PermissionNames.Pages_ImportExport))
        {
            CustomTabs.HasImport = true;
            CustomTabs.HasExport = true;
        }
    }
}