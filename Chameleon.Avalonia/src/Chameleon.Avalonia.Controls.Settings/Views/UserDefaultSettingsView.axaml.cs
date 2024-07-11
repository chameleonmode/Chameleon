using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Chameleon.Av.Fluent.Common.Pages;
using Chameleon.Avalonia.Common.Helpers;
using Chameleon.Avalonia.Controls.Settings.ViewModels;
using Chameleon.Common.Helpers;
using Chameleon.Core.Attributes;
using Chameleon.Interfaces.App.Settings;
using Chameleon.Interfaces.Dialogs.ViewModels;
using Chameleon.Interfaces.Settings;
using FluentAvalonia.UI.Controls;

namespace Chameleon.Avalonia.Controls.Settings;

public partial class UserDefaultSettingsView : SubPageViewControl
        , IUserDefaultSettingsView
{
    public UserDefaultSettingsView()
    {
        InitializeComponent();
        ControlName = "Default Settings";
        Description = "Customize the default homepage settings for your profiles";
        PreviewImage = ApplicationHelper.TryGetResource<IconSource>("DefaultSettingsPageIcon");
    }
}