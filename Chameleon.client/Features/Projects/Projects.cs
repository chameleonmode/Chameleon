using System.Collections.ObjectModel;

using Chameleon.lib.Api;
using Chameleon.lib.Helpers;
using Chameleon.lib.Common.Util;
using Chameleon.client.MvvM;

using Chameleon.client.Features.Projects.Folders;
using Chameleon.client.Features.Projects.Profiles;
using Chameleon.client.Features.Projects.Profiles.Identity;
using Chameleon.client.Services;
using Chameleon.lib.Api.Repos;
using DynamicData;
using System.Reactive.Subjects; 
using System.Reactive.Linq;
using DynamicData.Binding;

using Chameleon.lib;
using CommunityToolkit.Mvvm.ComponentModel;
using Chameleon.client.Features.Automation;
using Chameleon.lib.Util;

namespace Chameleon.client.Features.Projects;
public abstract partial class Profiler : Profilearee {
	public static readonly SortExpressionComparer<ObsProfile> AscendingComparer = SortExpressionComparer<ObsProfile>.Ascending(p => p.Dto.title ?? "");
	public static readonly SortExpressionComparer<ObsProfile> DescendingComparer = SortExpressionComparer<ObsProfile>.Descending(p => p.Dto.title ?? "");
	public static readonly BehaviorSubject<IComparer<ObsProfile>> CompareObservable = new(AscendingComparer);
	public IObservable<IChangeSet<ObsProfile, int>> Shared { get; }
	public ReadOnlyObservableCollection<ObsProfile> ObsProfiles { get; }
	public IEnumerable<ObsProfile> SelectedProfiles => Profiles.Where(i => i.IsSelected);
	public int SelectedCount => SelectedProfiles.Count();
	public bool HasSelectedItems => SelectedProfiles.Any();

	public Profiler(string? title = null) : base(title) {
		// 1) Create a shared change‐set (after your Transform + Filter)
		Shared = UserProfilesRepo.Connect().Transform(i => new ObsProfile(i,
			selectedChanged: SelectedChanged,
			onDeleted: p => Deleted(p)))
		.Publish()    // <-- multicast
		.RefCount();  // <-- auto-connect when first subscriber appears

		// 3) Your “full” (un-paged) view
		_ = Shared
		.SortAndBind(out var allProfiles, AscendingComparer)
		.Subscribe();
		
		// 4) Expose both lists
		ObsProfiles = allProfiles;
	}
	public virtual void SelectedChanged(ObsProfile profile) {
		OnPropertyChanged(nameof(HasSelectedItems));
		OnPropertyChanged(nameof(SelectedCount));
	}
	public virtual ObsProfile Deleted(ObsProfile profile) {
		return profile;
	}
}
public abstract partial class Folderer : ViewModelObjectBase {
	public static SortExpressionComparer<ObsFolder> AscendingComparer => SortExpressionComparer<ObsFolder>.Ascending(p => p.Dto!.title!);
	public static SortExpressionComparer<ObsFolder> DescendingComparer => SortExpressionComparer<ObsFolder>.Descending(p => p.Dto!.title!);
	public static readonly BehaviorSubject<IComparer<ObsFolder>> CompareObservable = new(AscendingComparer);
	public IObservable<IChangeSet<ObsFolder, int>> Shared { get; }
	public virtual ReadOnlyObservableCollection<ObsFolder> Folders { get; }
	public bool HasNoItems => Folders.Count == 0;
	public int SelectedCount => Folders.Where(i => i.IsSelected)?.Count() ?? 0;
	public bool HasSelectedItems => SelectedCount > 0;

	public Folderer(string? title = null) : base(title) {
		Shared = UserProfilesFolderRepo.Connect()
		.Transform(i => {
			i.title ??= "All";
			return new ObsFolder(folder: i, selectedChanged: SelectedChanged);
		})
		.Publish()    // <-- multicast
		.RefCount();  // <-- auto-connect when first subscriber appears

		_ = Shared.SortAndBind(out var folders, AscendingComparer).Subscribe();
		Folders = folders;
	}
	public virtual void SelectedChanged(ObsFolder folder) {
		OnPropertyChanged(nameof(HasSelectedItems));
		OnPropertyChanged(nameof(SelectedCount));
	}
	public virtual ObsFolder Deleted(ObsFolder profile) {
		return profile;
	}
}
public abstract partial class Profilearee(string? title = null) : Automatior(title) {
	public ChangeComparereOption[] Sorts { get; } = (ChangeComparereOption[])Enum.GetValues(typeof(ChangeComparereOption));
	[ObservableProperty] ChangeComparereOption sort = ChangeComparereOption.Ascending;
	public abstract ReadOnlyObservableCollection<ObsProfile> Profiles { get; }
	public bool HasProfiles => Profiles.Count > 0;

	partial void OnSortChanged(ChangeComparereOption value) {
		Profiler.CompareObservable.OnNext(value switch {
			ChangeComparereOption.Descending => Profiler.DescendingComparer,
			_ =>  Profiler.AscendingComparer
		});
	}
}
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
				) ProcessUtil.OpenBrowser(Const.PricingUrl);
				else throw;
			}
		};
	}
	public override async Task OnNavigatedToAsync(object? param) {
		await base.OnNavigatedToAsync(param);
		await FoldersViewModel.Instance.OnNavigatingTo(param as ObsFolder ?? FoldersViewModel.Instance.SelectedFolder);
		if (param is ObsProfile up) ProfilesViewModel.Instance.SearchText = up.Title ?? "";
		else if (param is string p) ProfilesViewModel.Instance.SearchText = p;
		
		// State machine context is already applied in ProfilesViewModel reactive pipeline
		// No need to apply it here to avoid race conditions
	}
	public static ViewModel Instance { get; } = new();
}
