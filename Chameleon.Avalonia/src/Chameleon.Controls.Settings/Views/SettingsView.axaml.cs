using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Chameleon.Avalonia.Controls.Settings.ViewModels;
using Chameleon.Core.Attributes;
using Chameleon.Interfaces.Settings;

namespace Chameleon.Avalonia.Controls.Settings;

[ViewModel(typeof(SettingsViewModel))]
public partial class SettingsView : UserControl
        , ISettingsView
{
    public SettingsView()
    {
        InitializeComponent();
    }

    public void SetTabContent(SettingTabs tab)
    {
        throw new NotImplementedException();
    }
}