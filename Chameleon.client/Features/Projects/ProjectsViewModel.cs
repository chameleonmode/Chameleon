using Chameleon.lib.Api;
using Chameleon.lib.Common.Constants;
using Chameleon.lib.Common.Util;
using CommunityToolkit.Mvvm.Input;
using Chameleon.lib.CommunityToolkit.MvvM;

using Chameleon.lib.Helpers;
using Chameleon.client.Features.ProfilesAndFolders.Profiles.Identity;
using Chameleon.app.Avalonia;
using Chameleon.client.Features.Projects.Folders;
using System.Collections.ObjectModel;
using Chameleon.client.Features.Projects.Profiles;

namespace Chameleon.client.Features.Projects;
public abstract partial class Projector(string? title = null) : ViewModelObjectBase(title) {
  public ReadOnlyObservableCollection<ObsProfile> Profiles { get; protected set; } = new([]);
  public ReadOnlyObservableCollection<ObsFolder> Folders { get; protected set; } = new([]);
	public bool HasNoFolderItems => Folders.Count == 0;
}
public partial class ProjectsViewModel : ViewModelObjectBase {
	public ProfilesViewModel Profiles { get; } = ProfilesViewModel.Instance;
	public FoldersViewModel Folders { get; } = FoldersViewModel.Instance;

	public bool IsCreateProfileBtnVisible => Auther.AuthSession?.CreatorUserId == null || Auther.AuthSession?.CanCreateProfiles == true;

	public ProjectsViewModel() : base() { }
	public override async Task OnNavigatedToAsync(object? param) {
		await base.OnNavigatedToAsync(param);
		if (param is ObsFolder folder) {
			if (!folder.Navigated || Folders.SelectedFolder?.Dto?.id == folder.Dto?.id) {
				await Folders.OnNavigatingTo(folder.Dto);
				folder.Navigated = true;
			}
		} else if (param is ObsProfile up) {
			if (!up.Navigated) {
				Profiles.OnFilterTo(up);
				up.Navigated = true;
			}
		} else {
			if (Folders.SelectedFolder != null)
				await Folders.OnNavigatingTo(Folders.SelectedFolder.Dto);
			else
				await Folders.OnNavigatingTo(null);

			if (param is string p)
				Profiles.SearchText = p;
		}
	}

	public bool IsDisabledCreateNewProfile = false;

	[RelayCommand]
	private async Task CreateProfile() {
		if (IsDisabledCreateNewProfile) {
			return;
		}
		//TODO:
		IsDisabledCreateNewProfile = true;
		try {
			var p = await Profiles.CreateNewProfile();
			Navigator.NavigateToType(typeof(IdentityView), p);
		} catch (Exception ex) {
			if (ex.Message == "limit_ex") {
				if (await MessageBox.Show("PROFILES LIMIT REACHED", "You have reached the maximum number of profiles."))
					ProUtil.GoToUrlDefault(Consts.PricingUrl);
			} else {
				Toaster.Error("Wooopsy?", ex.Message);
			}
		} finally {
			IsDisabledCreateNewProfile = false;
		}
	}
}
