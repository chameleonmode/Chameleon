using Chameleon.app.Avalonia.ViewModels;
using Chameleon.Av.Fluent.Common.Controls;

namespace Chameleon.app.Avalonia.Views;

[Chameleon.lib.Common.Attributes.ViewModel(typeof(SettingsViewModel))]
public partial class SettingsView : ViewControlBase<SettingsViewModel> {
    public SettingsView()
    {
        InitializeComponent();
    }
}