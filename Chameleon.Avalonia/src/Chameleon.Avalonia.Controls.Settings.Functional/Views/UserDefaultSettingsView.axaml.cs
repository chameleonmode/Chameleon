namespace Chameleon.Avalonia.Controls.Settings.Functional.Views;

public partial class UserDefaultSettingsView : SubPageViewControl
        , IUserDefaultSettingsView
{
    public UserDefaultSettingsView()
    {
        InitializeComponent();
        ControlName = "Default Settings";
        Description = "Customize the default homepage and anonymity settings for your profiles";
        PreviewImage = ApplicationHelper.TryGetResource<IconSource>("DefaultSettingsPageIcon");
    }
}