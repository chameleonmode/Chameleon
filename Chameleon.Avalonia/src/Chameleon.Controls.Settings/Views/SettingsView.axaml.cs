using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Chameleon.Interfaces.Settings;

namespace Chameleon.Avalonia.Controls.Settings;

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