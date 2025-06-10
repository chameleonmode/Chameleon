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
using Chameleon.lib.Util;
using Chameleon.lib.Api.Repos;
using DynamicData;
using System.Reactive.Subjects;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Reactive.Linq;
using DynamicData.Binding;
using Chameleon.client.UI.Components.ViewModels;
using Microsoft.AspNetCore.SignalR;

namespace Chameleon.client.Features.Projects;

public abstract partial class Profiler : ViewModelObjectBase {
	public static SortExpressionComparer<ObsProfile> AscendingComparer => SortExpressionComparer<ObsProfile>.Ascending(p => p.Dto!.title!);
	public static SortExpressionComparer<ObsProfile> DescendingComparer => SortExpressionComparer<ObsProfile>.Descending(p => p.Dto!.title!);

	[ObservableProperty] UPFolderViewModel? folder;

	public readonly BehaviorSubject<Func<ObsProfile, bool>> filter;
	public readonly BehaviorSubject<IComparer<ObsProfile>> profilesCompareObservable;
	readonly BehaviorSubject<IPageRequest> pageRequests;

	public PaginatorViewModel PaginatorViewModel { get; }

	public IObservable<IChangeSet<ObsProfile, int>> Shared { get; }
	public ReadOnlyObservableCollection<ObsProfile> Profiles { get; }
	public ReadOnlyObservableCollection<ObsProfile> ObsProfiles { get; protected set; } = new([]);
	public ReadOnlyObservableCollection<ObsProfile> ObsProfilesFavorite { get; protected set; } = new([]);
	public bool HasNoItems => Profiles.Count == 0;
	public bool HasFaves => ObsProfilesFavorite.Count == 0;
	public int SelectedCount => GetSelectedProfiles?.Count() ?? 0;
	public bool HasSelectedItems => Profiles.Any(v => v.IsSelected);
	public IEnumerable<ObsProfile> GetSelectedProfiles => Profiles.Where(i => i.IsSelected);

	public Profiler(string? title = null) : base(title) {
		filter = new BehaviorSubject<Func<ObsProfile, bool>>(p => Folder == null || Folder.Id == 0 || p.Dto.folderId == Folder.Id);
		pageRequests = new(new PageRequest(0, 9));
		profilesCompareObservable = new(AscendingComparer);
		PaginatorViewModel = new PaginatorViewModel(p => pageRequests.OnNext(new PageRequest(p.CurrentIndex, p.OnPageItems))) {
			TotalCount = UserProfilesRepo.Instance.ObservableCache.Count,
		};
		// 1) Create a shared change‐set (after your Transform + Filter)
		Shared = UserProfilesRepo
		.Connect()
		.Transform(i => new ObsProfile(i,
				onSelectedChanged: p => SelectedChanged(p),
				onDeleted: p => Deleted(p))
		)
		.Publish()    // <-- multicast
		.RefCount();  // <-- auto-connect when first subscriber appears

		// 2) Your paged view
		_ = Shared
		.Filter(filter)
		.SortAndPage(AscendingComparer, pageRequests)
		.SortAndBind(out var pagedProfiles, profilesCompareObservable)
		.Subscribe();

		// 3) Your “full” (un-paged) view
		_ = Shared
		.Filter(filter)
		.SortAndBind(out var allProfiles, AscendingComparer)
		.Subscribe();
		
		// 4) Expose both lists
		Profiles = pagedProfiles;
		ObsProfiles = allProfiles;
	}
	public virtual ObsProfile SelectedChanged(ObsProfile profile) {
					OnPropertyChanged(nameof(HasSelectedItems));
					OnPropertyChanged(nameof(SelectedCount));
		return profile;
	}
	public virtual ObsProfile Deleted(ObsProfile profile) {
		return profile;
	}
}

public abstract partial class Projector(string? title = null) : ViewModelObjectBase(title) {
	public ReadOnlyObservableCollection<ObsProfile> Profiles { get; protected set; } = new([]);
	public ReadOnlyObservableCollection<ObsProfile> ObsProfiles { get; protected set; } = new([]);
	public ReadOnlyObservableCollection<ObsProfile> ObsProfilesFavorite { get; protected set; } = new([]);
	public ReadOnlyObservableCollection<ObsFolder> Folders { get; protected set; } = new([]);
	public bool HasNoFolderItems => Folders.Count == 0;
	public bool HasNoItems => Profiles.Count == 0;
	public bool HasFaves => ObsProfilesFavorite.Count == 0;
	// public bool IsProfilesExist => UserProfilesRepo.Instance.ObservableCache.Items.Any();
}
public partial class ViewModel : ViewModelObjectBase {
	public bool IsCreateProfileBtnVisible => Auther.AuthSession?.CreatorUserId == null || Auther.AuthSession?.CanCreateProfiles == true;

	ViewModel() {
		AsyncCommandMap["CreateProfile"] = async () => {
			try {
				var p = await ProfilesViewModel.Instance.CreateNewProfile();
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
			if (!folder.Navigated || FoldersViewModel.Instance.SelectedFolder?.Dto?.id == folder.Dto?.id) {
				await FoldersViewModel.Instance.OnNavigatingTo(folder.Dto);
				folder.Navigated = true;
			}
		} else if (param is ObsProfile up) {
			if (!up.Navigated) {
				ProfilesViewModel.Instance.OnFilterTo(up);
				up.Navigated = true;
			}
		} else {
			await FoldersViewModel.Instance.OnNavigatingTo(FoldersViewModel.Instance.SelectedFolder?.Dto);
			if (param is string p) ProfilesViewModel.Instance.SearchText = p;
		}
	}
	public static ViewModel Instance { get; } = new();
}
