using Chameleon.app.Avalonia.Models.Observable;
using Chameleon.lib.Common.Constants;
using Chameleon.lib.Common.ServiceManagers;
using Chameleon.lib.CommunityToolkit.MvvM;
using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData;
using DynamicData.Binding;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reactive.Linq;

namespace Chameleon.client.Features.Automation.Actors.Dialogs;

public partial class ProfileOrFolderItem: ObservableObject, IDisposable {

	[ObservableProperty] bool isSelected = false;

	public object OriginalItem { get; }
	public object Item { get; } = null!;
	public ObsFolder? Folder => Item as ObsFolder;
	public ObsProfile? Profile => Item as ObsProfile;
	public bool IsFolder => Folder != null;
	public bool IsProfile => Profile != null;
	public string DisplayName => Folder?.Title ?? Profile?.Title ?? "Unknown";

	public long FolderIdKey => Folder?.Dto?.id ?? Profile?.Dto?.folderId ?? 0;

	public ProfileOrFolderItem(object item, bool initialIsSelected = false) {
		OriginalItem = item ?? throw new ArgumentNullException(nameof(item));
		isSelected = (Profile?.IsSelected ?? false) || initialIsSelected;

		if (item is ObsProfile profile) {
			Item = new ObsProfile(profile.Dto, isShowCheckboxColumn: false, isShowGlyph: true, hasActionOptions: false);
		}

		if (item is ObsFolder folder) {
			Item = new ObsFolder(folder.Dto, hasActionOptions: false, onSelectedChanged: null, nameAlreadyExist: null);
		}


		if (Profile != null) {
			Profile.PropertyChanged += OriginalProfile_PropertyChanged;
		}
	}

	private void OriginalProfile_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
		if (e.PropertyName == nameof(ObsProfile.IsSelected) && Profile != null) {
			SetProperty(ref isSelected, Profile.IsSelected, nameof(IsSelected));
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
		SetProperty(ref isGroupSelected, value, nameof(IsGroupSelected));
	}

	public void Cleanup() {
		foreach (var item in ProfileItems) {
			item.PropertyChanged -= ProfileItem_PropertyChanged;
			item.Dispose();
		}
	}
}

public partial class ProfileSelectorViewModel : ViewModelObjectBase, IDisposable {
	private readonly ReadOnlyObservableCollection<ObsFolder> allFolders;
	private readonly ReadOnlyObservableCollection<ObsProfile> allProfiles;
	private readonly IDisposable filterSubscription;
	private readonly HashSet<string?> initiallySelectedProfileIds = [];

	[ObservableProperty] string? searchText;

	public ObservableCollection<GroupedProfiles> DisplayGroups { get; } = [];
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

	public ProfileSelectorViewModel(
				ReadOnlyObservableCollection<ObsFolder> allFolders,
				ReadOnlyObservableCollection<ObsProfile> allProfiles,
				IEnumerable<ObsProfile>? initiallySelectedProfiles)
				: base("Select Profiles") {
		this.allFolders = allFolders;
		this.allProfiles = allProfiles;

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
		foreach (var group in DisplayGroups) {
			group.Cleanup();
		}
		DisplayGroups.Clear();

		var filteredSourceProfiles = string.IsNullOrWhiteSpace(searchText)
				? [.. allProfiles]
				: allProfiles.Where(p => p.Title?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false);

		var distinctFolders = allFolders
				 .Where(f => f.Dto != null)
				 .Distinct()
				 .OrderBy(f => f.Title);

		foreach (var folder in distinctFolders) {
			var profilesInThisFolder = filteredSourceProfiles.Where(p => p.Dto?.folderId == folder.Dto?.id);

			var folderTitleMatchesOrContainsFilteredProfiles =
					profilesInThisFolder.Any() ||
					(!string.IsNullOrWhiteSpace(searchText) && (folder.Title?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false));

			if (folderTitleMatchesOrContainsFilteredProfiles) {
				var profileItemsForGroup = profilesInThisFolder
						.Select(p => new ProfileOrFolderItem(p, initiallySelectedProfileIds.Contains(p.Dto?.id.ToString()) || p.IsSelected))
						.OrderBy(pfi => pfi.DisplayName);

				var groupVM = new GroupedProfiles(folder.Dto!.id, folder.Title ?? "Unnamed Folder", profileItemsForGroup);
				DisplayGroups.Add(groupVM);
			}
		}

		var ungroupedProfiles = filteredSourceProfiles.Where(p => p.Dto?.folderId is null or 0).ToList();
		if (ungroupedProfiles.Count != 0) {
			var profileItemsForUngrouped = ungroupedProfiles
					.Select(p => new ProfileOrFolderItem(p, initiallySelectedProfileIds.Contains(p.Dto?.id.ToString()) || p.IsSelected))
					.OrderBy(pfi => pfi.DisplayName);

			var ungroupedGroupVM = new GroupedProfiles(0, "Ungrouped Profiles", profileItemsForUngrouped);
			DisplayGroups.Add(ungroupedGroupVM);
		}

		foreach (var group in DisplayGroups) {
			group.UpdateGroupSelectionState();
		}
	}

	public async Task<bool> ShowDialogAsync() {
		var result = await Mbox.ShowTaskDialog<ProfileSelectorView, ProfileSelectorViewModel>(new(
				Initialize: () => this,
				Header: "Select Profiles",
				SubHeader: "Select profiles to run the actor automation with. You can search and expand folders.",
				Symbas: Enums.Symbas.People,
				Btns: Enums.MBoxButtons.OkCancel)
		);

		if (result == Enums.TaskDialogResult.OK) {
			var dialogSelectedProfileIds = new HashSet<string?>();
			foreach (var group in DisplayGroups) {
				foreach (var pfi in group.ProfileItems) {
					if (pfi.IsSelected && pfi.Profile?.Dto?.id != null) {
						_ = dialogSelectedProfileIds.Add(pfi.Profile.Dto.id.ToString());
					}
				}
			}

			foreach (var originalProfile in allProfiles) {
				if (originalProfile.Dto?.id != null) {
					originalProfile.IsSelected = dialogSelectedProfileIds.Contains(originalProfile.Dto.id.ToString());
				}
			}
		}

		Dispose();

		return result == Enums.TaskDialogResult.OK && SelectedProfiles.Any();
	}

	public void Dispose() {
		filterSubscription?.Dispose();
		foreach (var group in DisplayGroups) {
			group.Cleanup();
		}
	}
}