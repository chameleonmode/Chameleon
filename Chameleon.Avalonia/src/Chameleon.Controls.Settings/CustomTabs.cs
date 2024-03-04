using Prism.Mvvm;

namespace Chameleon.Avalonia.Controls.Settings;

public class CustomTabs : BindableBase
{
    private bool _hasProxySettingsView;
    public bool HasProxySettingsView
    {
        get => _hasProxySettingsView;
        set
        {
            if (SetProperty(ref _hasProxySettingsView, value))
            {
                RaisePropertyChanged(nameof(HasProxySettingsView));
            }
        }
    }

    private bool _hasProxyCredit;
    public bool HasProxyCredit
    {
        get => _hasProxyCredit;
        set
        {
            if (SetProperty(ref _hasProxyCredit, value))
            {
                RaisePropertyChanged(nameof(HasProxyCredit));
            }
        }
    }

    private bool _hasPhoneVerification;
    public bool HasPhoneVerification
    {
        get => _hasPhoneVerification;
        set
        {
            if (SetProperty(ref _hasPhoneVerification, value))
            {
                RaisePropertyChanged(nameof(HasPhoneVerification));
            }
        }
    }

    private bool _hasAssistantUsers;
    public bool HasAssistantUsers
    {
        get => _hasAssistantUsers;
        set
        {
            if (SetProperty(ref _hasAssistantUsers, value))
            {
                RaisePropertyChanged(nameof(HasAssistantUsers));
            }
        }
    }

    public bool _hasImport;
    public bool HasImport
    {
        get => _hasImport;
        set
        {
            if (SetProperty(ref _hasImport, value))
            {
                RaisePropertyChanged(nameof(HasImport));
            }
        }
    }

    public bool _hasExport;
    public bool HasExport
    {
        get => _hasExport;
        set
        {
            if (SetProperty(ref _hasExport, value))
            {
                RaisePropertyChanged(nameof(HasExport));
            }
        }
    }
}
