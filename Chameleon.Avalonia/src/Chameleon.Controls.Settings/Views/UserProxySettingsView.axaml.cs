using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Chameleon.Av.Fluent.Common.Controls;
using Chameleon.Common.Helpers;
using Chameleon.Core.Attributes;
using Chameleon.Interfaces.App.Settings;
using Chameleon.Interfaces.Settings;
using Chameleon.Interfaces.UserSettings;

namespace Chameleon.Avalonia.Controls.Settings;

public partial class UserProxySettingsView : SubPageViewControl, IUserProxySettingsView
{
    public UserProxySettingsView()
    {
        InitializeComponent();    
    }
}