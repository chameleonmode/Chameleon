using Chameleon.app.Avalonia.ViewModels;
using Chameleon.Av.Fluent.Common.Pages;

namespace Chameleon.app.Avalonia.Views;

[Chameleon.lib.Common.Attributes.ViewModel(typeof(DashboardViewModel))]
public partial class DashboardView : ChameleonNavigationPage {
    public DashboardView()
    {
        InitializeComponent();
    }
}