using Chameleon.client.UI.Pages;

namespace Chameleon.client.Features.Projects;

[Chameleon.lib.Common.Attributes.ViewModel(typeof(ProjectsViewModel))]
public partial class ProjectsView : ChameleonNavigationPage {
	public ProjectsView() {
		InitializeComponent();
	}
}