using Chameleon.Av.Fluent.Common.Pages;

namespace Chameleon.client.Features.Projects;

[Chameleon.lib.Common.Attributes.ViewModel(typeof(ProjectsViewModel))]
public partial class ProjectsView : ChameleonNavigationPage {
	public ProjectsView() {
		InitializeComponent();
	}
}