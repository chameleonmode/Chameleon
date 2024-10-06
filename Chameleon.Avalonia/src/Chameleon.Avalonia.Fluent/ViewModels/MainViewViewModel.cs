using Avalonia.Collections;
using Avalonia.Controls;

using Chameleon.app.Avalonia;
using Chameleon.Application.Startup;
using Chameleon.Auth.Services;
using Chameleon.Av.Fluent.Common.Services;
using Chameleon.Common.Helpers;
using Chameleon.Core.Extensions;
using Chameleon.CT.Common.Base;
using Chameleon.Interfaces;
using Chameleon.Interfaces.App;
using Chameleon.Interfaces.App.UserProfiles;
using Chameleon.Interfaces.Auth;
using Chameleon.Interfaces.Services;
using Chameleon.Interfaces.UserProfileFolders;
using Chameleon.Prism.Events;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;

namespace Chameleon.Av.Fluent.ViewModels;

public partial class MainViewViewModel : ObservableObjectBase, IMainViewViewModel
{
    [ObservableProperty]
    private MainAppSearchItem _selectedSearchTerm;

    [ObservableProperty]
    private bool isSplashVisible = true;

    public AvaloniaList<MainAppSearchItem> SearchTerms { get; } = [];

    public MainViewViewModel()
    {
		AppStartup.Instance.OnLoginSuccess += () => IsSplashVisible = false;

		EventAggregator
            .GetEvent<LoginSuccessEvent>()
            .SubscribeOnce(() => IsSplashVisible = false);
    }


    public void BuildSearchTerms(List<MainAppSearchItem> items)
    {
        SearchTerms.Clear();
        SearchTerms.AddRange(items);
    }

    partial void OnSelectedSearchTermChanged(MainAppSearchItem? oldValue, MainAppSearchItem newValue)
    {
        if (newValue is null) return;

        if (newValue.ViewModel is INavigateFromSearch nfs)
            nfs.Navigated = false;

        ContainerServiceHelper.Resolve<INavigationService>().NavigateToType(typeof(IProjectsView), newValue.ViewModel);
    }

    [RelayCommand]
    private void ClearSearch()
    {
        SelectedSearchTerm = null;
        ContainerServiceHelper.Resolve<IUserProfilesViewModel>().OnFilterTo();
    }

    [RelayCommand]
    private void ClickSearch(string p)
    {
        if (!p.HasAny())
            ClearSearch();
        else
            ContainerServiceHelper.Resolve<INavigationService>().NavigateToType(typeof(IProjectsView), p);
    }
}

