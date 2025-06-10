using Chameleon.client.UI.Pages;

namespace Chameleon.client.Features.Dashboard;
public partial class View : ChameleonNavigationPage {
    public View() {
        InitializeComponent();
    }
	protected override object? ViewModel => Dashboard.ViewModel.Instance;
}