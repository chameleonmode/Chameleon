using Chameleon.app.Avalonia.Models.Observable;
using Chameleon.lib.Common.Constants;
using Chameleon.lib.Common.ServiceManagers;
using Chameleon.lib.CommunityToolkit.MvvM;
using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData;
using DynamicData.Binding;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Chameleon.client.Features.Automation.Actors.Dialogs;

public partial class ProfileOrFolderItem: ObservableObject {

	[ObservableProperty] bool isSelected = false;
	public object Item { get; } = null!;
	public ObsFolder? Folder => Item as ObsFolder;
	public ObsProfile? Profile => Item as ObsProfile;
	public bool IsFolder => Folder != null;
	public bool IsProfile => Profile != null;
	public string DisplayName => Folder?.Title ?? Profile?.Title ?? "Unknown";

	public long FolderIdKey => Folder?.Dto?.id ?? Profile?.Dto?.folderId ?? 0;

	public ProfileOrFolderItem(object item, bool isSelected = false) {
		if(item is ObsProfile profile) {
			Item = new ObsProfile(profile.Dto, isShowCheckboxColumn: false, isShowGlyph: true, hasActionOptions: false);
		}

		if(item is ObsFolder folder) {
			Item = new ObsFolder(folder.Dto, hasActionOptions: false, onSelectedChanged: null, nameAlreadyExist: null);
		}
		this.isSelected = isSelected;
	}

	partial void OnIsSelectedChanged(bool value) {
		if (Profile != null) {
			Profile.IsSelected = value;
		}
	}
}

public partial class ProfileSelectorViewModel : ViewModelObjectBase, IDisposable {
	private readonly ReadOnlyObservableCollection<ObsFolder> allFolders;
	private readonly ReadOnlyObservableCollection<ObsProfile> allProfiles;
	private readonly IDisposable cleanup;

	[ObservableProperty] string? searchText;

	private readonly SourceList<ProfileOrFolderItem> sourceItems = new();

	private readonly BehaviorSubject<Func<ProfileOrFolderItem, bool>> filterPredicate = new(item => true);

	private readonly ReadOnlyObservableCollection<IGroup<ProfileOrFolderItem, long>> groupedFilteredItems;
	public ReadOnlyObservableCollection<IGroup<ProfileOrFolderItem, long>> GroupedFilteredItems => groupedFilteredItems;

	public IEnumerable<ObsProfile> SelectedProfiles => sourceItems.Items
																											.Where(item => item.IsProfile && item.IsSelected)
																											.Select(item => item.Profile!);

	public ProfileSelectorViewModel(
				ReadOnlyObservableCollection<ObsFolder> allFolders,
				ReadOnlyObservableCollection<ObsProfile> allProfiles)
				: base("Select Profiles") {
		this.allFolders = allFolders;
		this.allProfiles = allProfiles;

		PopulateSourceItems();

		var pipeline = sourceItems.Connect()
				.Filter(filterPredicate)
				.Sort(SortExpressionComparer<ProfileOrFolderItem>
							.Ascending(p => p.IsFolder ? 0 : 1)
							.ThenByAscending(p => p.DisplayName))
				.GroupOn(item => item.FolderIdKey)
				.Bind(out groupedFilteredItems)
				.DisposeMany()
				.Subscribe();

		var filterSubscription = this.WhenValueChanged(x => x.SearchText)
				.Throttle(TimeSpan.FromMilliseconds(300))
				.Select(text => (Func<ProfileOrFolderItem, bool>)(item => FilterLogic(item, text)))
				.Subscribe(filterPredicate);

		cleanup = new System.Reactive.Disposables.CompositeDisposable(pipeline, filterSubscription, sourceItems);
	}

	private bool FilterLogic(ProfileOrFolderItem item, string? searchText) {
		if (string.IsNullOrWhiteSpace(searchText))
			return true;

		if (item.IsProfile) {
			return item.Profile?.Title?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false;
		}

		if (item.IsFolder) {
			var folderId = item.Folder?.Dto?.id ?? 0;
			return allProfiles.Any(p => (p.Dto?.folderId ?? 0) == folderId &&
																	 (p.Title?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false));
		}

		return false;
	}

	private void PopulateSourceItems() {
		sourceItems.Edit(innerList => {
			innerList.Clear();
			foreach (var folder in allFolders) {
				if(folder.ProfilesCount == 0) continue;
				innerList.Add(new ProfileOrFolderItem(folder, false));
			}

			foreach (var profile in allProfiles) {
				var wrapper = new ProfileOrFolderItem(profile, profile.IsSelected);
				innerList.Add(wrapper);
			}
		});
	}

	public async Task<bool> ShowDialogAsync() {
		var result = await Mbox.ShowTaskDialog<ProfileSelectorView, ProfileSelectorViewModel>(new(
				Initialize: () => this,
				Header: "Select Profiles",
				SubHeader: "Select profiles to run the actor automation with. You can search and expand folders.",
				Symbas: Enums.Symbas.People,
				Btns: Enums.MBoxButtons.OkCancel)
		);
		return result == Enums.TaskDialogResult.OK && SelectedProfiles.Any();
	}

	public void Dispose() {
		cleanup?.Dispose();
		foreach (var item in sourceItems.Items.OfType<IDisposable>()) {
			item.Dispose();
		}
		sourceItems.Dispose();
	}
}