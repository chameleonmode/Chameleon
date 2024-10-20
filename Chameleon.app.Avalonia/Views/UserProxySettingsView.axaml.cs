using Chameleon.app.Avalonia.app;
using Chameleon.Av.Fluent.Common.Pages;
using FluentAvalonia.UI.Controls;

namespace Chameleon.app.Avalonia.Views;

public partial class UserProxySettingsView : ChameleonPageBase {
    public UserProxySettingsView()
    {
        InitializeComponent();
        ControlName = "Proxy Settings";
        Description = "Customize multiple profiles proxy settings";
        PreviewImage = AppLayers.TryGetResource<IconSource>("Proxy")!;
    }
}