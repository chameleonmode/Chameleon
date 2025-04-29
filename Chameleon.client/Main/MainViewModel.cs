using Chameleon.app.Avalonia.Models.Observable;
using Chameleon.app.Avalonia.Models;
using Chameleon.app.Avalonia;
using Chameleon.lib.Api.Repos;
using Chameleon.lib.Util;
using Chameleon.lib.CommunityToolkit.MvvM;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using DynamicData;
using System.Reflection;
using Chameleon.lib.Abs.Platformatic;
using Chameleon.lib.Helpers;

using UserProfilesViewModel = Chameleon.app.Avalonia.Features.ProfilesAndFolders.Profiles.MyProfiles.MyProfilesViewModel;
using DynamicData.PLinq;
using Chameleon.lib.Common.Models.Dto;
using Chameleon.app.Avalonia.Features.Search.ByTags.Controls;
using System.Reactive.Linq;
using Chameleon.app.Avalonia.Features.Search.ByTags;
using Chameleon.app.Avalonia.Features.ProfilesAndFolders.Projects;

namespace Chameleon.client.Main;

public partial class MainViewModel : ObservableObjectBase {
	public event Action<ObsProfile>? OnBoundProfilesProfileSelectedChanged;

	[ObservableProperty]
	private MainAppSearchItem? selectedSearchTerm;
	[ObservableProperty]
	private bool isSplashVisible = true;
	[ObservableProperty]
	private bool infoBarOpen;
	[ObservableProperty]
	private string? infoBarMessage;
	[ObservableProperty]
	private string? infoBarTitle;

	public NavigationFactory NavigationFactory { get; } = new NavigationFactory();

	private readonly ReadOnlyObservableCollection<MainAppSearchItem> _boundProfiles;
	private readonly ReadOnlyObservableCollection<MainAppSearchItem> _boundFolders;
	private readonly ReadOnlyObservableCollection<MainAppSearchItem> _boundTags;
	public IEnumerable<MainAppSearchItem> SearchTerms => _boundProfiles
		.Concat(_boundFolders)
		.Concat(_boundTags);

	private MainViewModel() {
		AppStartup.Instance.OnLoginSuccess += async () => {
			IsSplashVisible = false;

			try {
				var current = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "2024.x.x.x";
				var appClientInfo = await Service.Routes.App.GetLatestVersion;
				if (appClientInfo != null && appClientInfo.Latest != current) {
					InfoBarTitle = "New Version Available";
					InfoBarMessage = $"Download the latest version of Chameleon ({appClientInfo.Latest})";
					InfoBarOpen = true;
				}
			} catch (Exception e) {
				Toaster.Error(e.Message);
			}
		};
		_ = UserProfilesRepo
			.Connect()
			.Transform(i => new MainAppSearchItem() {
				Header = i.title ?? "xxx",
				Namespace = "Profile",
				ViewModel = new ObsProfile(i, onSelectedChanged: OnBoundProfilesProfileSelectedChanged),
				PageType = this.GetType()
			})
			.Bind(out _boundProfiles)
			.Subscribe(i => { OnPropertyChanged(nameof(SearchTerms)); });

		_ = UserProfilesFolderRepo
			.Connect()
			.Transform(i => new MainAppSearchItem() {
				Header = i.title ?? "xxx",
				Namespace = "Folder",
				ViewModel = new ObsFolder(i,null),
				PageType = this.GetType()
			})
			.Bind(out _boundFolders)
			.Subscribe(i => { OnPropertyChanged(nameof(SearchTerms)); });

		_ = TagsRepo.Connect()
			.Transform(i => new MainAppSearchItem() {
				Header = $"#{i.Name}",
				Namespace = "Tag",
				ViewModel = i,
				SearchType = SearchType.Tags,
				Items = i.Items.Select(x => new TagItemDto(x.Key, x.Value))
					.GroupBy(x => x.Type)
					.Select(x => x.ToList())
					.SelectMany(x =>  x.Select<TagItemDto, TagsSearchViewModelBase?>(t => t.Type switch {
						TagItemType.Folder => new TagFolderSearchViewModel(t),
						TagItemType.Profile => new TagProfilesSearchViewModel(t),
						_ => null
						})
					),
				PageType = this.GetType()
			})
			.Bind(out _boundTags)
			.Subscribe(i => { OnPropertyChanged(nameof(SearchTerms)); });
	}

	partial void OnSelectedSearchTermChanged(MainAppSearchItem? oldValue, MainAppSearchItem? newValue) {
		if (newValue is null) return;

		if (newValue.ViewModel is ViewModelObjectBase nfs)
			nfs.Navigated = false;

		Navigator.NavigateToType(typeof(ProjectsView), newValue.ViewModel);
	}

	[RelayCommand]
	private void ClearSearch() {
		SelectedSearchTerm = null;
		UserProfilesViewModel.Instance.OnFilterTo();
	}

	[RelayCommand]
	private void ClickSearch(string p) {
		if (p.Is())
			ClearSearch();
		else
			Navigator.NavigateToType(typeof(ProjectsView), p);
	}

	[RelayCommand]
	private async Task DownloadLatest() {
		InfoBarOpen = false;
		InfoBarOpen = !await Service.Routes.App.DownloadLatest((msg) => Toaster.Info(msg));
		if (InfoBarOpen)
			Toaster.Error("Failed to download latest version");
	}

	public static MainViewModel Instance { get; } = new();
}
