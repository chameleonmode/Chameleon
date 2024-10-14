using Avalonia.Collections;
using Avalonia.Controls;

using Chameleon.app.Avalonia;
using Chameleon.Common.Helpers;
using Chameleon.Core.Extensions;
using Chameleon.CT.Common.Base;
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

namespace Chameleon.Av.Fluent.ViewModels;

public partial class MainViewViewModel : ObservableObjectBase, IMainViewViewModel {
	[ObservableProperty]
	private MainAppSearchItem? selectedSearchTerm;

	[ObservableProperty]
	private bool isSplashVisible = true;

	public AvaloniaList<MainAppSearchItem> SearchTerms { get; } = [];

	public NavigationFactory NavigationFactory { get; }

	public MainViewViewModel()
	{
		AppStartup.Instance.OnLoginSuccess += () => IsSplashVisible = false;
		NavigationFactory = new NavigationFactory(this);
	}


	public void BuildSearchTerms(List<MainAppSearchItem> items)
	{
		SearchTerms.Clear();
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