using Chameleon.app.Avalonia.Models.Observable;
using Chameleon.client.Features.ProfilesAndFolders.Profiles.MyProfiles;
using Chameleon.client.Features.Projects.Folders;
using Chameleon.client.Features.Projects.Profiles.MyProfiles;
using Chameleon.lib.CommunityToolkit.MvvM;
using Chameleon.lib.Helpers;
using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData;
using DynamicData.Binding;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reactive.Linq;

namespace Chameleon.client.Features.Automation.Actors.Dialogs;

public partial class ProfileOrFolderItem: ObservableObject, IDisposable {

	[ObservableProperty] bool isSelected = false;
	[ObservableProperty] bool isVisible = true;
	public object Item { get; } = null!;
	public ObsFolder? Folder => Item as ObsFolder;
	public ObsProfile? Profile => Item as ObsProfile;
	public bool IsFolder => Folder != null;
	public bool IsProfile => Profile != null;
	public string DisplayName => Folder?.Title ?? Profile?.Title ?? "Unknown";

	public long FolderIdKey => Folder?.Dto?.id ?? Profile?.Dto?.folderId ?? 0;

	public ProfileOrFolderItem(object item, bool initialIsSelected = false) {
		isSelected = (Profile?.IsSelected ?? false) || initialIsSelected;

		if (item is ObsProfile profile) {
			Item = new ObsProfile(profile.Dto, isShowCheckboxColumn: false, isShowGlyph: true, hasActionOptions: false);
		}

		if (item is ObsFolder folder) {
			Item = new ObsFolder(folder.Dto);
		}

		if (Profile != null) {
			Profile.PropertyChanged += OriginalProfile_PropertyChanged;
		}
	}

	private void OriginalProfile_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
		if (e.PropertyName == nameof(ObsProfile.IsSelected) && Profile != null) {
			IsSelected = Profile.IsSelected;
		}
	}

	partial void OnIsSelectedChanged(bool value) {
		if (Profile != null && Profile.IsSelected != value) {
			Profile.IsSelected = value;
		}
	}

	public void Dispose() {
		if (Profile != null) {
			Profile.PropertyChanged -= OriginalProfile_PropertyChanged;
		}
	}
}

public partial class GroupedProfiles : ObservableObject {
	public long Key { get; }
	public string Title { get; }

	public ObservableCollection<ProfileOrFolderItem> ProfileItems { get; } = [];

	[ObservableProperty]
	private bool? isGroupSelected;

	private bool isUpdatingChildSelections = false;
	private bool isUpdatingGroupSelection = false;

	public GroupedProfiles(long key, string title, IEnumerable<ProfileOrFolderItem> profileItems) {
		Key = key;
		Title = title;

		foreach (var item in profileItems) {
			if (item.IsProfile)
			{
				item.PropertyChanged += ProfileItem_PropertyChanged;
				ProfileItems.Add(item);
			}
		}
		UpdateGroupSelectionState();
	}

	partial void OnIsGroupSelectedChanged(bool? value) {
		if (isUpdatingGroupSelection || value == null) 
			return;

		isUpdatingChildSelections = true;
		var newSelectionState = value.Value;
		foreach (var item in ProfileItems) {
			item.IsSelected = newSelectionState;
		}
		isUpdatingChildSelections = false;
	}

	private void ProfileItem_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
		if (e.PropertyName == nameof(ProfileOrFolderItem.IsSelected) && !isUpdatingChildSelections) {
			UpdateGroupSelectionState();
		}
	}

	public void UpdateGroupSelectionState() {
		isUpdatingGroupSelection = true;
		if (!ProfileItems.Any()) {
			SetIsGroupSelected(false);
			isUpdatingGroupSelection = false;
			return;
		}

		var allSelected = ProfileItems.All(p => p.IsSelected);
		var noneSelected = ProfileItems.All(p => !p.IsSelected);

		if (allSelected) {
			SetIsGroupSelected(true);
		} else if (noneSelected) {
			SetIsGroupSelected(false);
		} else
			{
			SetIsGroupSelected(null);
		}
		isUpdatingGroupSelection = false;
	}

	private void SetIsGroupSelected(bool? value) {
		IsGroupSelected = value;
	}

	public void Cleanup() {
		foreach (var item in ProfileItems) {
			item.PropertyChanged -= ProfileItem_PropertyChanged;
			item.Dispose();
		}
	}
}

public partial class ProfileSelectorViewModel : ViewModelObjectBase, IDisposable {
	private readonly IDisposable filterSubscription;
	private readonly HashSet<string?> initiallySelectedProfileIds = [];

	[ObservableProperty] string? searchText;

	[ObservableProperty] ObservableCollection<GroupedProfiles> displayGroups = [];

	public IEnumerable<ObsProfile> SelectedProfiles {
		get {
			var selectedOriginalProfiles = new List<ObsProfile>();
			foreach (var group in DisplayGroups) {
				foreach (var pfi in group.ProfileItems) {
					if (pfi.IsSelected && pfi.Profile?.Dto?.id != null) {
						selectedOriginalProfiles.Add(pfi.Profile);
					}
				}
			}
			return selectedOriginalProfiles.Distinct();
		}
	}

	public ProfileSelectorViewModel(IEnumerable<ObsProfile>? initiallySelectedProfiles) : base("Select Profiles") {

		if (initiallySelectedProfiles != null) {
			foreach (var p in initiallySelectedProfiles) {
				if (p.Dto?.id != null)
					_ = initiallySelectedProfileIds.Add(p.Dto.id.ToString());
			}
		}

		RebuildAndFilterDisplayGroups(searchText);

		filterSubscription = this.WhenValueChanged(x => x.SearchText)
															.Skip(1)
															.DistinctUntilChanged()
															.Throttle(TimeSpan.FromMilliseconds(300))
															.Subscribe(RebuildAndFilterDisplayGroups);
	}

	private void RebuildAndFilterDisplayGroups(string? searchText) {

		var filteredSourceProfiles = string.IsNullOrWhiteSpace(searchText)
				? [..  ProfilesViewModel.Instance.Profiles]
				:  ProfilesViewModel.Instance.Profiles.Where(p => p.Title?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false);

		var profileWrappers = filteredSourceProfiles
		.Select(p => new ProfileOrFolderItem(p, initiallySelectedProfileIds.Contains(p.Dto?.id.ToString()) || p.IsSelected));

		var groupedProfileWrappers = profileWrappers.GroupBy(pfi => pfi.FolderIdKey);

		var distinctFolders = FoldersViewModel.Instance.Folders
		.Where(f => f.Dto != null)
		.GroupBy(f => f.Dto!.id)
		.Select(g => g.First())
		.OrderBy(f => f.Title);

		var resultGroup = new List<GroupedProfiles>();

		foreach (var folder in distinctFolders) {
			var currentFolderId = folder.Dto!.id;
			var folderTitleMatchesSearch = !string.IsNullOrWhiteSpace(searchText) &&
																			(folder.Title?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false);

			var wrappersInThisGroup = groupedProfileWrappers.FirstOrDefault(g => g.Key == currentFolderId);

			if (folderTitleMatchesSearch || (wrappersInThisGroup != null && wrappersInThisGroup.Any())) {
				var groupVM = new GroupedProfiles(
						currentFolderId,
						folder.Title ?? "Unnamed Folder",
						wrappersInThisGroup ?? Enumerable.Empty<ProfileOrFolderItem>()
				);
				resultGroup.Add(groupVM);
			}
		}
		foreach (var group in resultGroup) {
			group.UpdateGroupSelectionState();
		}

		DisplayGroups = new ObservableCollection<GroupedProfiles>(resultGroup.OrderBy(g => g.Title));
	}

	public async Task<bool> ShowDialogAsync() {
		var result = await MessageBox.ShowTaskDialog<ProfileSelectorView, ProfileSelectorViewModel>(new(
				Initialize: () => this,
				Header: "Select Profiles",
				SubHeader: "Select profiles to run the actor automation with. You can search and expand folders.",
				Symbas: Symbas.People,
				Btns: MBoxButtons.OkCancel)
		);

		if (result == TaskDialogResult.OK) {
			var dialogSelectedProfileIds = new HashSet<string?>();
			foreach (var group in DisplayGroups) {
				foreach (var pfi in group.ProfileItems) {
					if (pfi.IsSelected && pfi.Profile?.Dto?.id != null) {
						_ = dialogSelectedProfileIds.Add(pfi.Profile.Dto.id.ToString());
					}
				}
			}

			foreach (var originalProfile in ProfilesViewModel.Instance.Profiles) {
				if (originalProfile.Dto?.id != null) {
					originalProfile.IsSelected = dialogSelectedProfileIds.Contains(originalProfile.Dto.id.ToString());
				}
			}
		}

		Dispose();

		return result == TaskDialogResult.OK && SelectedProfiles.Any();
	}

	public void Dispose() {
		filterSubscription?.Dispose();
		foreach (var group in DisplayGroups) {
			group.Cleanup();
		}
	}
}