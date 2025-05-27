using Chameleon.client.Features.ProfilesAndFolders.Folders;
using Chameleon.client.Features.ProfilesAndFolders.Profiles.MyProfiles;
using Chameleon.Av.Fluent.Common.Pages;

namespace Chameleon.client.Features.ProfilesAndFolders.Projects;

[Chameleon.lib.Common.Attributes.ViewModel(typeof(ProjectsViewModel))]
public partial class ProjectsView : ChameleonNavigationPage {
	private MyProfilesView? myProfilesView;
	private FoldersView? foldersView;
	public ProjectsView() {
		InitializeComponent();
	}

	public override async void OnAfterNavigatedTo() {
		base.OnAfterNavigatedTo();
		if (foldersView == null) {
			await Task.Delay(54);
			foldersView = new FoldersView();
			FoldersPanel.Content = foldersView;
		}

		if (myProfilesView == null) {
			await Task.Delay(54);
			myProfilesView = new MyProfilesView();
			ProfilesPanel.Content = myProfilesView;
		}
	}
}