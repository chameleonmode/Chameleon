using Chameleon.Common.Helpers;
using Chameleon.Controls.AssistantUsers.Interfaces;
using Chameleon.Core.Collections;
using Chameleon.Core.Collections.Views;
using Chameleon.CT.Common.Base;
using Chameleon.Infrastructure.Users;
using Chameleon.Interfaces.App.Assistants.Events;
using Chameleon.Interfaces.App.Permissions.ProfileFolderPermission;
using Chameleon.Interfaces.App.ShareFolders;
using Chameleon.Interfaces.App.Users.AssistantUser.Events;
using Chameleon.Interfaces.Dialogs;
using Chameleon.Interfaces.DialogWindows;
using Chameleon.Interfaces.UserProfileFolders;
using Chameleon.Interfaces.UserProfiles;
using Chameleon.Prism.Events;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentAvalonia.UI.Controls;
using System.ComponentModel;

namespace Chameleon.Avalonia.Controls.Settings.ViewModels;

public partial class ProfileViewModel
   : ObservableObject
{
    private readonly IEventAggregator EventAggregator;

    public ProfileViewModel(
        IUserProfile userProfile,
        IEventAggregator eventAggregator)
    {
        UserProfile = userProfile;
        EventAggregator = eventAggregator;
    }

    public IUserProfile UserProfile { get; }
    public string Title => UserProfile?.Title;

    public int? ProfileId => UserProfile?.Id;

    private bool _isSelected;
    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (SetProperty(ref _isSelected, value))
            {
                EventAggregator
                    .GetEvent<SelectedProfileChangeEvent>()
                    .Publish(/*new SelectedProfileChangeEventArgs(UserProfile, _isSelected)*/);
            }
        }
    }

    public char Code => string.IsNullOrWhiteSpace(Title) ? '0' : Title[0];

    [RelayCommand]
    private void Unselect()
    {
        IsSelected = false;
    }
}
public class ProfilePermissionViewModel
   : ObservableObject
{
    private readonly IEventAggregator _eventAggregator;
    public ProfilePermissionViewModel(
        IEventAggregator eventAggregator,
        IProfileFolderPermission permission
        )
    {
        _eventAggregator = eventAggregator;

        Permission = permission;
    }

    public IProfileFolderPermission Permission { get; }
    public string DisplayName => Permission.DisplayName;
    public int PermissionId => Permission.Id;

    private bool _IsChecked;
    public bool IsChecked
    {
        get => _IsChecked;
        set
        {
            if (SetProperty(ref _IsChecked, value))
            {
                _eventAggregator
                    .GetEvent<SelectedProfilePermissionChangeEvent>()
                    .Publish();
            }
        }
    }
}

public partial class FolderViewModel
   : ObservableObject
{
    private readonly IEventAggregator EventAggregator;

    public FolderViewModel(
        IUserProfileFolder userFolder,
        IEventAggregator eventAggregator)
    {
        EventAggregator = eventAggregator;
        UserFolder = userFolder;
    }

    public IUserProfileFolder UserFolder { get; }
    public string Title => UserFolder?.Title;
    public int? FolderId => UserFolder?.Id;

    private bool _isSelected;
    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (SetProperty(ref _isSelected, value))
            {
                EventAggregator
                    .GetEvent<SelectedFolderChangeEvent>()
                    .Publish(/*new SelectedFolderChangeEventArgs(UserFolder, _isSelected)*/);
            }
        }
    }

    public char Code => string.IsNullOrWhiteSpace(Title) ? '0' : Title[0];

    [RelayCommand]
    private void Unselect()
    {
        IsSelected = false;
    }
}
public class FolderPermissionViewModel
: ObservableObject
{
    private readonly IEventAggregator _eventAggregator;
    public FolderPermissionViewModel(
        IEventAggregator eventAggregator,
        IProfileFolderPermission permission
        )
    {
        _eventAggregator = eventAggregator;

        Permission = permission;
    }

    public IProfileFolderPermission Permission { get; }
    public string DisplayName => Permission.DisplayName;
    public int PermissionId => Permission.Id;

    private bool _IsChecked;
    public bool IsChecked
    {
        get => _IsChecked;
        set
        {
            if (SetProperty(ref _IsChecked, value))
            {
                _eventAggregator
                    .GetEvent<SelectedFolderPermissionChangeEvent>()
                    .Publish();
            }
        }
    }
}

public partial class InviteUserOrAddProfilesViewModel : ObservableObjectBase, 
    IInviteUserOrAddProfilesViewModel
{
    private readonly IUserProfileService _userProfileService;
    private readonly IUserAssistantService _userAssistantService;
    private readonly IUserProfileFolderService _userProfileFolderService;
    private readonly IShareFoldersService _shareFoldersService;
    private readonly IProfileFolderPermissionRepository _profileFolderPermissionRepository;

    //private readonly ErrorsViewModel _errorsViewModel;

    private ObservableCollection<IUserProfile, ProfileViewModel> _profileMapping;
    private ObservableCollection<IUserProfileFolder, FolderViewModel> _folderMapping;

    private IList<ProfilePermissionViewModel> _profilePermissionList;
    private IList<FolderPermissionViewModel> _folderPermissionList;

    private IList<int> _sharedProfileIds;
    private IList<int> _sharedFolderIds;

    public InviteUserOrAddProfilesViewModel(
        IUserProfileService userProfileService,
        IUserAssistantService userAssistantService,
        IUserProfileFolderService userProfileFolderService,
        IShareFoldersService shareFoldersService,
        IProfileFolderPermissionRepository profileFolderPermissionRepository)
    {
        _userProfileService = userProfileService;
        _userAssistantService = userAssistantService;
        _userProfileFolderService = userProfileFolderService;
        _shareFoldersService = shareFoldersService;
        _profileFolderPermissionRepository = profileFolderPermissionRepository;
    }
    public override async Task InitAsync(object? param)
    {
        await base.InitAsync(param);

        InitProfiles();
        InitProfilePermissionList();

        InitFolders();
        InitFolderPermissionList();

        OnPropertyChanged(string.Empty);

        EventAggregator
            .GetEvent<SelectedProfileChangeEvent>()
            .SubscribeOnce(OnSelectedProfileChange);

        EventAggregator
            .GetEvent<SelectedFolderChangeEvent>()
            .SubscribeOnce(OnSelectedFolderChange);

        EventAggregator
            .GetEvent<SelectedProfilePermissionChangeEvent>()
            .SubscribeOnce(OnSelectedProfilePermissionChange);

        EventAggregator
            .GetEvent<SelectedFolderPermissionChangeEvent>()
            .SubscribeOnce(OnSelectedFolderPermissionChange);
    }

    private long _assistantId;
    public long AssistantId
    {
        get => _assistantId;
        set
        {
            if (SetProperty(ref _assistantId, value))
            {
                if (_assistantId != 0)
                {
                    UpdateSharedProfileIds();
                    UpdateSharedFolderIds();
                }
            }
        }
    }

    private string _assistantName;
    public string AssistantName
    {
        get => _assistantName;
        set
        {
            if (SetProperty(ref _assistantName, value.Trim()))
            {
                OnPropertyChanged(nameof(CanSaveChangesBtn));
            }

            //_errorsViewModel.ClearErrors(nameof(AssistantName));

            //if (string.IsNullOrEmpty(_assistantName) || string.IsNullOrWhiteSpace(_assistantName))
            //{
            //    _errorsViewModel.AddError(nameof(AssistantName), "Field is required.");
            //}
        }
    }

    private string _assistantEmail;
    public string AssistantEmail
    {
        get => _assistantEmail;
        set
        {
            if (SetProperty(ref _assistantEmail, value.Trim()))
            {
                OnPropertyChanged(nameof(CanSaveChangesBtn));
            }

            //_errorsViewModel.ClearErrors(nameof(AssistantEmail));

            //if (string.IsNullOrEmpty(_assistantEmail) || string.IsNullOrWhiteSpace(_assistantEmail))
            //{
            //    _errorsViewModel.AddError(nameof(AssistantEmail), "Field is required.");
            //}
            //else if (!ValidatorExtensions.IsValidEmail(_assistantEmail))
            //{
            //    _errorsViewModel.AddError(nameof(AssistantEmail), "Email is invalid.");
            //}
        }
    }

    private bool _isProfileListVisibile;
    public bool IsProfileListVisibile
    {
        get => _isProfileListVisibile;
        set => SetProperty(ref _isProfileListVisibile, value);
    }

    private bool _isFolderListVisibile;
    public bool IsFolderListVisibile
    {
        get => _isFolderListVisibile;
        set => SetProperty(ref _isFolderListVisibile, value);
    }

    private bool _showInviteinfo;
    public bool ShowInviteinfo
    {
        get => _showInviteinfo;
        set => SetProperty(ref _showInviteinfo, value);
    }

    private string _titleText;
    public string TitleText
    {
        get => _titleText;
        set => SetProperty(ref _titleText, value);
    }

    private string _saveChangesBtnText;
    public string SaveChangesBtnText
    {
        get => _saveChangesBtnText;
        set => SetProperty(ref _saveChangesBtnText, value);
    }

    private string _serverErrorMessage;
    public string ServerErrorMessage
    {
        get => _serverErrorMessage;
        set
        {
            if (SetProperty(ref _serverErrorMessage, value))
            {
                OnPropertyChanged(nameof(HasServerError));
            }
        }
    }
    public bool HasServerError => !string.IsNullOrEmpty(ServerErrorMessage);
    public bool HasSelectedProfilesOrFolders => _selectedProfileViewModels?.Count() > 0 || _selectedFolderViewModels?.Count() > 0;
    public bool HasErrors => false;//TODO: _errorsViewModel.HasErrors;

    public bool CanSaveChangesBtn => !HasErrors && HasSelectedProfilesOrFolders &&
        (!ShowInviteinfo || (!string.IsNullOrEmpty(AssistantName) && !string.IsNullOrEmpty(AssistantEmail)));

    public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

    private IList<IProfileFolderPermission> _permissions;
    public IList<IProfileFolderPermission> Permissions
    {
        get
        {
            if (_permissions == null)
            {
                _permissions = _profileFolderPermissionRepository
                    .GetAll()
                    .Reverse()
                    .ToList();
            }
            return _permissions;
        }
    }

    private ObservableCollectionView<ProfileViewModel> _profileViewModels;
    public ObservableCollectionView<ProfileViewModel> ProfileViewModels
    {
        get
        {
            if (_profileViewModels == null && _profileMapping != null)
            {
                _profileViewModels = new ObservableCollectionView<ProfileViewModel>(_profileMapping)
                {
                    Order = profile => profile.Title,
                };
            }

            if (_profileViewModels != null && _sharedProfileIds != null && !_showInviteinfo)
            {
                _profileViewModels.Filter = (profileViewModel) =>
                    !_sharedProfileIds.Contains(profileViewModel.ProfileId.Value);
            }

            return _profileViewModels;
        }
    }

    private IEnumerable<ProfileViewModel> _selectedProfileViewModels;
    public IEnumerable<ProfileViewModel> SelectedProfileViewModels
    {
        get => _selectedProfileViewModels;
        set
        {
            if (SetProperty(ref _selectedProfileViewModels, value))
            {
                OnPropertyChanged(nameof(HasSelectedProfilesOrFolders));
                OnPropertyChanged(nameof(CanSaveChangesBtn));
            }
        }
    }

    private ObservableCollectionView<ProfilePermissionViewModel> _profilePermissionViewModels;
    public ObservableCollectionView<ProfilePermissionViewModel> ProfilePermissionViewModels
    {
        get
        {
            if (_profilePermissionViewModels == null && _profilePermissionList != null)
            {
                _profilePermissionViewModels = new ObservableCollectionView<ProfilePermissionViewModel>(_profilePermissionList);
            }

            return _profilePermissionViewModels;
        }
    }

    private IEnumerable<ProfilePermissionViewModel> _selectedProfilePermissionViewModels;
    public IEnumerable<ProfilePermissionViewModel> SelectedProfilePermissionViewModels
    {
        get => _selectedProfilePermissionViewModels;
        set => SetProperty(ref _selectedProfilePermissionViewModels, value);
    }

    private ObservableCollectionView<FolderViewModel> _folderViewModels;
    public ObservableCollectionView<FolderViewModel> FolderViewModels
    {
        get
        {
            if (_folderViewModels == null && _folderMapping != null)
            {
                _folderViewModels = new ObservableCollectionView<FolderViewModel>(_folderMapping)
                {
                    Order = folder => folder.Title
                };
            }

            if (_folderViewModels != null && _sharedFolderIds != null && !_showInviteinfo)
            {
                _folderViewModels.Filter = (folderViewModel) =>
                    !_sharedFolderIds.Contains(folderViewModel.FolderId.Value);
            }

            return _folderViewModels;
        }
    }

    private IEnumerable<FolderViewModel> _selectedFolderViewModels;
    public IEnumerable<FolderViewModel> SelectedFolderViewModels
    {
        get => _selectedFolderViewModels;
        set
        {
            if (SetProperty(ref _selectedFolderViewModels, value))
            {
                OnPropertyChanged(nameof(HasSelectedProfilesOrFolders));
                OnPropertyChanged(nameof(CanSaveChangesBtn));
            }
        }
    }

    private ObservableCollectionView<FolderPermissionViewModel> _folderPermissionViewModels;
    public ObservableCollectionView<FolderPermissionViewModel> FolderPermissionViewModels 
    {
        get
        {
            if (_folderPermissionViewModels == null && _folderPermissionList != null)
            {
                _folderPermissionViewModels = new ObservableCollectionView<FolderPermissionViewModel>(_folderPermissionList);
            }

            return _folderPermissionViewModels;
        }
    }

    private IEnumerable<FolderPermissionViewModel> _selectedFolderPermissionViewModels;
    public IEnumerable<FolderPermissionViewModel> SelectedFolderPermissionViewModels
    {
        get => _selectedFolderPermissionViewModels;
        set => SetProperty(ref _selectedFolderPermissionViewModels, value);
    }

    [RelayCommand]
    private void Discard()
    {
        //EventAggregator
        //    .GetEvent<CloseDialogWindowEvent>()
        //    .Publish(ButtonResult.Cancel);
    }

    private void SaveChanges()
    {
        var profiles = _selectedProfileViewModels?
            .Select(a => a.UserProfile)
            .ToList();

        var profilePermissionIds = _selectedProfilePermissionViewModels?
            .Select(a => a.PermissionId)
            .ToList();

        var folders = _selectedFolderViewModels?
            .Select(a => a.UserFolder)
            .ToList();

        var folderPermissionIds = _selectedFolderPermissionViewModels?
            .Select(a => a.PermissionId)
            .ToList();

        if (_assistantEmail == null && _assistantName == null)
        {
            AddProfiles(profiles, profilePermissionIds, folders, folderPermissionIds);
        }
        else
        {
            InviteUser(profiles, profilePermissionIds, folders, folderPermissionIds);
        }
    }

    private void InviteUser(IList<IUserProfile> profiles, IList<int> profilePermissionIds, IList<IUserProfileFolder> folders, IList<int> folderPermissionIds)
    {
        var profileIds = profiles?
            .Select(a => a.Id)
            .ToList();

        var folderIds = folders?
            .Select(a => a.Id)
            .ToList();

        ServerErrorMessage = string.Empty;

        try
        {
            var args = new InviteUserAssistantEventArgs
                (_assistantName, _assistantEmail, profileIds, profilePermissionIds, folderIds, folderPermissionIds);

            EventAggregator
                .GetEvent<InviteUserAssistantEvent>()
                .Publish(args);

            //EventAggregator
            //    .GetEvent<CloseDialogWindowEvent>()
            //    .Publish(ButtonResult.OK);
        }
        catch (Exception ex)
        {
            ServerErrorMessage = ex.Message;
        }
    }

    private void AddProfiles(IList<IUserProfile> profiles, IList<int> profilePermissionIds, IList<IUserProfileFolder> folders, IList<int> folderPermissionIds)
    {
        var args = new AddProfilesEventArgs
            (_assistantId, profiles, profilePermissionIds, folders, folderPermissionIds);

        EventAggregator
           .GetEvent<AddProfilesEvent>()
           .Publish(args);

        //EventAggregator
        //    .GetEvent<CloseDialogWindowEvent>()
        //    .Publish(ButtonResult.OK);
    }
    //FolderPermissionViewModels FolderViewModels ProfilePermissionViewModels ProfileViewModels
    private void InitProfiles()
    {
        var userProfiles = _userProfileService.GetAll();

        _profileMapping = new ObservableCollection<IUserProfile, ProfileViewModel>(
            userProfiles, userProfile => new ProfileViewModel(userProfile, EventAggregator)
            );

        OnPropertyChanged(nameof(ProfileViewModels));
    }

    private void InitFolders()
    {
        var userFolders = _userProfileFolderService.GetAll();

        _folderMapping = new ObservableCollection<IUserProfileFolder, FolderViewModel>(
            userFolders, userFolder => new FolderViewModel(userFolder, EventAggregator));

        OnPropertyChanged(nameof(FolderViewModels));
    }

    private void InitProfilePermissionList()
    {
        _profilePermissionList = Permissions
             .Select(permission => new ProfilePermissionViewModel
                (EventAggregator,
                 permission))
             .ToList();

        OnPropertyChanged(nameof(ProfilePermissionViewModels));
    }

    private void InitFolderPermissionList()
    {
        _folderPermissionList = Permissions
             .Select(permission => new FolderPermissionViewModel
                (EventAggregator,
                 permission))
             .ToList();

        OnPropertyChanged(nameof(FolderPermissionViewModels));
    }

    private void UpdateSharedProfileIds()
    {
        _sharedProfileIds = _userAssistantService.GetAllAssistantProfilesById(_assistantId)
            .Select(profile => profile.ProfileId)
            .ToList();

        OnPropertyChanged(nameof(ProfileViewModels));
    }

    private void UpdateSharedFolderIds()
    {
        _sharedFolderIds = _shareFoldersService.GetAll(_assistantId)
            .Select(folder => folder.FolderId)
            .ToList();

        OnPropertyChanged(nameof(FolderViewModels));
    }

    private void OnSelectedProfileChange()
    {
        SelectedProfileViewModels = ProfileViewModels
            .Where(profile => profile.IsSelected);
    }

    private void OnSelectedFolderChange()
    {
        SelectedFolderViewModels = FolderViewModels
            .Where(folder => folder.IsSelected);
    }

    private void OnSelectedProfilePermissionChange()
    {
        SelectedProfilePermissionViewModels = ProfilePermissionViewModels
             .Where(profilePermission => profilePermission.IsChecked);
    }

    private void OnSelectedFolderPermissionChange()
    {
        SelectedFolderPermissionViewModels = FolderPermissionViewModels
             .Where(folderPermission => folderPermission.IsChecked);
    }

    private void ErrorsViewModel_ErrorsChanged(object sender, DataErrorsChangedEventArgs e)
    {
        ErrorsChanged?.Invoke(this, e);
        OnPropertyChanged(nameof(HasErrors));
        OnPropertyChanged(nameof(CanSaveChangesBtn));
    }

    public void OnDialogClosing(IContentDialogResult result)
    {
        if (result == IContentDialogResult.Primary)
        {
            SaveChanges();
        }
        else
        {
            Discard();
        }
    }
}
