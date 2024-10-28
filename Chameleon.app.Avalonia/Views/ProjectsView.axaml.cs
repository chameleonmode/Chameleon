
using Avalonia;
using Avalonia.Interactivity;

using Chameleon.app.Avalonia.Controls;
using Chameleon.app.Avalonia.ViewModels;
using Chameleon.app.Avalonia.ViewModels.Controllers;
using Chameleon.Av.Fluent.Common.Pages;

namespace Chameleon.app.Avalonia.Views;

[Chameleon.lib.Common.Attributes.ViewModel(typeof(ProjectsViewModel))]
public partial class ProjectsView : ChameleonNavigationPage {
	private UserProfilesUserControl? userProfilesUserControl;
	private FoldersUserControl? foldersUserControl;
	public ProjectsView()
	{
		InitializeComponent();
	}

	public override async void OnAfterNavigatedTo()
	{
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