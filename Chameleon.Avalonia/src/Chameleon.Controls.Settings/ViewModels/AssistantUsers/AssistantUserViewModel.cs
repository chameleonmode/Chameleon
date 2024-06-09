using Chameleon.CT.Common.Base;
using Chameleon.Controls.AssistantUsers.Interfaces;
using Chameleon.Core.Collections;
using Chameleon.Core.Collections.Views;
using Chameleon.Domain.Entities;
using Chameleon.Domain.Entities.Assistants;
using Chameleon.Infrastructure.Users;
using Chameleon.Interfaces.App.Assistants;
using Chameleon.Interfaces.App.Assistants.Events;
using Chameleon.Interfaces.App.ShareFolders;
using Chameleon.Interfaces.App.Users.AssistantUser.Events;
using Chameleon.Interfaces.Assistants;
using Chameleon.Interfaces.Dialogs;
using Chameleon.Interfaces.UserProfiles;
using Chameleon.Prism.Events;
using CommunityToolkit.Mvvm.Input;
using Chameleon.Common.Helpers;
using Chameleon.Avalonia.Common.Helpers;
using Chameleon.Avalonia.Common.Services;

namespace Chameleon.Avalonia.Controls.Settings.ViewModels.AssistantUsers;

public partial class AssistantUserViewModel
       : SubPageViewModelBase
{
    private const string _unshareProfileDialogTitle = "Unshare Profile";
    private const string _unshareFolderDialogTitle = "Unshare Folder";
    private const string _deleteUserDialogTitle = "Delete User";

    private readonly IUserAssistantService _userAssistantService;
    private readonly IEventAggregator _eventAggregator;
    //private readonly IUnshareItemPopupView _unshareProfilePopupView;
    //private readonly IDialogWindowsService _dialogWindowsService;
    //private readonly IInviteUserOrAddProfilesPopupService _inviteUserOrAddProfilesPopupService;
    //private readonly IDeleteAssistantUserPopupView _deleteAssistantUserPopupView;
    private readonly IShareFoldersService _shareFoldersService;
    private readonly IToastNotificationService _toastNotificationService;
    private readonly IUserProfileService _userProfileService;

    private ObservableCollection<IAssistantProfile, AssistantUserProfileViewModel> _profileMapping;
    private ObservableCollection<IShareFolder, AssistantUserFolderViewModel> _folderMapping;

    public AssistantUserViewModel(
        IUserAssistant userAssistant,
        IUserAssistantService userAssistantService,
        IEventAggregator eventAggregator,
        //IUnshareItemPopupView unshareProfilePopupView,
        //IDialogWindowsService dialogWindowsService,
        //IInviteUserOrAddProfilesPopupService inviteUserOrAddProfilesPopupService,
        //IDeleteAssistantUserPopupView deleteAssistantUserPopupView,
        IShareFoldersService shareFoldersService,
        IToastNotificationService toastNotificationService,
        IUserProfileService userProfileService
        )
    {
        _userAssistantService = userAssistantService;
        _eventAggregator = eventAggregator;
       // _unshareProfilePopupView = unshareProfilePopupView;
        //_dialogWindowsService = dialogWindowsService;
       // _inviteUserOrAddProfilesPopupService = inviteUserOrAddProfilesPopupService;
       // _deleteAssistantUserPopupView = deleteAssistantUserPopupView;
        _shareFoldersService = shareFoldersService;
        _toastNotificationService = toastNotificationService;
        _userProfileService = userProfileService;

        UserAssistant = userAssistant;

        InitProfiles();
        InitFolders();

        _eventAggregator
            .GetEvent<UnshareProfileEvent>()
            .Subscribe(args => OnUnshareProfile(args));

        _eventAggregator
            .GetEvent<AddProfilesEvent>()
            .Subscribe(args => OnAddProfiles(args));

        _eventAggregator
            .GetEvent<UnshareFolderEvent>()
            .Subscribe(args => OnUnshareFolder(args));
    }

    #region Properties

    public IUserAssistant UserAssistant { get; }

    private bool _isOpenPopup;
    public bool IsOpenPopup
    {
        get => _isOpenPopup;
        set => SetProperty(ref _isOpenPopup, value);
    }
    public long Id
    {
        get => UserAssistant.Id;
        set => UserAssistant.Id = value;
    }
    public string Username
    {
        get => UserAssistant.UserName;
        set => UserAssistant.UserName = value;
    }
    public string Email
    {
        get => UserAssistant.EmailAddress;
        set => UserAssistant.EmailAddress = value;
    }
    public bool CanCreateProfiles
    {
        get => UserAssistant.CanCreateProfiles;
        set => UserAssistant.CanCreateProfiles = value;
    }

    private ObservableCollectionView<AssistantUserProfileViewModel> _profileViewModels;
    public ObservableCollectionView<AssistantUserProfileViewModel> ProfileViewModels
    {
        get
        {
            if (_profileViewModels == null && _profileMapping != null)
            {
                _profileViewModels = new ObservableCollectionView<AssistantUserProfileViewModel>(_profileMapping);
            }
            return _profileViewModels;
        }
    }

    private ObservableCollectionView<AssistantUserFolderViewModel> _folderViewModels;
    public ObservableCollectionView<AssistantUserFolderViewModel> FolderViewModels
    {
        get
        {
            if (_folderViewModels == null && _folderMapping != null)
            {
                _folderViewModels = new ObservableCollectionView<AssistantUserFolderViewModel>(_folderMapping);
            }
            return _folderViewModels;
        }
    }

    #endregion


    #region Methods

    private void InitProfiles()
    {
        var profiles = _userAssistantService.GetAllAssistantProfilesById(UserAssistant.Id);

        _profileMapping = new ObservableCollection<IAssistantProfile, AssistantUserProfileViewModel>(
            profiles, profile => new AssistantUserProfileViewModel(
                _eventAggregator,
                _userAssistantService,
                _toastNotificationService,
                _userProfileService,
                profile));
    }
    private void InitFolders()
    {
        var folders = _shareFoldersService.GetAll(UserAssistant.Id);

        _folderMapping = new ObservableCollection<IShareFolder, AssistantUserFolderViewModel>(
            folders, folder => new AssistantUserFolderViewModel(
                _eventAggregator,
                _shareFoldersService,
                _toastNotificationService,
                folder));
    }
    //TODO:
    //private async Task<ButtonResult> InitUnsharePopupAndGetResult(string title, string text)
    //{
    //    Action<IUnshareItemPopupViewModel> initialize = viewModel =>
    //    {
    //        viewModel.Text = text;
    //    };

    //    return (ButtonResult)await _dialogWindowsService.ShowDialogWindow(_unshareProfilePopupView, title, initialize);
    //}
    [RelayCommand]
    private void OpenPopup()
    {
        IsOpenPopup = !IsOpenPopup;
    }
  
    [RelayCommand]
    private async Task DeleteAssistant()
    {
        if (await MesageBoxHelper.ShowAsync(_deleteUserDialogTitle, $"Are you sure you want to delete {Username}", fontIconInfo: "Delete"))
            try
            {
                _userAssistantService.DeleteAssistant(UserAssistant);
            }
            catch
            {
                _toastNotificationService.ShowError($"Failed to delete {UserAssistant.UserName}. Please try again.");
            }
    }

    [RelayCommand]
    private void AddMoreProfiles()
    {
        ContentDialogService.ShowAsync<IInviteUserOrAddProfilesView, IInviteUserOrAddProfilesViewModel>(
            viewModel =>
            {
                viewModel.Title = "Add Profiles";
                viewModel.TitleText = "Add access to specific Profiles or whole Folders for this user";
                viewModel.ShowInviteinfo = false;
                viewModel.AssistantId = UserAssistant.Id;
            });
    }

    [RelayCommand]
    private async Task SendLicenceKey()
    {
        await ClipboardService.Instance.SetTextAsync($"{UserAssistant.EmailAddress} {UserAssistant.UserName} {UserAssistant.Password}");
    }
    private void AddFolders(AddProfilesEventArgs args)
    {
        var folderIds = args.Folders?
            .Select(a => a.Id)
            .ToList();

        if (folderIds == null || folderIds?.Count == 0)
        {
            return;
        }

        try
        {
            var newFolders = _shareFoldersService.Share(args.AssistantId, folderIds, args.FolderPermissionIds);

            foreach (var newFolder in newFolders)
            {
                var newVm = new AssistantUserFolderViewModel(
                    _eventAggregator,
                    _shareFoldersService,
                    _toastNotificationService,
                    newFolder
                    );

                FolderViewModels.Add(newVm);
            }

            _toastNotificationService.ShowSuccess($"{folderIds.Count} folder(s) shared successfully");
        }
        catch
        {
            _toastNotificationService.ShowError($"Failed to share folder(s). Please try again.");
        }
    }
    private void AddProfiles(AddProfilesEventArgs args)
    {
        var folderIds = args.Folders?
            .Select(a => a.Id)
            .ToList();

        var profileIds = args.Profiles?
            .Select(a => a.Id)
            .ToList();

        if (profileIds == null || profileIds?.Count == 0)
        {
            return;
        }

        try
        {
            _userAssistantService.AddProfiles(new UserAssistant
            {
                Id = args.AssistantId,
                ProfileIds = profileIds,
                ProfilePermissionIds = args.ProfilePermissionIds,
                FolderIds = folderIds,
                FolderPermissionIds = args.FolderPermissionIds
            });

            foreach (var profile in args.Profiles)
            {
                var assistantProfile = new AssistantProfile()
                {
                    Id = args.AssistantId,
                    ProfileId = profile.Id,
                    ProfileName = profile.Title
                };

                var newProfile = new AssistantUserProfileViewModel(
                    _eventAggregator,
                    _userAssistantService,
                    _toastNotificationService,
                    _userProfileService,
                    assistantProfile);

                ProfileViewModels.Add(newProfile);
            }

            _toastNotificationService.ShowSuccess($"{profileIds.Count} profile(s) shared successfully");
        }
        catch
        {
            _toastNotificationService.ShowError($"Failed to share profile(s). Please try again.");
        }
    }
    private void OnAddProfiles(AddProfilesEventArgs args)
    {
        if (UserAssistant.Id != args.AssistantId)
        {
            return;
        }

        AddProfiles(args);
        AddFolders(args);
    }
    private async void OnUnshareProfile(UnshareProfileEventArgs args)
    {
        if (UserAssistant.Id != args.AssistantProfile.Id)
        {
            return;
        }

        if (await MesageBoxHelper.ShowAsync(_unshareProfileDialogTitle, $"Are you sure you want to unshare {args.AssistantProfile.ProfileName}? This will not affect other profiles."))
            try
            {
                _userAssistantService.DeleteAssistantProfile(args.AssistantProfile);

                var viewModelToDelete = ProfileViewModels
                    .Single(v => v.AssistantProfile.ProfileId == args.AssistantProfile.ProfileId);

                ProfileViewModels.Remove(viewModelToDelete);

                _toastNotificationService.ShowSuccess($"{args.AssistantProfile.ProfileName} was unshared successfully");
            }
            catch
            {
                _toastNotificationService.ShowError($"Failed to unshare profile. Please try again.");
            }
    }
    private async void OnUnshareFolder(UnshareFolderEventArgs args)
    {
        if (UserAssistant.Id != args.AssistantFolder.UserId)
        {
            return;
        }

        if (await MesageBoxHelper.ShowAsync(_unshareFolderDialogTitle, $"Are you sure you want to unshare {args.AssistantFolder.FolderName}? This will not affect other folders."))
            try
            {
                _shareFoldersService.Delete(args.AssistantFolder.Id);

                var viewModelToDelete = FolderViewModels
                       .Single(v => v.ShareFolder.FolderId == args.AssistantFolder.FolderId);

                FolderViewModels.Remove(viewModelToDelete);

                _toastNotificationService.ShowSuccess($"{args.AssistantFolder.FolderName} was unshared successfully");
            }
            catch
            {
                _toastNotificationService.ShowError($"Failed to unshare folder. Please try again.");
            }
    }

    [RelayCommand]
    private void SetCanCreateProfiles()
    {
        try
        {
            _userAssistantService.SetCanCreateProfiles(Id, CanCreateProfiles);

            _toastNotificationService.ShowSuccess($"Permission to create profiles was successfully {(CanCreateProfiles ? "given" : "taken")}");
        }
        catch
        {
            _toastNotificationService.ShowError($"Create profiles permission update failed. Please try again.");
        }
    }

    #endregion
}
