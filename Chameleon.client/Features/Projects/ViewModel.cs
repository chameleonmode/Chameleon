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
using static Chameleon.lib.Common.Constants.Enums;
using Chameleon.lib;

namespace Chameleon.client.Features.Projects;
public abstract partial class Profiler : ViewModelObjectBase {
	public static SortExpressionComparer<ObsProfile> AscendingComparer => SortExpressionComparer<ObsProfile>.Ascending(p => p.Dto!.title!);
	public static SortExpressionComparer<ObsProfile> DescendingComparer => SortExpressionComparer<ObsProfile>.Descending(p => p.Dto!.title!);
	public readonly BehaviorSubject<IComparer<ObsProfile>> CompareObservable = new(AscendingComparer);

	public IObservable<IChangeSet<ObsProfile, int>> Shared { get; }
	public ReadOnlyObservableCollection<ObsProfile> ObsProfiles { get; }
	public abstract ReadOnlyObservableCollection<ObsProfile> Profiles { get; }
	public bool HasNoItems => Profiles.Count == 0;
	public int SelectedCount => GetSelectedProfiles?.Count() ?? 0;
	public bool HasSelectedItems => Profiles.Any(v => v.IsSelected);
	public IEnumerable<ObsProfile> GetSelectedProfiles => Profiles.Where(i => i.IsSelected);

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
	public IObservable<IChangeSet<ObsFolder, int>> Shared { get; }
	public virtual ReadOnlyObservableCollection<ObsFolder> Folders { get; }
	public bool HasNoItems => Folders.Count == 0;
	public int SelectedCount => Folders.Where(i => i.IsSelected)?.Count() ?? 0;
	public bool HasSelectedItems => SelectedCount > 0;

	public Folderer(string? title = null) : base(title) {
		Shared = UserProfilesFolderRepo.Connect()
		.Transform(i => {
			i.title ??= "All";
			return new ObsFolder(folder: i, onSelectedChanged: SelectedChanged);
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

public partial class ViewModel : ViewModelObjectBase {
	public bool IsCreateProfileBtnVisible { get; } = Auther.AuthSession?.CreatorUserId == null || Auther.AuthSession?.CanCreateProfiles == true;

	ViewModel(): base("") {
		AsyncCommandMap["CreateProfile"] = async () => {
			try {
				var p = await ProfilesViewModel.Instance.CreateNewProfile();
				Navigator.NavigateToType(typeof(IdentityView), p);
			} catch (Exception ex) {
				if (
					ex.Message == "limit_ex" &&
					await MessageBox.Show("PROFILES LIMIT REACHED", "You have reached the maximum number of profiles.")
				) ProUtil.GoToUrlDefault(Const.PricingUrl);
				else throw;
			}
		};
	}
	public override async Task OnNavigatedToAsync(object? param) {
		await base.OnNavigatedToAsync(param);
		if (param is ObsFolder folder) await FoldersViewModel.Instance.OnNavigatingTo(folder);
		else if (param is ObsProfile up) ProfilesViewModel.Instance.OnFilterTo(up);
		else {
			await FoldersViewModel.Instance.OnNavigatingTo(FoldersViewModel.Instance.SelectedFolder);
			if (param is string p) ProfilesViewModel.Instance.SearchText = p;
		}
		ProfilesViewModel.Instance.ObsProfiles.ForEach(p => p.IsActionOptionsVisible = p.IsShowCheckboxColumn = true);
	}
	public static ViewModel Instance { get; } = new();
}
