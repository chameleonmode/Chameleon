using Chameleon.Maui.Pages.Settings.ViewModels;
using Chameleon.Maui.Toolkit.Base;

namespace Chameleon.Maui.Pages.Settings.Views;

public partial class MainSettingsPage : BasePage<MainSettingsPageViewModel>
{
    public MainSettingsPage(MainSettingsPageViewModel vm)
        : base(vm)
    {
        InitializeComponent();
    }
}