using Chameleon.Av.Fluent.Common.Pages;

namespace Chameleon.client.Features.Dashboard;
[lib.Common.Attributes.ViewModel(typeof(ViewModel))]
public partial class View : ChameleonNavigationPage {
    public View() {
        InitializeComponent();
    }
}