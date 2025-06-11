using Chameleon.client.Features.Projects.Folders;
using Chameleon.client.Features.Projects.Profiles;
using Chameleon.lib.Api.Dto;
using Chameleon.lib.Api.Repos;
using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData;
using DynamicData.Binding;
using DynamicData.PLinq;
using System.Collections.ObjectModel;
using System.Reactive.Linq;

namespace Chameleon.client.Features.Dashboard.Tags;

public partial class TagsViewModel : Dashboarder {
	[ObservableProperty] string selectedTagName = "";
	[ObservableProperty] IEnumerable<string> folderTagIds = [];
	[ObservableProperty] IEnumerable<string> profileTagIds = [];
	public override ReadOnlyObservableCollection<ObsProfile> Profiles { get; }
	public override ReadOnlyObservableCollection<ObsFolder> Folders { get; }
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
		_ = ProfilesViewModel.Instance.Shared
					.Filter(
						this.WhenValueChanged(vm => vm.ProfileTagIds)
								.Where(ids => ids is not null)
								.Select(ids => new Func<ObsProfile, bool>(f => ids!.Any(id => id == f.Dto.id.ToString())))
					)
					.SortAndBind(out var profiles, profilesCompareObservable)
					.Transform(i => { i.IsShowCheckboxColumn = false; return i;})
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
