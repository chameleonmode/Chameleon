using AutoMapper;
using Avalonia.Controls;
using Chameleon.Authorization;
using Chameleon.Common.Helpers;
using Chameleon.Core.Settings;
using Chameleon.Core.Util;
using Chameleon.CT.Common.Base;
using Chameleon.Domain.Entities;
using Chameleon.Infrastructure.Users;
using Chameleon.Interfaces.App.Assistants.Events;
using Chameleon.Interfaces.App.Synchronization.Events;
using Chameleon.Interfaces.App.UserProfileFolders.Events;
using Chameleon.Interfaces.App.UserProfiles;
using Chameleon.Interfaces.Auth;
using Chameleon.Interfaces.Common;
using Chameleon.Interfaces.Services;
using Chameleon.Interfaces.UserProfileFolders;
using Chameleon.Interfaces.UserProfiles;
using Chameleon.Prism.Events;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Chameleon.Avalonia.Controls.UserProfilesView.ViewModels;

public partial class ProjectsViewModel : PageViewModelBase,
    IProjectsViewModel
{
    //ObservableCollection<ProfileViewModel> ProfileViewModels { get; set; }
    //ObservableCollection<DirectoryViewModel> DirectoryViewModels { get; set; }
    private readonly IUserAssistantService _userAssistantService;
    private readonly IApplicationUser _applicationUser;
    //TODO: private readonly IFeatureTourNavigator _featureTourNavigator;
    private readonly IAuthSession _authSession;
    private readonly IUserProfileFoldersViewModel folders;
    private readonly IUserProfilesViewModel profiles;


    private string name = "Profiles";
    public string Name
    {
        get { return name; }
        set { SetProperty(ref name, value); }
    }

    private int sIListView = 1;
    public int SIListView
    {
        get { return sIListView; }
        set
        {
            if (SetProperty(ref sIListView, value))
            {
                switch (value)
                {
                    case 0:
                        ListViewVisible = false;
                        break;

                    case 1:
                        ListViewVisible = true;
                        break;

                    default:
                        break;
                }
            }
        }
    }

    private bool listviewVisibl = true;
    public bool ListViewVisible
    {
        get { return listviewVisibl; }
        set { SetProperty(ref listviewVisibl, value); }
    }               

   // public string Directory { get { return FileStorageUtil.GetBaseProjectsDir(); } }

    private bool _isCreateProfileBtnVisible = true;
    public bool IsCreateProfileBtnVisible
    {
        get => _isCreateProfileBtnVisible;
        set => SetProperty(ref _isCreateProfileBtnVisible, value);
    }

    public ProjectsViewModel(IUserAssistantService userAssistantService,
        IApplicationUser applicationUser,
        IAuthSession authSession,
        IUserProfileFoldersViewModel folders,
        IUserProfilesViewModel profiles)
    {
        _userAssistantService = userAssistantService;
        _applicationUser = applicationUser;
        _authSession = authSession;
        this.folders = folders;
        this.profiles = profiles;

        EventAggregator
             .GetEvent<RestrictContentEvent>()
             .Subscribe(args => IsCreateProfileBtnVisible = args.Permissions.Contains(PermissionNames.Pages_CreateProfiles)                                                           
             && (!_applicationUser.IsAssistant || _authSession.CanCreateProfiles));

        EventAggregator
            .GetEvent<LoginSuccessEvent>()
            .SubscribeOnce(OnAuthenticated);

        EventAggregator
            .GetEvent<SavedUserAssistantEvent>()
            .Subscribe(args => SyncBtnVisibilityChange());

        EventAggregator
            .GetEvent<DeletedUserAssistantEvent>()
            .Subscribe(args => SyncBtnVisibilityChange());

        //_featureTourNavigator = FeatureTour.GetNavigator();

        //_featureTourNavigator.ForStep(ElementID.CreateProfileBtn).AttachDoable(
        //            currentStep => OnCreateProfile());
    }

    public override async Task OnNavigatedToAsync(object? param)
    {
        await base.OnNavigatedToAsync(param);

        if (param is IUserProfileFolder folder)
        {
            ////TODO: wtf
            //await Task.Delay(500);
            //EventAggregator
            //    .GetEvent<OpenUserProfileFolderEvent>()
            //    .Publish(new UserProfileFolderEventArgs(folder));

            if (!folder.Navigated || folders is UserProfileFoldersViewModel f && f.SelectedFolder.UserProfileFolder.Id == folder.Id)
            {
                folders.OnNavigatingTo(folder);
                folder.Navigated = true;
            }
        }
        else if(param is IUserProfile up)
        {
            if (!up.Navigated)
            {
                profiles.OnFilterTo(up);
                up.Navigated = true;
            }
        }
        else
        {
            if(folders is UserProfileFoldersViewModel f && f.SelectedFolder != null)
                folders.OnNavigatingTo(f.SelectedFolder.UserProfileFolder);
            else
                folders.OnNavigatingTo(null);
        }
    }

    public override Task InitAsync(object? param)
    {
        return base.InitAsync(param);
    }

    public bool IsDisabledCreateNewProfile = false;
    [RelayCommand]
    private async Task CreateProfile()
    {
        if (IsDisabledCreateNewProfile)
        {
            return;
        }
        //TODO:
        IsDisabledCreateNewProfile = true;
        var filter = profiles.Filter;
        try
        {
            var p = await profiles.CreateNewProfile();
            profiles.Filter = profile => p.Id == profile.Id;
            NavigationService.NavigateToType(typeof(IUserProfileIdentityView), p);

            EventAggregator.Push<ChangeProfilesInFavoriteFolderEvent, ChangeProfilesInFavoriteFolderEventArgs>(new ChangeProfilesInFavoriteFolderEventArgs(p.FolderId ?? 0, false, p));
        }
        catch (Exception ex)
        {
            if (ex.Message == "limit_ex")
            {
                if (await MesageBoxHelper.ShowAsync("PROFILES LIMIT REACHED", "You have reached the maximum number of profiles."))
                    ProcessesUtil.GoToUrlDefault(GlobalSettings.PricingUrl);
            }
            else
            {
                await MesageBoxHelper.ShowErrorAsync("Wooopsy?", ex.Message);
            }
        }
        finally
        {
            profiles.Filter = filter;
            IsDisabledCreateNewProfile = false;
        }
    }

    [RelayCommand]
    private void SyncChanges()
    {
        EventAggregator
            .GetEvent<SyncChangesEvent>()
            .Publish();
    }

    private void SyncBtnVisibilityChange()
    {
        OnPropertyChanged(nameof(IsSyncChangesBtnVisible));
    }

    private bool HasAssistants()
    {
        return _applicationUser.IsAuthenticated && _userAssistantService.Get().Count > 0;
    }
    public bool IsSyncChangesBtnVisible => _applicationUser.IsAssistant || HasAssistants();

    public void OnAuthenticated()
    {
        SyncBtnVisibilityChange();
    }
}
