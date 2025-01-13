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

namespace Chameleon.app.client.ViewModels;

public partial class MainViewModel : ObservableObjectBase {
	public event Action<ObsProfile>? OnBoundProfilesProfileSelectedChanged;
	[ObservableProperty]
	private MainAppSearchItem? selectedSearchTerm;

	[ObservableProperty]
	private bool isSplashVisible = true;

	public NavigationFactory NavigationFactory { get; } = new NavigationFactory();

	private readonly ReadOnlyObservableCollection<MainAppSearchItem> _boundProfiles;
	private readonly ReadOnlyObservableCollection<MainAppSearchItem> _boundFolders;
	public IEnumerable<MainAppSearchItem> SearchTerms => _boundProfiles.Concat(_boundFolders);

	private MainViewModel()
	{
		AppStartup.Instance.OnLoginSuccess += () => { IsSplashVisible = false; };
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

	public static MainViewModel Instance { get; } = new();
}
