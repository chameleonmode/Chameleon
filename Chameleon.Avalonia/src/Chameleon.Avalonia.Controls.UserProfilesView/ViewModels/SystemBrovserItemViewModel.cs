using Chameleon.Interfaces.WebBrowser;
using Chameleon.CT.Common.Base;

namespace Chameleon.Avalonia.Controls.UserProfilesView.ViewModels;

public class SystemBrovserItemViewModel
    : SubPageViewModelBase
{
    public SystemBrovserItemViewModel(SystemBrowserType systemBrowserType)
    {
        SystemBrowserType = systemBrowserType;
    }

    private SystemBrowserType _systemBrowserType;
    public SystemBrowserType SystemBrowserType
    {
        get => _systemBrowserType;
        set => SetProperty(ref _systemBrowserType, value);
    }

    public string IconName => SystemBrowserType.ToString().ToLower();
}
