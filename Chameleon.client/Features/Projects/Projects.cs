using Chameleon.client.Services;
using Chameleon.client.Features.Projects.Folders;
using Chameleon.client.Features.Projects.Profiles;
using Chameleon.client.Features.Projects.Profiles.Identity;

using Chameleon.lib.Util;
using Chameleon.lib.Api;
using Chameleon.lib.Helpers;
using Chameleon.client.Features.Automation;

namespace Chameleon.client.Features.Projects;

public partial class Projects : Automatior {
	public bool IsCreateProfileBtnVisible { get; } = Auther.AuthSession?.CreatorUserId == null || Auther.AuthSession?.CanCreateProfiles == true;

	Projects() {
		AsyncCommandMap["CreateProfile"] = async () => {
			try {
				var p = await ProfilesViewModel.Instance.CreateNewProfile();
				Navigator.NavigateToType(typeof(IdentityView), p);
			} catch (Exception ex) {
				if (
					ex.Message == "limit_ex" &&
					await MessageBox.Show("PROFILES LIMIT REACHED", "You have reached the maximum number of profiles.")
				) Processez.OpenBrowser("https://chameleonmode.com/pricing/");
				else throw;
			}
		};
	}
	public override async Task OnNavigatedTo(object? param) {
		await base.OnNavigatedTo(param);
		await FoldersViewModel.Instance.OnNavigatingTo(param as ObsFolder ?? FoldersViewModel.Instance.SelectedFolder);
		if (param is ObsProfile up) ProfilesViewModel.Instance.SearchText = up.Title ?? "";
		else if (param is string p) ProfilesViewModel.Instance.SearchText = p;
		
		ProfileUIContextManager.SetModuleContext(ProfileUIModule.Profiles, ProfileUIContext.Profiles);
		ProfilesViewModel.Instance.ApplyProfilesContext();
	}
	public static Projects I { get; } = new();
}