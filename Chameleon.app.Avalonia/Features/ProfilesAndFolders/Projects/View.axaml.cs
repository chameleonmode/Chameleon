using Chameleon.app.Avalonia.Controls;
using Chameleon.Av.Fluent.Common.Pages;
using UserProfilesUserControl = Chameleon.app.Avalonia.Features.ProfilesAndFolders.Profiles.MyProfiles.View;

namespace Chameleon.app.Avalonia.Features.ProfilesAndFolders.Projects;

[Chameleon.lib.Common.Attributes.ViewModel(typeof(ViewModel))]
public partial class View : ChameleonNavigationPage {
	private UserProfilesUserControl? userProfilesUserControl;
	private FoldersUserControl? foldersUserControl;
	public View() {
		InitializeComponent();
	}

	public override async void OnAfterNavigatedTo() {
		base.OnAfterNavigatedTo();
		if (foldersUserControl == null) {
			await Task.Delay(54);
			foldersUserControl = new FoldersUserControl();
			FoldersPanel.Content = foldersUserControl;
		}
		//_ = UserProfileFoldersViewModel.Instance.InitializeAsync(this);

		if (userProfilesUserControl == null) {
			await Task.Delay(54);
			userProfilesUserControl = new UserProfilesUserControl();
			ProfilesPanel.Content = userProfilesUserControl;
		}
		//_ = UserProfilesViewModel.Instance.InitializeAsync(this);
	}
}