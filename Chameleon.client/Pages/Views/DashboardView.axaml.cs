using Chameleon.Av.Fluent.Common.Pages;
namespace Chameleon.client.Pages.Views;

[lib.Common.Attributes.ViewModel(typeof(Features.Dashboard.ViewModel))]
public partial class DashboardView : ChameleonNavigationPage {
    public DashboardView() {
        InitializeComponent();
    }
}