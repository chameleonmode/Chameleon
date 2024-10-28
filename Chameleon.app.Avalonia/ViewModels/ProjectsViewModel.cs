using Chameleon.app.Avalonia.Models.Observable;
using Chameleon.app.Avalonia.ViewModels.Controllers;
using Chameleon.app.Avalonia.Views;
using Chameleon.lib.Api;
using Chameleon.lib.Common;
using Chameleon.lib.Common.Constants;
using Chameleon.lib.Common.Interfaces.Sys;
using Chameleon.lib.Common.ServiceManagers;
using Chameleon.lib.Common.Util;
using Chameleon.lib.CommunityToolkit.MvvM;

using CommunityToolkit.Mvvm.Input;

namespace Chameleon.app.Avalonia.ViewModels;
public partial class ProjectsViewModel : ViewModelObjectBase {
	public UserProfilesViewModel Profiles { get; } = UserProfilesViewModel.Instance;
	public UserProfileFoldersViewModel Folders { get; } = UserProfileFoldersViewModel.Instance;

	public bool IsCreateProfileBtnVisible => Auther.AuthSession?.CreatorUserId == null || Auther.AuthSession?.CanCreateProfiles == true;

	public ProjectsViewModel()
		: base("Profiles & Folders") 
	{
	}

	public override async Task OnNavigatedToAsync(object? param)
	{
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
	private async Task CreateProfile()
	{
		if (IsDisabledCreateNewProfile) {
			return;
		}
		//TODO:
		IsDisabledCreateNewProfile = true;
		try {
			var p = await Profiles.CreateNewProfile();
			Navigator.NavigateToType(typeof(UserProfileIdentityView), p);
		} catch (Exception ex) {
			if (ex.Message == "limit_ex") {
				if (await Mbox.Show("PROFILES LIMIT REACHED", "You have reached the maximum number of profiles."))
					ProUtil.GoToUrlDefault(Consts.GlobalSettings.PricingUrl);
			} else {
				Toaster.ShowErr("Wooopsy?", ex.Message);
			}
		} finally {
			IsDisabledCreateNewProfile = false;
		}
	}
}
