using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Chameleon.Avalonia.Controls.Settings.ViewModels;
using Chameleon.Core.Attributes;
using Chameleon.Interfaces.Settings;

namespace Chameleon.Avalonia.Controls.Settings;

[ViewModel(typeof(UserDefaultSettingsViewModel))]
public partial class UserDefaultSettingsView : UserControl
        , IUserDefaultSettingsView
{
    public UserDefaultSettingsView()
    {
        InitializeComponent();
    }
}