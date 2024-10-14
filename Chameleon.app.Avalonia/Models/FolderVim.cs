using Chameleon.Interfaces.App.UserProfileFolders.Events;
using Chameleon.Interfaces.App.UserProfiles;
using Chameleon.Interfaces.UserProfileFolders;
using Chameleon.Interfaces.UserProfiles;
using Chameleon.lib.Common.ServiceManagers;
using Chameleon.lib.CommunityToolkit.MvvM;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Chameleon.app.Avalonia.Models;
public partial class FolderVim : ViewModelObjectBase {
	private readonly IUserProfileService _userProfileService;
	private readonly IUserProfileFolderService _userProfileFolderService;

	[ObservableProperty]
	private bool isFavorite;
	[ObservableProperty]
	private int profilesCount;
	public IUserProfileFolder Folder { get; private set; }

	public FolderVim(IUserProfileFolder folder, IUserProfileService userProfileService, IUserProfileFolderService userProfileFolderService)
			: base(folder.Title)
	{
		_userProfileService = userProfileService;
		_userProfileFolderService = userProfileFolderService;

		this.Folder = folder;
		isFavorite = Folder.IsFavorite;
		profilesCount = Folder.ProfilesCount;

		_ = EventAggregator
				.GetEvent<ChangeProfilesInFavoriteFolderEvent>()
				.Subscribe(args => OnChangeProfilesInFavoriteFolder(args.FolderId));

		CommandMap["SetFavoriteFolder"] = SetFavoriteFolder;
		CommandMap["ViewGroup"] = ViewGroup;
	}

	private void OnChangeProfilesInFavoriteFolder(int folderId)
	{
		if (Folder.Id != folderId) {
			return;
		}
		Folder = _userProfileFolderService.Get(folderId);
		ProfilesCount = _userProfileService.GetAll().Count(a => a.FolderId == folderId);
		IsFavorite = Folder.IsFavorite;
		OnPropertyChanged(nameof(ProfilesCount));
	}

	private void ViewGroup()
	{
		Folder.Navigated = false;
		Navigator.NavigateToType(typeof(IProjectsView), Folder);
	}

	private void SetFavoriteFolder()
	{
		IsFavorite = !IsFavorite;

		Folder.IsFavorite = IsFavorite;
		_userProfileFolderService.Save(Folder);

		EventAggregator.GetEvent<UpdateFavoriteFolderEvent>().Publish();
		EventAggregator.GetEvent<UpdateUserProfileFolderEvent>().Publish(new UserProfileFolderEventArgs(Folder));
	}
}
