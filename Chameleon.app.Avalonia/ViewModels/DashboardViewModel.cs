using System.ComponentModel;
using Chameleon.Interfaces.App.Synchronization.Events;
using Chameleon.Interfaces.Auth;
using Chameleon.Interfaces.UserProfileFolders;
using Chameleon.Interfaces.UserProfiles;
using CommunityToolkit.Mvvm.ComponentModel;

using CommunityToolkit.Mvvm.Input;
using Chameleon.lib.CommunityToolkit.MvvM;
using Chameleon.Common.Helpers;
using Chameleon.Avalonia.Controls.UserProfilesView.ViewModels;
using DynamicData.Binding;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using DynamicData;
using System;
using Microsoft.VisualBasic;
using AutoMapper;
using Chameleon.Domain.Entities;
using Chameleon.Interfaces;
using Chameleon.app.Avalonia.Models;

namespace Chameleon.app.Avalonia.ViewModels;
public partial class DashboardViewModel
			 : ViewModelObjectBase {

	private readonly IUserProfileService _userProfileService;
	private readonly IUserProfileFolderService _userProfileFolderService;
	private readonly IApplicationUser _applicationUser;

	[ObservableProperty]
	private bool isSyncChangesBtnVisible = true;
	[ObservableProperty]
	private ListSortDirection sortSelected = ListSortDirection.Ascending;
	[ObservableProperty]
	private ListSortDirection folderSortSelected = ListSortDirection.Ascending;

	public ListSortDirection[] Sorts { get; } = (ListSortDirection[])Enum.GetValues(typeof(ListSortDirection));

	public Predicate<object> ProfilesFilter => (obj) => obj is UserProfileViewModel fii && fii.UserProfile.IsFavourite;
	public Task<List<UserProfileViewModel>> Profiles => GetProfiles();
	private async Task<List<UserProfileViewModel>> GetProfiles()
	{
		var userProfiles = await _userProfileService.GetAllAsync();
		var profiles = new List<UserProfileViewModel>();
		List<MainAppSearchItem> items = [];
		foreach (var profile in userProfiles) {
			var vm = new UserProfileViewModel(_userProfileService, (profile as UserProfile)!, _applicationUser, false);
			profiles.Add(vm);
			items.Add(new() {
				Header = vm.Title,
				Namespace = "Profile",
				ViewModel = vm.UserProfile,
				PageType = this.GetType()
			});
		}
		ContainerServiceHelper.Resolve<IMainViewViewModel>()?.BuildSearchTerms(items);
		return profiles;
	}

	public Predicate<object> FoldersFilter => (obj) => obj is FolderVim fii && fii.Folder.IsFavorite;
	public Task<List<FolderVim>> Folders => GetFolders();
	private async Task<List<FolderVim>> GetFolders()
	{
		var all = await _userProfileFolderService.GetAllAsync();
		var folders = new List<FolderVim>();
		List<MainAppSearchItem> items = [];
		foreach (var folder in all) {
			var vm = new FolderVim(folder, _userProfileService, _userProfileFolderService);
			folders.Add(vm);
			items.Add(new() {
				Header = vm.Title ?? "xxx",
				Namespace = "Profiles " + vm.ProfilesCount,
				ViewModel = vm.Folder,
				PageType = this.GetType()
			});
		}
		ContainerServiceHelper.Resolve<IMainViewViewModel>()?.BuildSearchTerms(items);
		return folders;
	}

	//[ObservableProperty]
	//private readonly ObservableCollectionExtended<UserProfileViewModel> profiles = [];

	public DashboardViewModel() 
		: base("Dashboard")
	{
		//TODO: change
		_userProfileService = ContainerServiceHelper.Resolve<IUserProfileService>()!;
		_userProfileFolderService = ContainerServiceHelper.Resolve<IUserProfileFolderService>()!;
		_applicationUser = ContainerServiceHelper.Resolve<IApplicationUser>()!;
		//var myObservableChangeSet = _userProfileService.Cache.Connect().Filter(p=>p.IsFavourite);
		////Dynamic data is an extension of reactive so subscribe at the end of the chain
		//var loader = _userProfileService.Cache.Connect()
		//	.Transform(profile => new UserProfileViewModel(_userProfileService, (profile as UserProfile)!, _applicationUser, false))
		//.Bind(profiles)
		//.Subscribe();

		//_systemBrowserManager = systemBrowserManager;

		//EventAggregator
		//	 .GetEvent<DeleteUserProfileEvent>()
		//	 .Subscribe(OnUpdateViewModel);

		//EventAggregator
		//		.GetEvent<FavoriteUserProfileEvent>()
		//		.Subscribe(OnUpdateViewModel);

		//EventAggregator
		//		.GetEvent<UnfavoriteUserProfileEvent>()
		//		.Subscribe(OnUpdateViewModel);

		//EventAggregator
		//		.GetEvent<SavedUserProfileEvent>()
		//		.Subscribe(OnUpdateViewModel);

		//EventAggregator
		//		.GetEvent<UpdateFavoriteFolderEvent>()
		//		.Subscribe(()=> OnPropertyChanged(nameof(Folders)));
	}
	public override async Task InitAsync(object? param)
	{
		if (!Loaded) {
			await base.InitAsync(param);
		}

		OnPropertyChanged(nameof(ProfilesFilter));
		OnPropertyChanged(nameof(FoldersFilter));
	}

	partial void OnFolderSortSelectedChanged(ListSortDirection value)
	{
	}
	partial void OnSortSelectedChanged(ListSortDirection value)
	{

	}

	//public ListSortDirection SortSelected {
	//	get => _sortSelected;
	//	set {
	//		if (SetProperty(ref _sortSelected, value)) {
	//			ViewModels.Ascending = value == ListSortDirection.Ascending;
	//		}
	//	}
	//}

	//public ListSortDirection FolderSortSelected {
	//	get => _folderSortSelected;
	//	set {
	//		if (SetProperty(ref _folderSortSelected, value)) {
	//			FolderViewModels.Ascending = value == ListSortDirection.Ascending;
	//		}
	//	}
	//}

	[RelayCommand]
	private void SyncChanges()
	{
		EventAggregator
				.GetEvent<SyncChangesEvent>()
				.Publish();
	}
}

