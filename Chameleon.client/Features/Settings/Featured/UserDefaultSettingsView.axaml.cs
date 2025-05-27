using Chameleon.Av.Fluent.Common.Pages;

using FluentAvalonia.UI.Controls;

namespace Chameleon.app.Avalonia.Views;

public partial class UserDefaultSettingsView : ChameleonPageBase
{
    public UserDefaultSettingsView()
    {
        InitializeComponent();
        ControlName = "Default Settings";
        Description = "Customize the default homepage and anonymity settings for your profiles";
        PreviewImage = AppLayers.TryGetResource<IconSource>("DefaultSettingsPageIcon")!;
    }
}