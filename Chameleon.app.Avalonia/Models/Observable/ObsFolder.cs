using Chameleon.app.Avalonia.ViewModels.Controllers;
using Chameleon.app.Avalonia.Views;
using Chameleon.lib.Api;
using Chameleon.lib.Api.Repos;
using Chameleon.lib.Common.Constants;
using Chameleon.lib.Common.Extensions;
using Chameleon.lib.Common.Models.Dto;
using Chameleon.lib.Common.ServiceManagers;
using Chameleon.lib.CommunityToolkit.MvvM;

using CommunityToolkit.Mvvm.ComponentModel;

namespace Chameleon.app.Avalonia.Models.Observable;
public partial class ObsFolder : Obs<UPFolderDto> {
	public event Action<ObsFolder>? OnSelectedChanged;

	[ObservableProperty]
	private bool isFavorite;
	[ObservableProperty]
	private int profilesCount;
	[ObservableProperty]
	private bool isRenamed;
	[ObservableProperty]
	private bool isActionOptionsVisible;

	public bool ShowFavoriteIcon => IsContextMenuItemEnabled;
	public bool IsSharedFolder => Dto?.creatorUserId != null &&  Dto?.creatorUserId != Auther.AuthSession?.UserId;
	public bool IsContextMenuItemEnabled => Auther.AuthSession?.CreatorUserId == null || Auther.AuthSession?.CreatorUserId == Dto?.creatorUserId;
	public bool IsContextMenuVisible => Dto!.id != 0;
	public bool IsFolderNotEmpty => UserProfilesRepo.Instance.ObservableCache.Items.Any(p => p.folderId == Dto!.id); 

	public ObsFolder(UPFolderDto folder) : base(folder.title)
	{
		Dto = folder;

		isFavorite = Dto.isFavorite;
		profilesCount = Dto.profilesCount;

		CommandMap["SetFavoriteFolder"] = SetFavoriteFolder;
		CommandMap["ViewGroup"] = ViewGroup;
		CommandMap["StartRename"] = StartRename;
		CommandMap["ChangeProxies"] = ChangeProxies;

		AsyncCommandMap["Open"] = Open;
		AsyncCommandMap["SetFavorite"] = SetFavorite;
		AsyncCommandMap["Delete"] = Delete;
		AsyncCommandMap["SaveRename"] = SaveRename;

		UserProfilesRepo.Instance.OnProfileChanged += (profile) => {
			if (profile.folderId == Dto!.id) {
				ProfilesCount = UserProfilesRepo.Instance.ObservableCache.Items.Count(p => p.folderId == Dto!.id);
			}
		};
	}
	public ObsFolder(
		UPFolderDto folder,
		bool hasActionOptions,
		Action<ObsFolder>? onSelectedChanged)
		: this(folder)
	{
		IsActionOptionsVisible = hasActionOptions;
		OnSelectedChanged = onSelectedChanged;
	}

	// Properties Changed Events
	public override void OnAnyIsSelectedChanged(bool value)
	{
		if (value == false) {
			IsRenamed = false;
		}

		OnSelectedChanged?.Invoke(this);
	}

	// CommandMap Commands
	private void ViewGroup()
	{
		Navigator.NavigateToType(typeof(ProjectsView), this);
	}
	private void SetFavoriteFolder()
	{
		IsFavorite = !IsFavorite;

		Dto!.isFavorite = IsFavorite;
		_ = UserProfilesFolderRepo.Instance.Put(Dto);
	}
	private void StartRename()
	{
		Title = Dto?.title;
		IsRenamed = true;
	}
	private void ChangeProxies()
	{
		Navigator.NavigateToType(typeof(FunctionalSettingsView), this);
	}

	// AsyncCommandMap Commands
	public async Task Open()
	{
		await UserProfileFoldersViewModel.Instance.OnNavigatingTo(Dto);
		IsSelected = true;
	}
	private async Task SetFavorite()
	{
		IsFavorite = !IsFavorite;
		Dto!.isFavorite = IsFavorite;

		_ = await UserProfilesFolderRepo.Instance.Put(Dto);

		OnPropertyChanged(nameof(Dto));
	}
	private async Task Delete()
	{
		if (await Mbox.Show("Delete Folder",
				$"Are you sure you want to delete {Dto!.title} folder? This will not affect individual profiles within the folder.",
				Enums.MBoxButtons.OkCancel,
				"DeleteLines")) {

			var userProfiles = UserProfilesRepo.Instance.ObservableCache.Items.Where(p=> p.folderId == Dto!.id);
			var deletes = new List<Task>();
			foreach (var item in userProfiles) {
				item.folderId = null;
				deletes.Add(UserProfilesRepo.Instance.Put(item));
			}
			await Task.WhenAll(deletes);
			var res = await UserProfilesFolderRepo.Instance.Delete(Dto!.id);
			if (!res.success) {

			}
			await UserProfileFoldersViewModel.Instance.AllProfiles!.Open();
		}
	}
	private async Task SaveRename()
	{
		if (!Title.Is()) {
			return;
		}

		var wasSelected = IsSelected;
		var orignalTitle = Dto!.title;
		try {
			Dto.title = Title;
			var res = await UserProfilesFolderRepo.Instance.Put(Dto);
			if (res != null) {
				IsRenamed = false;
			}
		} catch {
			Dto.title = orignalTitle;
		}

		Title = Dto.title;

		IsRenamed = false;

		if (wasSelected) {
			await Open();
		}
	}
}
