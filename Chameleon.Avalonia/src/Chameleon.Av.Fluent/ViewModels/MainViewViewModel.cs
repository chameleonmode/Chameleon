using Avalonia.Collections;
using Avalonia.Controls;
using Chameleon.Auth.Services;
using Chameleon.Av.Fluent.Common.Services;
using Chameleon.Common.Helpers;
using Chameleon.CT.Common.Base;
using Chameleon.Interfaces;
using Chameleon.Interfaces.App.UserProfiles;
using Chameleon.Interfaces.Auth;
using Chameleon.Interfaces.Services;
using Chameleon.Prism.Events;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;

namespace Chameleon.Av.Fluent.ViewModels;

public partial class MainViewViewModel:ObservableObjectBase, IMainViewViewModel
{
    public AvaloniaList<MainAppSearchItem> SearchTerms { get; } = new AvaloniaList<MainAppSearchItem>();

    [ObservableProperty]
    private MainAppSearchItem _selectedSearchTerm;

    [ObservableProperty]
    private bool isSplashVisible = true;


    public MainViewViewModel()
    {
        //EventAggregator
        //    .GetEvent<LoginFailEvent>()
        //    .SubscribeOnce(LoginFailEventMethod);

        EventAggregator
            .GetEvent<LoginSuccessEvent>()
            .SubscribeOnce(LoginSuccessEventMethod);
    }

    private void LoginSuccessEventMethod()
    {
        IsSplashVisible = false;
    }

    //private async void LoginFailEventMethod()
    //{
    //    if (!Design.IsDesignMode)
    //    {
    //        IsSplashVisible = true;
    //        await _authService.ShowLoginDialogAsync();
    //    }
    //}

    public void BuildSearchTerms(List<MainAppSearchItem> items)
    {
        SearchTerms.Clear();
        foreach (var item in items)
        {
            SearchTerms.Add(item);
        }

        //OnPropertyChanged(nameof(SearchTerms));
    }

    partial void OnSelectedSearchTermChanged(MainAppSearchItem? oldValue, MainAppSearchItem newValue)
    {
        //var pvm = ContainerServiceHelper.Resolve<IUserProfilesViewModel>();
        //var pvm2 = ContainerServiceHelper.Resolve<IUserProfileFoldersViewModel>();
        if (newValue != null) 
            ContainerServiceHelper.Resolve<INavigationService>().NavigateToType(typeof(IProjectsView), newValue.ViewModel);
    }

    [RelayCommand]
    private void ClearSearch()
    {
        SelectedSearchTerm = null;
        ContainerServiceHelper.Resolve<IUserProfilesViewModel>().OnFilterTo();
    }
}

