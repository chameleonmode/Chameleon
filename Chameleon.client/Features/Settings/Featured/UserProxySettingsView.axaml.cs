using Chameleon.client.UI.Pages;
using FluentAvalonia.UI.Controls;

namespace Chameleon.client.Features.Settings.Featured;

public partial class UserProxySettingsView : ChameleonPageBase {
    public UserProxySettingsView()
    {
        InitializeComponent();
        ControlName = "Proxy Settings";
        Description = "Customize multiple profiles proxy settings";
        PreviewImage = App.TryGetResource<IconSource>("Proxy");
    }
}