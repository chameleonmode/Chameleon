using Chameleon.Controls.AssistantUsers.Interfaces;
using Chameleon.Domain.Entities.Assistants;
using Chameleon.Infrastructure.Users;
using Chameleon.Interfaces.App.Assistants;
using Chameleon.Interfaces.App.Assistants.Events;
using Chameleon.Interfaces.App.ShareFolders;
using Chameleon.Interfaces.App.Users.AssistantUser.Events;
using Chameleon.Interfaces.Assistants;
using Chameleon.lib.Common.ServiceManagers;

namespace Chameleon.Avalonia.Controls.Settings.ViewModels.AssistantUsers;

public partial class AssistantUserViewModel(
        IUserAssistant userAssistant,
        IUserAssistantService userAssistantService,
        IShareFoldersService shareFoldersService,
        IUserProfileService userProfileService)
       : SubPageViewModelBase
{
    private const string _unshareProfileDialogTitle = "Unshare Profile";
    private const string _unshareFolderDialogTitle = "Unshare Folder";
    private const string _deleteUserDialogTitle = "Delete User";

    private ObservableCollection<IAssistantProfile, AssistantUserProfileViewModel> _profileMapping;
    private ObservableCollection<IShareFolder, AssistantUserFolderViewModel> _folderMapping;

    [ObservableProperty]
    private bool canCreateProfiles = userAssistant.CanCreateProfiles;

    public override async Task InitAsync(object? param)
    {
        await base.InitAsync(param);

        if (!Loaded)
        {
            await InitProfiles();
            await InitFolders();

            EventAggregator
                .GetEvent<UnshareProfileEvent>()
                .Subscribe(OnUnshareProfile);

            EventAggregator
                .GetEvent<AddProfilesEvent>()
                .Subscribe(OnAddProfiles);

            EventAggregator
                .GetEvent<UnshareFolderEvent>()
                .Subscribe(OnUnshareFolder);
        }
    }

    #region Properties
    public IUserAssistant UserAssistant => userAssistant;

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

    private async Task InitProfiles()
    {
        var profiles = await Task.Run(() => userAssistantService.GetAllAssistantProfilesById(UserAssistant.Id));

        _profileMapping = new ObservableCollection<IAssistantProfile, AssistantUserProfileViewModel>(
            profiles, profile => new AssistantUserProfileViewModel(
                EventAggregator,
                userAssistantService,
                userProfileService,
                profile));
    }
    private async Task InitFolders()
    {
        var folders = await Task.Run(()=> shareFoldersService.GetAll(UserAssistant.Id));

        _folderMapping = new ObservableCollection<IShareFolder, AssistantUserFolderViewModel>(
            folders, folder => new AssistantUserFolderViewModel(
                EventAggregator,
                shareFoldersService,
                folder));
    }
  
    [RelayCommand]
    private async Task DeleteAssistant()
    {
        if (await Mbox.ShowAsync(_deleteUserDialogTitle, $"Are you sure you want to delete {Username}", fontIconInfo: "Delete"))
            try
            {
                userAssistantService.DeleteAssistant(UserAssistant);
            }
            catch
            {
				Toaster.ShowErr($"Failed to delete {UserAssistant.UserName}. Please try again.");
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
        await CopyPasta.Copy($"{UserAssistant.EmailAddress} {UserAssistant.UserName} {UserAssistant.Password}");
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
            var newFolders = shareFoldersService.Share(args.AssistantId, folderIds, args.FolderPermissionIds);

            foreach (var newFolder in newFolders)
            {
                var newVm = new AssistantUserFolderViewModel(
                    EventAggregator,
                    shareFoldersService,
                    newFolder
                    );

                FolderViewModels.Add(newVm);
            }

			Toaster.ShowSuccess($"{folderIds.Count} folder(s) shared successfully");
        }
        catch
        {
			Toaster.ShowErr($"Failed to share folder(s). Please try again.");
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
            userAssistantService.AddProfiles(new UserAssistant
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
                    EventAggregator,
                    userAssistantService,
                    userProfileService,
                    assistantProfile);

                ProfileViewModels.Add(newProfile);
            }

			Toaster.ShowSuccess($"{profileIds.Count} profile(s) shared successfully");
        }
        catch
        {
			Toaster.ShowErr($"Failed to share profile(s). Please try again.");
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

        if (await Mbox.ShowAsync(_unshareProfileDialogTitle, $"Are you sure you want to unshare {args.AssistantProfile.ProfileName}? This will not affect other profiles."))
            try
            {
                userAssistantService.DeleteAssistantProfile(args.AssistantProfile);

                var viewModelToDelete = ProfileViewModels
                    .Single(v => v.AssistantProfile.ProfileId == args.AssistantProfile.ProfileId);

                ProfileViewModels.Remove(viewModelToDelete);

				Toaster.ShowSuccess($"{args.AssistantProfile.ProfileName} was unshared successfully");
            }
            catch
            {
				Toaster.ShowErr($"Failed to unshare profile. Please try again.");
            }
    }
    private async void OnUnshareFolder(UnshareFolderEventArgs args)
    {
        if (UserAssistant.Id != args.AssistantFolder.UserId)
        {
            return;
        }

        if (await Mbox.ShowAsync(_unshareFolderDialogTitle, $"Are you sure you want to unshare {args.AssistantFolder.FolderName}? This will not affect other folders."))
            try
            {
                shareFoldersService.Delete(args.AssistantFolder.Id);

                var viewModelToDelete = FolderViewModels
                       .Single(v => v.ShareFolder.FolderId == args.AssistantFolder.FolderId);

                FolderViewModels.Remove(viewModelToDelete);

				Toaster.ShowSuccess($"{args.AssistantFolder.FolderName} was unshared successfully");
            }
            catch
            {
				Toaster.ShowErr($"Failed to unshare folder. Please try again.");
            }
    }

    partial void OnCanCreateProfilesChanged(bool oldValue, bool newValue)
    {
        UserAssistant.CanCreateProfiles = newValue;
        SetCanCreateProfiles();
    }

    [RelayCommand]
    private void SetCanCreateProfiles()
    {
        try
        {
            userAssistantService.SetCanCreateProfiles(Id, CanCreateProfiles);

			Toaster.ShowSuccess($"Permission to create profiles was successfully {(CanCreateProfiles ? "given" : "taken")}");
        }
        catch
        {
			Toaster.ShowErr($"Create profiles permission update failed. Please try again.");
        }
    }

    #endregion
}
