using Chameleon.client.UI.Pages;

using FluentAvalonia.UI.Controls;

namespace Chameleon.client.Features.Settings.Featured;

public partial class UserDefaultSettingsView : ChameleonPageBase
{
    public UserDefaultSettingsView()
    {
        InitializeComponent();
        ControlName = "Default Settings";
        Description = "Customize the default homepage and anonymity settings for your profiles";
        PreviewImage = App.TryGetResource<IconSource>("DefaultSettingsPageIcon");
    }
}