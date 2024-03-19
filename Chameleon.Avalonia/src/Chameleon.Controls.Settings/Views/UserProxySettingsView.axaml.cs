using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Chameleon.Av.Fluent.Common.Controls;
using Chameleon.Avalonia.Common.Helpers;
using Chameleon.Common.Helpers;
using Chameleon.Core.Attributes;
using Chameleon.Interfaces.App.Settings;
using Chameleon.Interfaces.Settings;
using Chameleon.Interfaces.UserSettings;
using FluentAvalonia.UI.Controls;

namespace Chameleon.Avalonia.Controls.Settings;

public partial class UserProxySettingsView : SubPageViewControl, IUserProxySettingsView
{
    public UserProxySettingsView()
    {
        InitializeComponent();
        ControlName = "Proxy Settings";
        Description = "Customize your default homepages here";
        PreviewImage = ApplicationHelper.TryGetResource<IconSource>("Proxy");
    }
}