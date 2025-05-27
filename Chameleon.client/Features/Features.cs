using Microsoft.Extensions.DependencyInjection;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using System.Collections.ObjectModel;
using System.Reactive.Linq;

using DynamicData;
using DynamicData.PLinq;

using Chameleon.client.Features.Automation.Playwright;
using Chameleon.client.Features.Tenants.Members;
using Chameleon.client.Features.Automation.Actors;

using Chameleon.app.Avalonia.Models.Observable;
using Chameleon.app.Avalonia;

using Chameleon.lib.Api.Repos;
using Chameleon.lib.Util;
using Chameleon.lib.CommunityToolkit.MvvM;
using Chameleon.lib.Abs.Platformatic;
using Chameleon.lib.Helpers;
using Chameleon.lib.Common.Models.Dto;
using Chameleon.client.Features.ProfilesAndFolders.Profiles.Identity;
using Chameleon.client.Features.ProfilesAndFolders.Projects;
using Chameleon.client.Features.ProfilesAndFolders.Search;
using Chameleon.client.Features.ProfilesAndFolders.Search.ByTags;
using Chameleon.client.Features.ProfilesAndFolders.Search.ByTags.Controls;
using Chameleon.client.Features.ProfilesAndFolders.Profiles.MyProfiles;

namespace Chameleon.client.Features;

public static class Modules {
  public static IServiceCollection Automation(this IServiceCollection services) => services
  .AddSingleton<Automation.View>()
  .AddSingleton<Automation.ViewModel>()
  .AddSingleton<Automation.AI.ChameleonAIR.View>()
  .AddSingleton<Automation.AI.ChameleonAIR.ViewModel>()
  .AddSingleton<PlaywrightView>()
  .AddSingleton<PlaywrightViewModel>()
  .AddSingleton<ActorsViewModel>()
  .AddSingleton<ActorsView>();

  public static IServiceCollection WithProfilesAndFolders(this IServiceCollection services) => services
  .AddSingleton<IdentityView>()
  .AddSingleton<IdentityViewModel>()
  .AddSingleton<ProjectsView>()
  .AddSingleton<ProjectsViewModel>();

  public static IServiceCollection WithAllPagesAndFeatures(this IServiceCollection services) => services
  .Automation()
  .WithProfilesAndFolders()
  .AddSingleton<Dashboard.View>()
  .AddSingleton<Dashboard.ViewModel>()
  .AddSingleton<Tenants.ViewModel>()
  .AddSingleton<Tenants.View>()
  .AddSingleton<TenantMembersView>()
  .AddSingleton<TenantMembersViewModel>()
  .AddSingleton<Settings.View>()
  .AddSingleton<Settings.ViewModel>();
}

public partial class ViewModel : ObservableObjectBase {
  public event Action<ObsProfile>? OnBoundProfilesProfileSelectedChanged;

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

  private ViewModel() {
#if DEBUG
    AppStartup.Instance.OnLoginSuccess += () => {
      IsSplashVisible = false;
    };
#else
		AppStartup.Instance.OnLoginSuccess += async () => {
			IsSplashVisible = false;
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
		};
#endif
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
        ViewModel = new ObsFolder(i, null),
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
          .SelectMany(x => x.Select<TagItemDto, TagsSearchViewModelBase?>(t => t.Type switch {
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
    MyProfilesViewModel.Instance.OnFilterTo();
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

  public static ViewModel Instance { get; } = new();
}

