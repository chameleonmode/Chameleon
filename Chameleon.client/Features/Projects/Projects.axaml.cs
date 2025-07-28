using Chameleon.client.UI.Pages;

namespace Chameleon.client.Features.Projects;

public partial class View : ChameleonNavigationPage {
	public View() {
		InitializeComponent();
	}

	protected override object? ViewModel => Projects.I;
}