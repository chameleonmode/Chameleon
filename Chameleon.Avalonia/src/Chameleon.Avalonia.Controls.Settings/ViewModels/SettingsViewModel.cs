using Chameleon.Authorization;
using Chameleon.CT.Common.Base;
using Chameleon.Interfaces.App.ImportExport.Views;
using Chameleon.Interfaces.App.ProxyCredit.Views;
using Chameleon.Interfaces.App.UserSettings.View;
using Chameleon.Interfaces.Auth;
using Chameleon.Interfaces.Settings;
using Chameleon.Interfaces.UserSettings;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Avalonia.Styling;
using Avalonia;
using FluentAvalonia.Styling;
using Avalonia.Media;
using System.ComponentModel;

namespace Chameleon.Avalonia.Controls.Settings.ViewModels;

public partial class SettingsViewModel
       : PageViewModelBase
       , ISettingsViewModel
{                        
    private const string _system = "System";
    private const string _dark = "Dark";
    private const string _light = "Light";

    //TODO: refactor
    public string CurrentVersion { get; } = "2024.0.9.9";

    public string[] AppThemes => [_system, _light, _dark /*, FluentAvaloniaTheme.HighContrastTheme*/];
    public List<Color> PredefinedColors => new List<Color>
        {
            Color.FromRgb(255,185,0),
            Color.FromRgb(255,140,0),
            Color.FromRgb(247,99,12),
            Color.FromRgb(202,80,16),
            Color.FromRgb(218,59,1),
            Color.FromRgb(239,105,80),
            Color.FromRgb(209,52,56),
            Color.FromRgb(255,67,67),
            Color.FromRgb(231,72,86),
            Color.FromRgb(232,17,35),
            Color.FromRgb(234,0,94),
            Color.FromRgb(195,0,82),
            Color.FromRgb(227,0,140),
            Color.FromRgb(191,0,119),
            Color.FromRgb(194,57,179),
            Color.FromRgb(154,0,137),
            Color.FromRgb(0,120,212),
            Color.FromRgb(0,99,177),
            Color.FromRgb(142,140,216),
            Color.FromRgb(107,105,214),
            Color.FromRgb(135,100,184),
            Color.FromRgb(116,77,169),
            Color.FromRgb(177,70,194),
            Color.FromRgb(136,23,152),
            Color.FromRgb(0,153,188),
            Color.FromRgb(45,125,154),
            Color.FromRgb(0,183,195),
            Color.FromRgb(3,131,135),
            Color.FromRgb(0,178,148),
            Color.FromRgb(1,133,116),
            Color.FromRgb(0,204,106),
            Color.FromRgb(16,137,62),
            Color.FromRgb(122,117,116),
            Color.FromRgb(93,90,88),
            Color.FromRgb(104,118,138),
            Color.FromRgb(81,92,107),
            Color.FromRgb(86,124,115),
            Color.FromRgb(72,104,96),
            Color.FromRgb(73,130,5),
            Color.FromRgb(16,124,16),
            Color.FromRgb(118,118,118),
            Color.FromRgb(76,74,72),
            Color.FromRgb(105,121,126),
            Color.FromRgb(74,84,89),
            Color.FromRgb(100,124,100),
            Color.FromRgb(82,94,84),
            Color.FromRgb(132,117,69),
            Color.FromRgb(126,115,95)
        };

    private readonly IApplicationUser _applicationUser;
    private readonly IApplicationSettingsService _settingsService;
                                           
    private IApplicationSettings _settings;
    private readonly FluentAvaloniaTheme _faTheme;

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

    [ObservableProperty]
    string _currentAppTheme = _system;
    [ObservableProperty]
    bool _useCustomAccentColor = false;
    [ObservableProperty]
    Color _customAccentColor = Colors.SlateBlue;
    [ObservableProperty]
    Color? _listBoxColor;


    [ObservableProperty]
    string _liscencedTo;

    public SettingsViewModel(IApplicationUser user, IApplicationSettingsService settingsService) 
        : base("Settings")
    {           
        _applicationUser = user;
        _settingsService = settingsService;
                                         
        _faTheme = Application.Current.Styles[0] as FluentAvaloniaTheme;

        EventAggregator
          .GetEvent<LoginSuccessEvent>()
          .Subscribe(async args => await InitializeLoginSucces());
    }

    private async Task InitializeLoginSucces()
    {
        _settings = await _settingsService.GetAsync();
        CurrentAppTheme = _settings.Settings.CurrentAppTheme;
        if (_settings.Settings.UseCustomAccentColor && _settings.Settings.CustomAccentColor is string coler)
            UpdateAppAccentColor(Color.Parse(coler));
        UseCustomAccentColor = _settings.Settings.UseCustomAccentColor;
        LiscencedTo = $"Licensed to: {_applicationUser.Email}";
        InitializeTabControl();
    }

    public override async Task InitAsync(object? param)
    {
        await base.InitAsync(param);

        if (!Loaded)
        {
            await InitializeLoginSucces();
        }
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

        NavigationService.NavigateToType(type,param);
    }

    [RelayCommand]
    public async Task Logout()
    {
       await _settingsService.Logout();
    }

    //TODO: refactor
    bool save = true;
    partial void OnUseCustomAccentColorChanged(bool oldValue, bool newValue)
    {
        if(Loaded)
save = false;
        if (newValue)
        {
            if (_faTheme.TryGetResource("SystemAccentColor", null, out var curColor))
            {
                CustomAccentColor = (Color)curColor;
                ListBoxColor = CustomAccentColor;
            }
            else
            {
                // This should never happen, if it does, something bad has happened
                throw new Exception("Unable to retreive SystemAccentColor");
            }
        }
        else
        {
            CustomAccentColor = default;
            ListBoxColor = default;
            UpdateAppAccentColor(null);
        }

        Save();
    }

    partial void OnCustomAccentColorChanged(Color oldValue, Color newValue)
    {
        UpdateAppAccentColor(newValue);
    }
    partial void OnListBoxColorChanged(Color? oldValue, Color? newValue)
    {
        UpdateAppAccentColor(newValue);
    }

    partial void OnCurrentAppThemeChanged(string? oldValue, string newValue)
    {
        ThemeVariant GetThemeVariant(string value)
        {
            switch (value)
            {
                case _light:
                    return ThemeVariant.Light;
                case _dark:
                    return ThemeVariant.Dark;
                case _system:
                default:
                    return null;
            }
        }

        var newTheme = GetThemeVariant(newValue);
        if (newTheme != null)
        {
            Application.Current.RequestedThemeVariant = newTheme;
        }
        if (newValue != _system)
        {
            _faTheme.PreferSystemTheme = false;
        }
        else
        {
            _faTheme.PreferSystemTheme = true;
        }
if(save)
        Save();
    }
    private async void Save()
    {
        if (Loaded)
        {
            _settings.Settings.CurrentAppTheme = CurrentAppTheme;
            _settings.Settings.CustomAccentColor = _faTheme.CustomAccentColor?.ToString();
            _settings.Settings.UseCustomAccentColor = UseCustomAccentColor;

            await _settingsService.Save();
            save = true;
        }
    }
    private void UpdateAppAccentColor(Color? color)
    {
        if (_faTheme.CustomAccentColor != color)
            _faTheme.CustomAccentColor = color;

if(save)
        Save();
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