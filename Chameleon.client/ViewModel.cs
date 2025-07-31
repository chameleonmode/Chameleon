
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using System.Collections.ObjectModel;
using System.Reactive.Linq;

using DynamicData;
using DynamicData.PLinq;

using Chameleon.client.MvvM;
using Chameleon.lib.Helpers;
using Chameleon.client.Features.Projects.Search;
using Chameleon.client.Features.Projects.Search.ByTags;
using Chameleon.client.Features.Projects.Search.ByTags.Controls;
using Chameleon.client.Features.Projects.Folders;
using Chameleon.lib.Api.Repos;
using Chameleon.lib.Abs.Platformatic;
using Chameleon.client.Features;
using Chameleon.client.Features.Projects.Profiles;
using Chameleon.client.Services;
using Chameleon.lib.Browzio;
using Chameleon.lib.Playwright;
using Chameleon.lib.Abs.Repos;

namespace Chameleon.client;

public partial class ViewModel : OO {
  [ObservableProperty] MainAppSearchItem? selectedSearchTerm;
  [ObservableProperty] bool isSplashVisible = true;
  [ObservableProperty] bool infoBarOpen;
  [ObservableProperty] string? infoBarMessage;
  [ObservableProperty] string? infoBarTitle;

  public NavigationFactory NavigationFactory { get; } = new NavigationFactory();

  private readonly ReadOnlyObservableCollection<MainAppSearchItem> _boundProfiles;
  private readonly ReadOnlyObservableCollection<MainAppSearchItem> _boundFolders;
  private readonly ReadOnlyObservableCollection<MainAppSearchItem> _boundTags;
  public IEnumerable<MainAppSearchItem> SearchTerms => _boundProfiles
    .Concat(_boundFolders)
    .Concat(_boundTags);

  ViewModel() {
    _ = UserProfilesRepo.Connect().Transform(i => new MainAppSearchItem() {
      Header = i.title ?? "xxx",
      Namespace = "Profile",
      ViewModel = new ObsProfile(i) { IsShowCheckboxColumn = false },
      PageType = this.GetType()
    })
    .Bind(out _boundProfiles)
    .Subscribe(i => { OnPropertyChanged(nameof(SearchTerms)); });

    _ = UserProfilesFolderRepo.Connect().Transform(i => new MainAppSearchItem() {
      Header = i.title ?? "xxx",
      Namespace = "Folder",
      ViewModel = new ObsFolder(i),
      PageType = this.GetType()
    })
    .Bind(out _boundFolders)
    .Subscribe(i => { OnPropertyChanged(nameof(SearchTerms)); });

    _ = TagsRepo.Connect().Transform(i => new MainAppSearchItem() {
      Header = $"#{i.Name}",
      Namespace = "Tag",
      ViewModel = i,
      PageType = this.GetType(),
      SearchType = SearchType.Tags,
      Items = i.Items.Select(x => new TagItemDto(x.Key, x.Value))
      .GroupBy(x => x.Type)
      .Select(x => x.ToList())
      .SelectMany(x => x.Select<TagItemDto, TagsSearchViewModelBase?>(t => t.Type switch {
        TagItemType.Folder => new TagFolderSearchViewModel(t),
        TagItemType.Profile => new TagProfilesSearchViewModel(t),
        _ => null
      }))
    })
    .Bind(out _boundTags)
    .Subscribe(i => { OnPropertyChanged(nameof(SearchTerms)); });

    AsyncCommandMap["DownloadLatest"] = async () => {
      InfoBarOpen = false;
      InfoBarOpen = !await Service.Routes.App.DownloadLatest((msg) => Toaster.Info(msg));
      if (InfoBarOpen) Toaster.Error("Failed to download latest version");
    };
  }

  partial void OnSelectedSearchTermChanged(MainAppSearchItem? oldValue, MainAppSearchItem? newValue) {
    if (newValue is null) return;

    if (newValue.ViewModel is OOVM nfs)
      nfs.Navigated = false;

    Navigator.NavigateToType(typeof(Features.Projects.View), newValue.ViewModel);
  }

  [RelayCommand]
  void ClickSearch(string p) {
    Navigator.NavigateToType(typeof(Features.Projects.View), p);
    ProfilesViewModel.Instance.SearchText = p;
  }

  public async Task Init() {
    // This is where you can initialize any data or state needed for the ViewModel
    // For example, you might want to load initial data from a repository or service
    await Browzio.I.Init();
    await Playwrightio.I.Init();
    await Modules.Sync();
    IsSplashVisible = false;
#if DEBUG
#else
    try {
      var current = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "2024.x.x.x";
      var appClientInfo = await Service.Routes.App.GetLatestVersion;
      if (appClientInfo != null && appClientInfo.Latest != current) {
        InfoBarTitle = "New Version Available";
        InfoBarMessage = $"Download the latest version of Chameleon ({appClientInfo.Latest})";
        InfoBarOpen = true;
      }
    } catch (Exception e) {
      Toaster.Error(e.Message);
    }
#endif
  }

  public static ViewModel Instance { get; } = new();
}
