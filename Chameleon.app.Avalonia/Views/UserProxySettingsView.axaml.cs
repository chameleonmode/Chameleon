using Chameleon.Av.Fluent.Common.Pages;
using Chameleon.Avalonia.Common.Helpers;
using Chameleon.Interfaces.UserSettings;
using FluentAvalonia.UI.Controls;

namespace Chameleon.app.Avalonia.Views;

public partial class UserProxySettingsView : SubPageViewControl, IUserProxySettingsView
{
    public UserProxySettingsView()
    {
        InitializeComponent();
        ControlName = "Proxy Settings";
        Description = "Customize multiple profiles proxy settings";
        PreviewImage = ApplicationHelper.TryGetResource<IconSource>("Proxy")!;
    }
}