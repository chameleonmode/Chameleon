namespace Chameleon.Avalonia.Controls.Settings.Functional.ViewModels;
public class FunctionalSettingsViewModel : PageViewModelBase,
    IFunctionalSettingsViewModel
{
    public int LastSelectedIndex { get; set; } = -1;
}
