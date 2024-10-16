using Chameleon.Interfaces.Auth;
using Chameleon.Interfaces.UserProfileFolders;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Chameleon.lib.CommunityToolkit.MvvM;
using Chameleon.lib.Api.Repos;
using Chameleon.app.Avalonia.Models.Observable;
using System.Collections.ObjectModel;
using DynamicData;
using Chameleon.app.Avalonia.Com.DynamicData;
using Chameleon.lib.Common.Models.Dto;
using Chameleon.lib.Common;
using Chameleon.Common.Helpers;

namespace Chameleon.app.Avalonia.ViewModels.Controllers;
public partial class UserProfileFoldersViewModel : ViewModelObjectBase {
	private readonly IAuthSession _authSession = ContainerServiceHelper.Resolve<IAuthSession>()!;

	private ObsFolder? _allProfiles;

	[ObservableProperty]
	private ObsFolder? selectedFolder;

	public ReadOnlyObservableCollection<ObsFolder> Folders { get; }

	private UserProfileFoldersViewModel()
	{
		_ = UserProfilesFolderRepo
		.Connect(i => i.isFavorite)
		.Transform(i => new ObsFolder(i))
		.SortAndBind(out var flist, Compares.FolderVimCompares.AscendingComparer)
		.Subscribe((i) => {
		});
		Folders = flist;
	}

	public ObsFolder AllProfiles {
		get {
			_allProfiles ??= new ObsFolder(new UPFolderDto() { title = "All profiles" }) { IsFavoriteButtonVisible = false };

			return _allProfiles;
		}
	}

	[RelayCommand]
	private async Task Create()
	{
		var folder = await UserProfilesFolderRepo.CreateFolder($"New Folder - {Folders.Count}");

		EventAggregator
				.GetEvent<AfterCreateOrRemoveFolderEvent>()
				.Publish();

		_ = OnNavigatingTo(folder);
	}


	public async Task OnNavigatingTo(UPFolderDto? p = null)
	{
		while (!Loaded)
			await Task.Delay(250);

		if (p != null) {
			foreach (var item in Folders)
				item.IsSelected = item.Dto!.id == p.id;

			var pvm = Folders.FirstOrDefault(vm => vm.Dto!.id == p.id);
			if (pvm != null) {
				IoC.GetService<UserProfilesViewModel>()?.Open(p);
			}
		} else {
			if (!AllProfiles.Navigated) {
				AllProfiles.Navigated = true;
				await AllProfiles.Open();
			}
		}
	}

	public async void SetSelectedById(int id)
	{
		while (!Loaded)
			await Task.Delay(250);

		await OnNavigatingTo(Folders.FirstOrDefault(m => m.Dto?.id == id)?.Dto);
	}

	public static UserProfileFoldersViewModel Instance { get; } = new UserProfileFoldersViewModel();
}
