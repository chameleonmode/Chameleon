using Chameleon.Authorization;

using Chameleon.CT.Common.Base;
using Chameleon.Infrastructure.Users;
using Chameleon.Interfaces.App.ContentDiscoverey;
using Chameleon.Interfaces.App.UserProfiles;
using Chameleon.Interfaces.Auth;
using Chameleon.Interfaces.Common;
using Chameleon.Interfaces.SidePanel;
using Chameleon.Interfaces.UserProfiles;
using Chameleon.Prism.Events;
using CommunityToolkit.Mvvm.Input;

namespace Chameleon.Avalonia.Controls.UserProfileView.ViewModels;

public partial class UserProfileViewModel
       : SubPageViewModelBase
       , IUserProfileViewModel
{
    private readonly IUserAssistantService _userAssistantService;
    private readonly IAuthSession _authSession;
    //TODO: private readonly IFeatureTourNavigator _featureTourNavigator;
    private readonly IUserProfileService _userProfileService;

    // mapping from UI index into view type
    private readonly IList<UserProfileViewTab> TabIndexes = new List<UserProfileViewTab>(new[]
    {
            UserProfileViewTab.Details,
            UserProfileViewTab.Browser,
            //UserProfileViewTab.ContentDiscoverey,
            //UserProfileViewTab.OutReach,
            //UserProfileViewTab.YoutubeUploader,
            //UserProfileViewTab.Publishub
        });

    public UserProfileViewModel(
        IUserAssistantService userAssistantService,
        IAuthSession authSession,
        IUserProfileService userProfileService
        )
    {
        _userAssistantService = userAssistantService;
        _authSession = authSession;
        //_featureTourNavigator = FeatureTour.GetNavigator();
        _userProfileService = userProfileService;

        //EventAggregator
        //    .GetEvent<LoginSuccessEvent>()
        //    .Subscribe(args => OnAuthentication(args.Session));

        EventAggregator
            .GetEvent<SavedUserProfileEvent>()
            .Subscribe(_ => UpdateBreadcrumbs());

        //_featureTourNavigator.ForStep(ElementID.OpenBrowserTab).AttachDoable(
        //           currentStep => Tab = UserProfileViewTab.Browser);
    } 
    //public override Task InitAsync(object? param)
    //{
    //    OnAuthentication(_authSession);
    //    return base.InitAsync(param);
    //}
    public override async Task InitAsync(object? param)
    {
        await base.InitAsync(param);

        if (!Loaded)
            OnAuthentication(_authSession);
    }

    public override async Task OnNavigatedToAsync(object? param)
    {
        await base.OnNavigatedToAsync(param);

        UserProfile = param as IUserProfile;
    }

    //private BreadcrumbsViewModel _breadcrumbsViewModel;
    //public BreadcrumbsViewModel BreadcrumbsViewModel
    //{
    //    get
    //    {
    //        if (_breadcrumbsViewModel == null)
    //        {
    //            var root = new BreadcrumbViewModel
    //            {
    //                Title = _titlePage,
    //                IsBold = true,
    //                HasContinuation = true
    //            };

    //            var profileCrumb = new BreadcrumbViewModel()
    //            {
    //                Title = UserProfile?.Title
    //            };

    //            _breadcrumbsViewModel = new BreadcrumbsViewModel();
    //            _breadcrumbsViewModel.Breadcrumbs.Add(root);
    //            _breadcrumbsViewModel.Breadcrumbs.Add(profileCrumb);
    //        }

    //        return _breadcrumbsViewModel;
    //    }
    //}

    private bool _isTabNavigationEnabled = true;
    public bool IsTabNavigationEnabled
    {
        get => _isTabNavigationEnabled;
        set => SetProperty(ref _isTabNavigationEnabled, value);
    }

    private IUserProfile _userProfile;
    public IUserProfile UserProfile
    {
        get => _userProfile;
        set
        {
            SetProperty(ref _userProfile, value);

            UpdateBreadcrumbs();

            if (_authSession.CreatorUserId.HasValue && _userProfileService.IsSharedProfile(_userProfile))
            {
                UpdateProfilePermissions(_userProfile.Id);
            }
            else
            {
                UpdateTabsVisibility();
            }
        }
    }

    private void UpdateBreadcrumbs()
    {
        //var profileCrumb = BreadcrumbsViewModel.Breadcrumbs.LastOrDefault();
        //profileCrumb.Title = UserProfile?.Title;
    }

    private UserProfileViewTab _tab;
    public UserProfileViewTab Tab
    {
        get => _tab;
        set
        {
            if (SetProperty(ref _tab, value))
            {
                SelectedTabIndex = TabIndexes.IndexOf(value);

                EventAggregator
                    .GetEvent<HideSidePanelEvent>()
                    .Publish();

                if (value == UserProfileViewTab.Browser)
                {
                   // _featureTourNavigator.IfCurrentStepEquals(ElementID.OpenBrowserTab).GoNext();
                }
            }

            OnPropertyChanged(nameof(IsChameleonBrowserOpen));
            OnPropertyChanged(nameof(IsProfileDetailsOpen));
        }
    }

    private int _selectedTabIndex;
    public int SelectedTabIndex
    {
        get => _selectedTabIndex;
        set
        {
            if (SetProperty(ref _selectedTabIndex, value))
            {
                Tab = TabIndexes[value];
            }
        }
    }

    private bool _isOutreachIsVisible;
    public bool IsOutreachVisible
    {
        get => _isOutreachIsVisible;
        set => SetProperty(ref _isOutreachIsVisible, value);
    }

    private bool _isCurateVisible;
    public bool IsCurateVisible
    {
        get => _isCurateVisible;
        set => SetProperty(ref _isCurateVisible, value);
    }

    private bool _isYoutubeUploaderVisible;
    public bool IsYoutubeUploaderVisible
    {
        get => _isYoutubeUploaderVisible;
        set => SetProperty(ref _isYoutubeUploaderVisible, value);
    }

    private bool _isProspectorVisible;
    public bool IsProspectorVisible
    {
        get => _isProspectorVisible;
        set => SetProperty(ref _isProspectorVisible, value);
    }

    private bool _isRssVisible;
    public bool IsRssVisible
    {
        get => _isRssVisible;
        set => SetProperty(ref _isRssVisible, value);
    }

    public bool IsProfileDetailsOpen => _tab == UserProfileViewTab.Details;
    public bool IsChameleonBrowserOpen => _tab == UserProfileViewTab.Browser;

    private void OnAuthentication(IAuthSession authSession)
    {
        SetTabsVisibility(authSession.Limits);

        EventAggregator
            .GetEvent<RestrictContentEvent>()
            .Publish(new RestrictContentEventArgs(authSession.Limits, authSession.Permissions));

        EventAggregator
            .GetEvent<RestrictContentDiscoveryTabsEvent>()
            .Publish(new RestrictContentDiscoveryTabsEventArgs(IsProspectorVisible, IsRssVisible));
    }

    private void SetTabsVisibility(ILimits limits)
    {
        IsOutreachVisible = limits.HasOutreach;
        IsCurateVisible = limits.HasWordPress;
        IsYoutubeUploaderVisible = limits.HasWordPress;
        IsRssVisible = limits.ContentDiscoveryLimits.MaxRssCount > 0;
        IsProspectorVisible = limits.ContentDiscoveryLimits.HasProspector;
    }

    private void UpdateProfilePermissions(int profieId)
    {
        var profilePermissions = _userAssistantService.GetAllProfilePermissions(_authSession.UserId, profieId);

        var grantedPermissions = profilePermissions
            .Where(p => p.IsGranted)
            .Select(p => p.PermissionName)
            .ToList();

        UpdateTabsVisibility(grantedPermissions);
    }


    private void UpdateTabsVisibility(IList<string> grantedPermissions = null)
    {
        IsOutreachVisible = _authSession.Limits.HasOutreach
            && (grantedPermissions?.Contains(PermissionNames.Pages_Outreach) ?? true);

        IsCurateVisible = _authSession.Limits.HasWordPress
            && (grantedPermissions?.Contains(PermissionNames.Pages_Curate) ?? true);

        IsYoutubeUploaderVisible = _authSession.Limits.HasYouTube
            && (grantedPermissions?.Contains(PermissionNames.Pages_YouTube) ?? true);

        IsRssVisible = _authSession.Limits.ContentDiscoveryLimits.MaxRssCount > 0
            && (grantedPermissions?.Contains(PermissionNames.Pages_RSS) ?? true);

        IsProspectorVisible = _authSession.Limits.ContentDiscoveryLimits.HasProspector
            && (grantedPermissions?.Contains(PermissionNames.Pages_Prospector) ?? true);

        EventAggregator
            .GetEvent<RestrictContentDiscoveryTabsEvent>()
            .Publish(new RestrictContentDiscoveryTabsEventArgs(IsProspectorVisible, IsRssVisible));
    }

    [RelayCommand]
    private void OpenProfileDetails()
    {
        Tab = UserProfileViewTab.Details;
    }
    [RelayCommand]
    private void OpenChameleonBrowser()
    {
        Tab = UserProfileViewTab.Browser;
    }
}
