using Chameleon.app.Avalonia.ViewModels;
using Chameleon.Av.Fluent.Common.Pages;

namespace Chameleon.app.Avalonia.Views;

[Chameleon.lib.Common.Attributes.ViewModel(typeof(SettingsViewModel))]
public partial class SettingsView : ChameleonNavigationPage {
    public SettingsView()
    {
        InitializeComponent();
    }
}