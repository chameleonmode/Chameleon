using Chameleon.Avalonia.Controls.Paginator.ViewModels;
using Chameleon.Avalonia.Controls.Settings.ViewModels.AssistantUsers;
using Chameleon.CT.Common.Base;
using Chameleon.Controls.AssistantUsers.Interfaces;
using Chameleon.Core.Collections;
using Chameleon.Core.Collections.Views;
using Chameleon.Core.Util;
using Chameleon.Domain.Entities;
using Chameleon.Infrastructure.Users;
using Chameleon.Interfaces.App.Assistants;
using Chameleon.Interfaces.App.Assistants.Events;
using Chameleon.Interfaces.App.ShareFolders;
using Chameleon.Interfaces.Auth;
using Chameleon.Interfaces.Dialogs;
using Chameleon.Interfaces.DialogWindows;
using Chameleon.Interfaces.UpgradePlan;
using Chameleon.Interfaces.UserProfiles;
using Chameleon.Prism.Events;
using CommunityToolkit.Mvvm.Input;

namespace Chameleon.Avalonia.Controls.Settings.ViewModels;

public partial class AssistantUsersViewModel
       : SubPageViewModelBase
       , IAssistantUsersViewModel
{
    private readonly IUserAssistantService _userAssistantService;
    //private readonly IUnshareItemPopupView _unshareProfilePopupView;
    private readonly IDialogWindowsService _dialogWindowsService;
    //private readonly IInviteUserOrAddProfilesPopupService _inviteUserOrAddProfilesPopupService;
    // private readonly IDeleteAssistantUserPopupView _deleteAssistantUserPopupView;
    private readonly IAuthSession _authSession;
    private readonly IUpgradePlanPopupView _upgradePlanPopupView;
    private readonly IShareFoldersService _shareFoldersService;
    private readonly IToastNotificationService _toastNotificationService;
    private readonly IUserProfileService _userProfileService;

    private ObservableCollection<IUserAssistant, AssistantUserViewModel> _mapping;

    public AssistantUsersViewModel(
        IUserAssistantService userAssistantService,
        //IUnshareItemPopupView unshareProfilePopupView,
        //IDialogWindowsService dialogWindowsService,
        // IInviteUserOrAddProfilesPopupService inviteUserOrAddProfilesPopupService,
        //IDeleteAssistantUserPopupView deleteAssistantUserPopupView,
        IAuthSession authSession,
        IUpgradePlanPopupView upgradePlanPopupView,
        IShareFoldersService shareFoldersService,
        IToastNotificationService toastNotificationService,
        IUserProfileService userProfileService
        )
    {
        _userAssistantService = userAssistantService;
        //_unshareProfilePopupView = unshareProfilePopupView;
        //_dialogWindowsService = dialogWindowsService;
        // _inviteUserOrAddProfilesPopupService = inviteUserOrAddProfilesPopupService;
        // _deleteAssistantUserPopupView = deleteAssistantUserPopupView;
        _authSession = authSession;
        _upgradePlanPopupView = upgradePlanPopupView;
        _shareFoldersService = shareFoldersService;
        _toastNotificationService = toastNotificationService;
        _userProfileService = userProfileService;


    }
    public override async Task InitAsync(object? param)
    {
        EventAggregator.SetEventSubscription<InviteUserAssistantEvent, InviteUserAssistantEventArgs>(OnCreate);

        EventAggregator.SetEventSubscription<SavedUserAssistantEvent, SavedUserAssistantEventArgs>(OnUserAssistantSaved);
            //.GetEvent<SavedUserAssistantEvent>()
            //.Subscribe(args => OnUserAssistantSaved(args));

        EventAggregator
            .GetEvent<DeletedUserAssistantEvent>()
            .Subscribe(OnUserAssistantDeleted);
       
        await InitUserAssistantsAsync();
    }

    private int _totalCount;
    public int TotalCount
    {
        get => _totalCount;
        set => SetProperty(ref _totalCount, value);
    }

    private PaginatorViewModel _paginatorViewModel;
    public PaginatorViewModel PaginatorViewModel
    {
        get => _paginatorViewModel;
        set
        {
            if (SetProperty(ref _paginatorViewModel, value))
            {
                _paginatorViewModel.ChangePageIndex += OnChangePage;
            }
        }
    }

    private ObservableCollectionView<AssistantUserViewModel> _viewModels;
    public ObservableCollectionView<AssistantUserViewModel> ViewModels
    {
        get
        {
            if (_viewModels == null && _mapping != null)
            {
                _viewModels = new ObservableCollectionView<AssistantUserViewModel>(_mapping);

                _mapping.CollectionChanged += OnViewModelChange;
                InitPaginator();
            }

            return _viewModels;
        }
    }

    [RelayCommand]
    private void CreateNewUserAssistant()
    {
        if (_viewModels?.Items.Count >= _authSession.Limits.MaxAssistantsCount)
        {
            ShowOutOfLimitPopup();
        }
        else
        {
            //_inviteUserOrAddProfilesPopupService.ShowPopup(true);
        }
    }

    private async Task InitUserAssistantsAsync()
    {
        var assistants = await Task.Run(()=>_userAssistantService.Get()); 

        _mapping = new ObservableCollection<IUserAssistant, AssistantUserViewModel>(
            assistants, userAssistant =>
            new AssistantUserViewModel
                (userAssistant,
                _userAssistantService,
                EventAggregator,
                // _unshareProfilePopupView,
                //_dialogWindowsService,
                // _inviteUserOrAddProfilesPopupService,
                // _deleteAssistantUserPopupView,
                _shareFoldersService,
                _toastNotificationService,
                _userProfileService)
            );

        OnPropertyChanged(nameof(ViewModels));
    }

    private void InitPaginator()
    {
        PaginatorViewModel = new PaginatorViewModel(_viewModels.Count);
        ViewModels.Offset = PaginatorViewModel.Skip;
        ViewModels.Limit = PaginatorViewModel.OnPageItems;
        TotalCount = PaginatorViewModel.TotalCount;
    }
    private void ShowOutOfLimitPopup()
    {
        Action<IUpgradePlanViewModel> initialize = viewModel =>
        {
            viewModel.LimitExceededText = "You have reached the maximum number of users.";
        };

        _dialogWindowsService.ShowDialogWindow
            (_upgradePlanPopupView, "USERS LIMIT REACHED", initialize);
    }
    private void SendLicenceKey(string emailAddress, string password)
    {
        var url = $"mailto:{emailAddress}?subject=Chameleon invitation&body=You’ve been invited to Chameleon. Your credentials:%0DEmail: {emailAddress}%0DKey: {password}%0D";
        ProcessesUtil.GoToUrlDefault(url);
    }
    private void OnUserAssistantDeleted(DeletedUserAssistantEventArgs args)
    {
        var itemToRemove = ViewModels
            .First(v => v.Id == args.Id);
        ViewModels.Remove(itemToRemove);

        _toastNotificationService.ShowSuccess($"{itemToRemove.Username} was deleted successfully");
    }
    private void OnUserAssistantSaved(SavedUserAssistantEventArgs args)
    {
        var userAssistant = new UserAssistant
        {
            Id = args.Id,
            UserName = args.AssistantName,
            EmailAddress = args.AssistantEmail,
            Password = args.Password,
            ProfileIds = args.ProfileIds,
            ProfilePermissionIds = args.ProfilePermissionids
        };

        var assistantUserViewModel = new AssistantUserViewModel(
            userAssistant,
            _userAssistantService,
            EventAggregator,
            // _unshareProfilePopupView,
            //_dialogWindowsService,
            // _inviteUserOrAddProfilesPopupService,
            // _deleteAssistantUserPopupView,
            _shareFoldersService,
            _toastNotificationService,
            _userProfileService);

        var isExist = ViewModels
            .Any(v => v.Id == userAssistant.Id);

        if (!isExist)
        {
            ViewModels.Add(assistantUserViewModel);
            SendLicenceKey(userAssistant.EmailAddress, userAssistant.Password);
        }

        if (args.ProfileIds?.Count > 0)
        {
            _toastNotificationService.ShowSuccess($"{args.ProfileIds.Count} profile(s) shared successfully");
        }

        if (args.FolderIds?.Count > 0)
        {
            _toastNotificationService.ShowSuccess($"{args.FolderIds.Count} folder(s) shared successfully");
        }
    }
    private void OnCreate(InviteUserAssistantEventArgs args)
    {
        try
        {
            _userAssistantService.Save(new UserAssistant
            {
                UserName = args.AssistantName,
                EmailAddress = args.AssistantEmail,
                ProfileIds = args.ProfileIds,
                ProfilePermissionIds = args.ProfilePermissionIds,
                FolderIds = args.FolderIds,
                FolderPermissionIds = args.FolderPermissionIds
            });
        }
        catch
        {
            _toastNotificationService.ShowError($"Failed to invite the user. Please try again.");
        }
    }

    private void OnChangePage(object sender, EventArgs e)
    {
        ViewModels.Offset = PaginatorViewModel.Skip;
    }

    private void OnViewModelChange(object sender, EventArgs args)
    {
        var count = _viewModels.Items.Count;
        PaginatorViewModel.TotalCount = count;
        TotalCount = count;

        OnPropertyChanged(nameof(ViewModels));
    }
}
