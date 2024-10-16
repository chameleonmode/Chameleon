using Chameleon.app.Avalonia.Models.Observable;
using Chameleon.app.Avalonia.ViewModels.Controllers;
using Chameleon.app.Avalonia.Views;
using Chameleon.Common.Helpers;
using Chameleon.Core.Settings;
using Chameleon.Interfaces.Auth;
using Chameleon.lib.Common.ServiceManagers;
using Chameleon.lib.Common.Util;
using Chameleon.lib.CommunityToolkit.MvvM;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Chameleon.app.Avalonia.ViewModels;
public partial class ProjectsViewModel : ViewModelObjectBase {
	private readonly IAuthSession _authSession = ContainerServiceHelper.Resolve<IAuthSession>()!;

	private int sIListView = 1;

	[ObservableProperty]
	private bool listViewVisible = true;
	public bool IsCreateProfileBtnVisible => _authSession.CanCreateProfiles;

	public ProjectsViewModel()
		: base("Profiles") 
	{
	}
	public int SIListView {
		get { return sIListView; }
		set {
			if (SetProperty(ref sIListView, value)) {
				switch (value) {
					case 0:
						ListViewVisible = false;
						break;

					case 1:
						ListViewVisible = true;
						break;

					default:
						break;
				}
			}
		}
	}

	public override async Task OnNavigatedToAsync(object? param)
	{
		await base.OnNavigatedToAsync(param);

		if (param is ObsFolder folder) {
			////TODO: wtf
			//await Task.Delay(500);
			//EventAggregator
			//    .GetEvent<OpenUserProfileFolderEvent>()
			//    .Publish(new UserProfileFolderEventArgs(folder));

			if (!folder.Navigated || UserProfileFoldersViewModel.Instance.SelectedFolder?.Dto?.id == folder.Dto?.id) {
				await UserProfileFoldersViewModel.Instance.OnNavigatingTo(folder.Dto);
				folder.Navigated = true;
			}
		} else if (param is ObsProfile up) {
			if (!up.Navigated) {
				UserProfilesViewModel.Instance.OnFilterTo(up);
				up.Navigated = true;
			}
		} else {
			if (UserProfileFoldersViewModel.Instance.SelectedFolder != null)
				await UserProfileFoldersViewModel.Instance.OnNavigatingTo(UserProfileFoldersViewModel.Instance.SelectedFolder.Dto);
			else
				await UserProfileFoldersViewModel.Instance.OnNavigatingTo(null);

			if (param is string p)
				UserProfilesViewModel.Instance.SearchText = p;
		}
	}

	public override Task InitAsync(object? param)
	{
		return base.InitAsync(param);
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
			var p = await UserProfilesViewModel.Instance.CreateNewProfile();
			//profiles.Filter = profile => p.Id == profile.Id;
			Navigator.NavigateToType(typeof(UserProfileIdentityView), p);

			//EventAggregator.Push<ChangeProfilesInFavoriteFolderEvent, ChangeProfilesInFavoriteFolderEventArgs>(new ChangeProfilesInFavoriteFolderEventArgs(p.FolderId ?? 0, false, p));
		} catch (Exception ex) {
			if (ex.Message == "limit_ex") {
				if (await Mbox.Show("PROFILES LIMIT REACHED", "You have reached the maximum number of profiles."))
					ProUtil.GoToUrlDefault(GlobalSettings.PricingUrl);
			} else {
				Toaster.ShowErr("Wooopsy?", ex.Message);
			}
		} finally {
			//profiles.Filter = filter;
			IsDisabledCreateNewProfile = false;
		}
	}
}
