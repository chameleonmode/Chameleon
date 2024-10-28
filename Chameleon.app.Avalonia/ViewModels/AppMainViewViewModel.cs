using System.Collections.ObjectModel;
using Avalonia.Controls;
using Chameleon.app.Avalonia.Com.DynamicData;
using Chameleon.app.Avalonia.Models.Observable;
using Chameleon.app.Avalonia.Models;
using Chameleon.app.Avalonia.ViewModels.Controllers;
using Chameleon.app.Avalonia.Views;
using Chameleon.Av.Fluent.Common.Models;
using Chameleon.lib.Api.Repos;
using Chameleon.lib.Common.Extensions;
using Chameleon.lib.Common.ServiceManagers;
using Chameleon.lib.CommunityToolkit.MvvM;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using FluentAvalonia.UI.Controls;
using DynamicData;
using Chameleon.lib.Common;

namespace Chameleon.app.Avalonia.ViewModels;
public partial class AppMainViewViewModel : ObservableObjectBase {
	public event Action<ObsProfile>? OnBoundProfilesProfileSelectedChanged;
	[ObservableProperty]
	private MainAppSearchItem? selectedSearchTerm;

	[ObservableProperty]
	private bool isSplashVisible = true;

	public NavigationFactory NavigationFactory { get; } = new NavigationFactory(Instance);

	private readonly ReadOnlyObservableCollection<MainAppSearchItem> _boundProfiles;
	private readonly ReadOnlyObservableCollection<MainAppSearchItem> _boundFolders;
	public IEnumerable<MainAppSearchItem> SearchTerms => _boundProfiles.Concat(_boundFolders);

	private AppMainViewViewModel()
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

	public static AppMainViewViewModel Instance { get; } = new();
}

public class NavigationFactory(AppMainViewViewModel owner) : INavigationPageFactory {
	public AppMainViewViewModel Owner { get; } = owner;

	public Control GetPage(Type srcType)
	{
		var c = IoC.GetService(srcType) as Control ?? IoC.GetService(srcType) as Control;
		ArgumentNullException.ThrowIfNull(c, "Could not resolve page from type");
		return c;
	}

	public Control? GetPageFromObject(object target)
	{
		if (target is MainPageModelBase t) {
			Control? c = null;

			//if (t.NavHeader == "Dashboard")
			//	c = ContainerServiceHelper.Resolve<IDashboardView>() as Control;
			//else if (t.NavHeader == "Profiles")
			//	c = ContainerServiceHelper.Resolve<IProjectsView>() as Control;
			//else if (t.NavHeader == "Automation")
			//	c = ContainerServiceHelper.Resolve<IProjectsView>() as Control;
			//else if (t.NavHeader == "Settings")
			//	c = ContainerServiceHelper.Resolve<ISettingsView>() as Control;

			return c;
		} else if (target is string nameOf) {
			//var c = ContainerServiceHelper.Resolve<ISettingsView>() as Control;
			return ResolvePage(nameOf);
		} else {
			return ResolvePage(target as PageBaseModel);
		}
	}

	private Control? ResolvePage(PageBaseModel? pbvm)
	{
		if (pbvm is null)
			return null;

		Control? page = null;
		var key = pbvm.PageKey;

		return page;
	}

	private Control? ResolvePage(string pbvm)
	{
		Control? page = null;

		if (SettingsPages.TryGetValue(pbvm, out var func)) {
			page = func();
			//(page as ChameleonPageBase).CreationContext = pbvm;
		}

		return page;
	}

	private readonly Dictionary<string, Func<Control?>> SettingsPages = new() {
		//{ nameof(IUserDefaultSettingsView), () =>  ContainerServiceHelper.Resolve<IUserDefaultSettingsView>() as Control },
	};
}