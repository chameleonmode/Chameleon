using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Chameleon.Av.Fluent.Common.Controls;
using Chameleon.Avalonia.Controls.Settings.ViewModels;
using Chameleon.Common.Helpers;
using Chameleon.Core.Attributes;
using Chameleon.Interfaces.App.Settings;
using Chameleon.Interfaces.Dialogs.ViewModels;
using Chameleon.Interfaces.Settings;

namespace Chameleon.Avalonia.Controls.Settings;

public partial class UserDefaultSettingsView : SubPageViewControl
        , IUserDefaultSettingsView
{
    public UserDefaultSettingsView()
    {
        InitializeComponent();
    }
}