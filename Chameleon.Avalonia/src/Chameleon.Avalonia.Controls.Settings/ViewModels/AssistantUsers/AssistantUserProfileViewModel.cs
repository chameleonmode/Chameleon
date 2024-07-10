using Chameleon.Authorization;
using Chameleon.CT.Common.Base;
using Chameleon.Core.Collections;
using Chameleon.Core.Collections.Views;
using Chameleon.Infrastructure.Users;
using Chameleon.Interfaces.App.Assistants;
using Chameleon.Interfaces.App.Assistants.Events;
using Chameleon.Interfaces.App.Synchronization.Events;
using Chameleon.Interfaces.Assistants;
using Chameleon.Interfaces.Dialogs;
using Chameleon.Interfaces.UserProfiles;
using Chameleon.Prism.Events;
using CommunityToolkit.Mvvm.Input;

namespace Chameleon.Avalonia.Controls.Settings.ViewModels.AssistantUsers;

public partial class AssistantUserProfileViewModel
        : AssistantViewModelBase
{
    private readonly IEventAggregator _eventAggregator;
    private readonly IUserAssistantService _userAssistantService;
    private readonly IToastNotificationService _toastNotificationService;
    private readonly IUserProfileService _userProfileService;

    private ObservableCollection<IAssistantProfilePermission, AssistantProfilePermissionViewModel> _permissionMapping;

    public AssistantUserProfileViewModel(
        IEventAggregator eventAggregator,
        IUserAssistantService userAssistantService,
        IToastNotificationService toastNotificationService,
        IUserProfileService userProfileService,
        IAssistantProfile assistantProfile
        )
    {
        _eventAggregator = eventAggregator;
        _userAssistantService = userAssistantService;
        _toastNotificationService = toastNotificationService;
        _userProfileService = userProfileService;

        AssistantProfile = assistantProfile;

        InitPermissions();

        _eventAggregator
            .GetEvent<UpdateStaleDataEvent>()
            .Subscribe(RefreshTitle);
        _eventAggregator
            .GetEvent<SavedUserProfileEvent>()
            .Subscribe(args => RefreshTitle());
    }

    private void RefreshTitle()
    {
        var profile = _userProfileService.Get(AssistantProfile.ProfileId);
        AssistantProfile.ProfileName = profile.Title;
        OnPropertyChanged(nameof(Name));
    }

    public IAssistantProfile AssistantProfile { get; }
    public override string Name => AssistantProfile.ProfileName;

    private ObservableCollectionView<AssistantProfilePermissionViewModel> _permissionViewModels;
    public ObservableCollectionView<AssistantProfilePermissionViewModel> PermissionViewModels
    {
        get
        {
            if (_permissionViewModels == null && _permissionMapping != null)
            {
                _permissionViewModels = new ObservableCollectionView<AssistantProfilePermissionViewModel>(_permissionMapping);
            }
            return _permissionViewModels;
        }
        set => SetProperty(ref _permissionViewModels, value);
    }

    public AssistantProfilePermissionViewModel Outreach =>
        PermissionViewModels
            .Single(a => a.PermissionName == PermissionNames.Pages_Outreach);

    public AssistantProfilePermissionViewModel Prospector =>
        PermissionViewModels
            .Single(a => a.PermissionName == PermissionNames.Pages_Prospector);

    public AssistantProfilePermissionViewModel YouTube =>
        PermissionViewModels
            .Single(a => a.PermissionName == PermissionNames.Pages_YouTube);

    public AssistantProfilePermissionViewModel RSS =>
        PermissionViewModels
            .Single(a => a.PermissionName == PermissionNames.Pages_RSS);

    public AssistantProfilePermissionViewModel Curate =>
        PermissionViewModels
            .Single(a => a.PermissionName == PermissionNames.Pages_Curate);


    public override void Unshare()
    {
        _eventAggregator
            .GetEvent<UnshareProfileEvent>()
            .Publish(new UnshareProfileEventArgs(AssistantProfile));

       base.Unshare();
    }

    private void InitPermissions()
    {
        var permissions = _userAssistantService.GetAllProfilePermissions(AssistantProfile.Id, AssistantProfile.ProfileId);

        _permissionMapping = new ObservableCollection<IAssistantProfilePermission, AssistantProfilePermissionViewModel>(
            permissions, permission => new AssistantProfilePermissionViewModel(
                _userAssistantService,
                _toastNotificationService,
                permission)
            );

        OnPropertyChanged(nameof(PermissionViewModels));
        OnPropertyChanged(nameof(Outreach));
        OnPropertyChanged(nameof(Prospector));
        OnPropertyChanged(nameof(YouTube));
        OnPropertyChanged(nameof(RSS));
        OnPropertyChanged(nameof(Curate));
    }
}
