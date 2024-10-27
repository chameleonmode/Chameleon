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
	[ObservableProperty]
	private MainAppSearchItem? selectedSearchTerm;

	[ObservableProperty]
	private bool isSplashVisible = true;

	private readonly ReadOnlyObservableCollection<ObsProfile> _boundProfilesList;
	private readonly ReadOnlyObservableCollection<ObsFolder> _boundFoldersList;
	public ObservableCollection<MainAppSearchItem> SearchTerms { get; } = [];

	public NavigationFactory NavigationFactory { get; }

	private AppMainViewViewModel()
	{
		AppStartup.Instance.OnLoginSuccess += () => { IsSplashVisible = false; };

		NavigationFactory = new NavigationFactory(this);

		_ = UserProfilesRepo
			.Connect()
			.Transform(i => new ObsProfile(i))
			.SortAndBind(out _boundProfilesList, Compares.ObsProfileCompares.AscendingComparer)
			.Subscribe((i) => {
				if (_boundProfilesList != null) {
					foreach (var profile in _boundProfilesList) {
						if (_boundProfilesList?.Contains(profile) == false && SearchTerms.FirstOrDefault(a => a.ViewModel == profile) is MainAppSearchItem st) {
							_ = SearchTerms.Remove(st);
						} else if (_boundProfilesList?.Contains(profile) == true && SearchTerms.FirstOrDefault(a => a.ViewModel == profile) is null) {
							SearchTerms.Add(new() {
								Header = profile.Title ?? "xxx",
								Namespace = "Profile",
								ViewModel = profile,
								PageType = this.GetType()
							});
						} else if (_boundProfilesList?.Contains(profile) == true && SearchTerms.FirstOrDefault(a => a.ViewModel == profile) is MainAppSearchItem str) {
							str.Header = profile.Title ?? "xxx";
							str.ViewModel = profile;
						}
					}
				}
			});

		_ = UserProfilesFolderRepo
			.Connect()
			.Transform(i => new ObsFolder(i))
			.SortAndBind(out _boundFoldersList, Compares.ObsFolderCompares.AscendingComparer)
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
	}

	public void BuildSearchTerms(List<MainAppSearchItem> items)
	{
		//SearchTerms.Clear();
		SearchTerms.AddRange(items);
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