using Chameleon.client.UI.Pages;

namespace Chameleon.client.Features.Dashboard;
[lib.Common.Attributes.ViewModel(typeof(ViewModel))]
public partial class View : ChameleonNavigationPage {
    public View() {
        InitializeComponent();
    }
}