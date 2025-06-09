using System.Collections.ObjectModel;

using Chameleon.lib.Api;
using Chameleon.lib.Helpers;
using Chameleon.lib.Common.Util;
using Chameleon.lib.Common.Constants;
using Chameleon.client.MvvM;

using Chameleon.client.Features.Projects.Folders;
using Chameleon.client.Features.Projects.Profiles;
using Chameleon.client.Features.Projects.Profiles.Identity;
using Chameleon.client.Services;

namespace Chameleon.client.Features.Projects;

public abstract partial class Projector(string? title = null) : ViewModelObjectBase(title) {
	public ReadOnlyObservableCollection<ObsProfile> Profiles { get; protected set; } = new([]);
	public ReadOnlyObservableCollection<ObsFolder> Folders { get; protected set; } = new([]);
	public bool HasNoFolderItems => Folders.Count == 0;
}
public partial class ProjectsViewModel : ViewModelObjectBase {
	public ProfilesViewModel Profiles => ProfilesViewModel.Instance;
	public FoldersViewModel Folders => FoldersViewModel.Instance;

	public bool IsCreateProfileBtnVisible => Auther.AuthSession?.CreatorUserId == null || Auther.AuthSession?.CanCreateProfiles == true;

	public ProjectsViewModel() : base() {
		AsyncCommandMap["CreateProfile"] = async () => {
			try {
				var p = await Profiles.CreateNewProfile();
				Navigator.NavigateToType(typeof(IdentityView), p);
			} catch (Exception ex) {
				if (
					ex.Message == "limit_ex" &&
					await MessageBox.Show("PROFILES LIMIT REACHED", "You have reached the maximum number of profiles.")
				) ProUtil.GoToUrlDefault(Consts.PricingUrl);
				else throw;
			}
		};
	}
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
}
