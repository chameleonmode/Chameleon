using Chameleon.app.Avalonia.Models.Observable;
using Chameleon.lib.Api.Repos;
using Chameleon.lib.Common.Models.Dto;
using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData;
using DynamicData.Binding;
using System.Reactive.Linq;

namespace Chameleon.app.Avalonia.Features.Dashboard.Tags;

public partial class TagsViewModel : DashboardItemsViewModelBase {
	[ObservableProperty] string selectedTagName = "";
	[ObservableProperty] IEnumerable<string> folderTagIds = [];
	[ObservableProperty] IEnumerable<string> profileTagIds = [];

	public TagsViewModel() : base("Tags") {
		var tagItems = TagsRepo
			.Connect()
			.Filter(tag => tag.Name == SelectedTagName);

		_ = this.WhenValueChanged(x => x.SelectedTagName)
			.Where(tagName => !string.IsNullOrEmpty(tagName))
			.SelectMany(_ => tagItems)
			.Subscribe(RefreshProfilesAndFolders);

		var profileTagIdsChangedFilter = this
			.WhenValueChanged(vm => vm.ProfileTagIds)
			.Where(ids => ids is not null)
			.Select(ids => new Func<UserProfileDto, bool>(f => ids!.Any(id => id == f.id.ToString())));

		var profiles = UserProfilesRepo
					.Connect()
					.Filter(profileTagIdsChangedFilter)
					.Transform(i => new ObsProfile(i, false))
					.SortAndBind(out var profileList, profilesCompareObservable)
					.Subscribe(_ => OnPropertyChanged(nameof(HasNoItems)));
		Profiles = profileList;

		var folderTagIdsChangedFilter = this
					.WhenValueChanged(vm => vm.FolderTagIds)
					.Where(ids => ids is not null)
					.Select(ids => new Func<UPFolderDto, bool>(f => ids!.Any(id => id == f.id.ToString())));

		var folders = UserProfilesFolderRepo
					.Connect()
					.Filter(folderTagIdsChangedFilter)
					.Transform(i => new ObsFolder(i, true, null, null))
					.SortAndBind(out var folderlist, foldersCompareObservable)
					.Subscribe(_ => OnPropertyChanged(nameof(HasNoFolderItems)));
		Folders = folderlist;
	}

	void RefreshProfilesAndFolders(IChangeSet<TagDto, string> changeSet) {
		var items = changeSet.Select(change => change.Current)
								.SelectMany(x => x.Items)
								.ToList();
		FolderTagIds = items
			.Where(x => x.Key == TagItemType.Folder)
			.SelectMany(x => x.Value).Distinct();
		ProfileTagIds = items
			.Where(x => x.Key == TagItemType.Profile)
			.SelectMany(x => x.Value).Distinct();
	}

	public static TagsViewModel Instance { get; } = new TagsViewModel();
}
