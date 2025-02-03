using Chameleon.app.Avalonia.Models.Observable;
using Chameleon.app.Avalonia.Models;
using Chameleon.app.Avalonia.ViewModels.Controllers;
using Chameleon.app.Avalonia.Views;
using Chameleon.app.Avalonia;
using Chameleon.lib.Api.Repos;
using Chameleon.lib.Common.Extensions;
using Chameleon.lib.CommunityToolkit.MvvM;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using DynamicData;
using System.Linq;
using System.Reflection;
using Chameleon.lib.Abs.Platformatic;
using System.Threading.Tasks;
using Chameleon.lib.Helpers;

using ProjectsView = Chameleon.app.Avalonia.Features.ProfilesAndFolders.Projects.View;
using UserProfilesViewModel = Chameleon.app.Avalonia.Features.ProfilesAndFolders.Profiles.MyProfiles.ViewModel;

namespace Chameleon.app.client.ViewModels;

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
	public IEnumerable<MainAppSearchItem> SearchTerms => _boundProfiles.Concat(_boundFolders);

	private MainViewModel()
	{
		AppStartup.Instance.OnLoginSuccess += async () => {
			IsSplashVisible = false;

			try {
				var current = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "2024.x.x.x";
				var appClientInfo = await PlatformaticDB.Instance.GetLatestVersion;
				if (appClientInfo != null && appClientInfo.latest != current) {
					InfoBarTitle = "New Version Available";
					InfoBarMessage = $"Download the latest version of Chameleon ({appClientInfo.latest})";
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
				ViewModel = new ObsFolder(i),
				PageType = this.GetType()
			})
			.Bind(out _boundFolders)
			.Subscribe(i => { OnPropertyChanged(nameof(SearchTerms)); });
	}

	partial void OnSelectedSearchTermChanged(MainAppSearchItem? oldValue, MainAppSearchItem? newValue)
	{
		if (newValue is null) return;

		if (newValue.ViewModel is ViewModelObjectBase nfs)
			nfs.Navigated = false;

		Navigator.NavigateToType(typeof(ProjectsView), newValue.ViewModel);
	}

	[RelayCommand]
	private void ClearSearch()
	{
		SelectedSearchTerm = null;
		UserProfilesViewModel.Instance.OnFilterTo();
	}

	[RelayCommand]
	private void ClickSearch(string p)
	{
		if (!p.Is())
			ClearSearch();
		else
			Navigator.NavigateToType(typeof(ProjectsView), p);
	}

	[RelayCommand]
	private async Task DownloadLatest() {
		InfoBarOpen = false;
		InfoBarOpen = !await PlatformaticDB.Instance.DownloadLatest(Toaster.Info);
		if (InfoBarOpen)
			Toaster.Error("Failed to download latest version");
	}

	public static MainViewModel Instance { get; } = new();
}
