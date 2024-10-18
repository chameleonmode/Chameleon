using Avalonia.Collections;
using Avalonia.Controls;

using Chameleon.app.Avalonia;
using Chameleon.Common.Helpers;
using Chameleon.Core.Extensions;
using Chameleon.Interfaces;
using Chameleon.Interfaces.App;
using Chameleon.Interfaces.App.UserProfiles;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using FluentAvalonia.UI.Controls;
using Chameleon.lib.Common;
using Chameleon.Av.Fluent.Common.Models;
using Chameleon.Interfaces.Dashboard;
using Chameleon.Interfaces.Settings;
using Chameleon.lib.Common.ServiceManagers;
using Chameleon.lib.Api.Repos;
using Chameleon.app.Avalonia.Com.DynamicData;
using DynamicData;
using Chameleon.app.Avalonia.Models.Observable;
using Chameleon.lib.CommunityToolkit.MvvM;

namespace Chameleon.Av.Fluent.ViewModels;

public partial class MainViewViewModel : ObservableObjectBase, IMainViewViewModel {
	[ObservableProperty]
	private MainAppSearchItem? selectedSearchTerm;

	[ObservableProperty]
	private bool isSplashVisible = true;

	private readonly IList<ObsProfile> _boundProfilesList;
	private readonly IList<ObsFolder> _boundFoldersList;
	public AvaloniaList<MainAppSearchItem> SearchTerms { get; } = [];

	public NavigationFactory NavigationFactory { get; }

	public MainViewViewModel()
	{
		AppStartup.Instance.OnLoginSuccess += OnLoginSuccess;
		NavigationFactory = new NavigationFactory(this);

		_ = UserProfilesRepo
			.Connect()
			.Transform(i => new ObsProfile(i))
			.SortAndBind(out var plist, Compares.ObsProfileCompares.AscendingComparer)
			.Subscribe((i) => {
				foreach (var c in i) {
					if (_boundProfilesList?.Contains(c.Current) == false && SearchTerms.FirstOrDefault(a => a.ViewModel == c.Current) is MainAppSearchItem st) {
						SearchTerms.Remove(st);
					} else if (_boundProfilesList?.Contains(c.Current) == true && SearchTerms.FirstOrDefault(a => a.ViewModel == c.Current) is null) {
						SearchTerms.Add(new() {
							Header = c.Current.Title ?? "xxx",
							Namespace = "Profile",
							ViewModel = c.Current,
							PageType = this.GetType()
						});
					} else if (_boundProfilesList?.Contains(c.Current) == true && SearchTerms.FirstOrDefault(a => a.ViewModel == c.Current) is MainAppSearchItem str) {
						str.Header = c.Current.Title ?? "xxx";
						str.ViewModel = c.Current;
					}
				}
			});
		_boundProfilesList = plist;

		_ = UserProfilesFolderRepo
			.Connect()
			.Transform(i => new ObsFolder(i))
			.SortAndBind(out var flist, Compares.ObsFolderCompares.AscendingComparer)
			.Subscribe((i) => {
				foreach (var c in i) {
					if (_boundFoldersList?.Contains(c.Current) == false && SearchTerms.FirstOrDefault(a => a.ViewModel == c.Current) is MainAppSearchItem st) {
						SearchTerms.Remove(st);
					} else if (_boundFoldersList?.Contains(c.Current) == true && SearchTerms.FirstOrDefault(a => a.ViewModel == c.Current) is null) {
						SearchTerms.Add(new() {
							Header = c.Current.Title ?? "xxx",
							Namespace = "Folder",
							ViewModel = c.Current,
							PageType = this.GetType()
						});
					} else if (_boundFoldersList?.Contains(c.Current) == true && SearchTerms.FirstOrDefault(a => a.ViewModel == c.Current) is MainAppSearchItem str) {
						str.Header = c.Current.Title ?? "xxx";
						str.ViewModel = c.Current;
					}
				}
			});
		_boundFoldersList = flist;
	}
	private async void OnLoginSuccess()
	{
		IsSplashVisible = false;
	}

	public void BuildSearchTerms(List<MainAppSearchItem> items)
	{
		//SearchTerms.Clear();
		SearchTerms.AddRange(items);
	}

	partial void OnSelectedSearchTermChanged(MainAppSearchItem? oldValue, MainAppSearchItem? newValue)
	{
		if (newValue is null) return;

		if (newValue.ViewModel is INavigateFromSearch nfs)
			nfs.Navigated = false;

		Navigator.NavigateToType(typeof(IProjectsView), newValue.ViewModel);
	}

	[RelayCommand]
	private void ClearSearch()
	{
		SelectedSearchTerm = null;
		ContainerServiceHelper.Resolve<IUserProfilesViewModel>()?.OnFilterTo();
	}

	[RelayCommand]
	private void ClickSearch(string p)
	{
		if (!p.HasAny())
			ClearSearch();
		else
			Navigator.NavigateToType(typeof(IProjectsView), p);
	}
}

public class NavigationFactory(MainViewViewModel owner) : INavigationPageFactory {
	public MainViewViewModel Owner { get; } = owner;

	public Control GetPage(Type srcType)
	{
		var c = ContainerServiceHelper.Resolve(srcType) as Control ?? IoC.GetService(srcType) as Control;
		ArgumentNullException.ThrowIfNull(c, "Could not resolve page from type");
		return c;
	}

	public Control? GetPageFromObject(object target)
	{
		if (target is MainPageModelBase t) {
			Control? c = null;

			if (t.NavHeader == "Dashboard")
				c = ContainerServiceHelper.Resolve<IDashboardView>() as Control;
			else if (t.NavHeader == "Profiles")
				c = ContainerServiceHelper.Resolve<IProjectsView>() as Control;
			else if (t.NavHeader == "Automation")
				c = ContainerServiceHelper.Resolve<IProjectsView>() as Control;
			else if (t.NavHeader == "Settings")
				c = ContainerServiceHelper.Resolve<ISettingsView>() as Control;

			return c;
		} else if (target is string nameOf) {
			var c = ContainerServiceHelper.Resolve<ISettingsView>() as Control;
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

	private readonly Dictionary<string, Func<Control?>> SettingsPages = new()
	{
				 { nameof(IUserDefaultSettingsView), () =>  ContainerServiceHelper.Resolve<IUserDefaultSettingsView>() as Control },
	};
}