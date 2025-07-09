using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

using Chameleon.client.MvvM;
using Chameleon.client.Services;
using Chameleon.client.Features.Projects.Folders;
using Chameleon.client.Features.Projects.Profiles;
using Chameleon.client.Features.Projects.Profiles.Identity;

using Chameleon.lib;
using Chameleon.lib.Util;
using Chameleon.lib.Api;
using Chameleon.lib.Helpers;

namespace Chameleon.client.Features.Projects;

public abstract partial class Projector(string? title = null) : Profilearee(title) {
	[ObservableProperty] ChangeComparereOption sortFolder = ChangeComparereOption.Ascending;

	public abstract ReadOnlyObservableCollection<ObsFolder> Folders { get; }

	public bool HasFolders => Folders.Count > 0;
	public bool HasNoItems => !HasFolders && !HasProfiles;

	partial void OnSortFolderChanged(ChangeComparereOption value) {
		Folderer.CompareObservable.OnNext(value switch {
			ChangeComparereOption.Descending => Folderer.DescendingComparer,
			_ => Folderer.AscendingComparer
		});
	}
}

public partial class ViewModel : ViewModelObjectBase {
	public bool IsCreateProfileBtnVisible { get; } = Auther.AuthSession?.CreatorUserId == null || Auther.AuthSession?.CanCreateProfiles == true;

	ViewModel() : base("") {
		AsyncCommandMap["CreateProfile"] = async () => {
			try {
				var p = await ProfilesViewModel.Instance.CreateNewProfile();
				Navigator.NavigateToType(typeof(IdentityView), p);
			} catch (Exception ex) {
				if (
					ex.Message == "limit_ex" &&
					await MessageBox.Show("PROFILES LIMIT REACHED", "You have reached the maximum number of profiles.")
				) ProcessUtil.OpenBrowser("https://chameleonmode.com/pricing/");
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
	}
	public static ViewModel Instance { get; } = new();
}