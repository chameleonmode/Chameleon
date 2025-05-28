using Chameleon.app.Avalonia.Models.Observable;
using Chameleon.client.Features.Projects.Folders;
using Chameleon.lib.Api.Repos;
using Chameleon.lib.Common.Models.Dto;
using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData;
using DynamicData.Binding;
using System.Reactive.Linq;

namespace Chameleon.client.Features.Dashboard.Tags;

public partial class TagsViewModel : Base {
	[ObservableProperty] string selectedTagName = "";
	[ObservableProperty] IEnumerable<string> folderTagIds = [];
	[ObservableProperty] IEnumerable<string> profileTagIds = [];

	public TagsViewModel() : base("Tags") {
		var tagItems = TagsRepo.Connect()
			.Filter(tag => tag.Name == SelectedTagName);

		_ = this.WhenValueChanged(x => x.SelectedTagName)
			.Where(tagName => !string.IsNullOrEmpty(tagName))
			.SelectMany(_ => tagItems)
			.Subscribe(changeSet => {
				var items = changeSet
										.Select(change => change.Current)
										.SelectMany(x => x.Items);
										
				FolderTagIds = items
					.Where(x => x.Key == TagItemType.Folder)
					.SelectMany(x => x.Value).Distinct();
				ProfileTagIds = items
					.Where(x => x.Key == TagItemType.Profile)
					.SelectMany(x => x.Value).Distinct();
			});

		_ = UserProfilesRepo.Connect()
					.Filter(
						this.WhenValueChanged(vm => vm.ProfileTagIds)
								.Where(ids => ids is not null)
								.Select(ids => new Func<UserProfileDto, bool>(f => ids!.Any(id => id == f.id.ToString())))
					)
					.Transform(i => new ObsProfile(i, false))
					.SortAndBind(out var profiles, profilesCompareObservable)
					.Subscribe(_ => OnPropertyChanged(nameof(HasNoItems)));
		Profiles = profiles;

		_ = UserProfilesFolderRepo.Connect()
					.Filter(
						this.WhenValueChanged(vm => vm.FolderTagIds)
								.Where(ids => ids is not null)
								.Select(ids => new Func<UPFolderDto, bool>(f => ids!.Any(id => id == f.id.ToString())))
					)
					.Transform(i => new ObsFolder(i){ IsActionOptionsVisible = true})
					.SortAndBind(out var folders, foldersCompareObservable)
					.Subscribe(_ => OnPropertyChanged(nameof(HasNoFolderItems)));
		Folders = folders;
	}

	public static TagsViewModel Instance { get; } = new TagsViewModel();
}
