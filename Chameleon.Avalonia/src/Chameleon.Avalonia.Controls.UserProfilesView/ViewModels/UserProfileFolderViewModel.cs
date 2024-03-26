using Chameleon.Common.Helpers;
using Chameleon.CT.Common.Base;
using Chameleon.Interfaces.Auth;
using Chameleon.Interfaces.Dialogs;
using Chameleon.Interfaces.OutReach;
using Chameleon.Interfaces.UserProfileFolders;
using CommunityToolkit.Mvvm.Input;

namespace Chameleon.Avalonia.Controls.UserProfilesView.ViewModels;

public partial class UserProfileFolderViewModel : SubPageViewModelBase
{
    private readonly IApplicationUser _currentUser;
    private readonly IAuthSession _authSession;
    private readonly IUserProfileFolder _folder;
    private readonly IUserProfileFolderService _userProfileFolderService;

    private const string _dialogTitle = "DELETE FOLDER";

    public UserProfileFolderViewModel(
        IApplicationUser currentUser,
        IAuthSession authSession,
        IUserProfileFolder folder,
        IUserProfileFolderService userProfileFolderService
        )
    {
        _currentUser = currentUser;
        _authSession = authSession;
        _folder = folder;
        _userProfileFolderService = userProfileFolderService;

        EventAggregator
            .GetEvent<SavedUserProfileFolderEvent>()
            .Subscribe(OnFolderSaved);

        EventAggregator
            .GetEvent<OpenUserProfileFolderEvent>()
            .Subscribe(SetSelected);
    }

    public IUserProfileFolder UserProfileFolder => _folder;

   [RelayCommand]
    public void Open()
    {
        EventAggregator
            .GetEvent<OpenUserProfileFolderEvent>()
            .Publish(new UserProfileFolderEventArgs(_folder));
    }

    private void SetSelected(UserProfileFolderEventArgs args)
    {
        IsSelected = args.UserProfileFolder.Id == _folder.Id;
    }

    private void OnFolderSaved(UserProfileFolderEventArgs args)
    {
        if (args.UserProfileFolder != _folder)
        {
            return;
        }
    }

    [RelayCommand]
    private async Task Delete()
    {
        IsOpenMenuPopup = false;

        if (await MesageBoxHelper.ShowAsync(_dialogTitle,
            $"Are you sure you want to delete {UserProfileFolder.Title} folder? This will not affect individual profiles within the folder.",
            ContentDialogButtons.YesNo,
            "DeleteLines"))
            EventAggregator.PublishEvent<DeleteUserProfileFolderEvent, UserProfileFolderEventArgs>(new UserProfileFolderEventArgs(_folder));
                //.GetEvent<DeleteUserProfileFolderEvent>()
                //.Publish(new UserProfileFolderEventArgs(_folder));
    }

    [RelayCommand]
    private void OpenMenu()
    {
        IsOpenMenuPopup = !IsOpenMenuPopup;
    }

    private bool _isOpenMenuPopup;
    public bool IsOpenMenuPopup
    {
        get => _isOpenMenuPopup;
        set => SetProperty(ref _isOpenMenuPopup, value);
    }

    private string _title;
    public new string Title
    {
        get => _title;
        set
        {
            IsRenamed = false;

            if (!SetProperty(ref _title, value))
            {
                return;
            }

            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            var orignalTitle = _folder.Title;
            try
            {
                _folder.Title = _title;
                _userProfileFolderService.Save(_folder);

                EventAggregator
                    .GetEvent<RenameFolderEvent>()
                    .Publish(new RenameFolderEventArgs(_folder.Id, _folder.Title));
            }
            catch
            {
                _folder.Title = orignalTitle;
            }

            Title = string.Empty;
        }
    }

   [RelayCommand]
    private void StartRename()
    {
        IsOpenMenuPopup = false;
        IsRenamed = true;
    }

    [RelayCommand]
    private void ChangeProxies()
    {
        EventAggregator
            .GetEvent<OpenChangeProxiesEvent>()
            .Publish(new OpenChangeProxiesEventArgs(_folder.Id));
    }

    [RelayCommand]
    private void SaveRename()
    {
        IsRenamed = false;
    }

    private bool _isSelected;
    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            SetProperty(ref _isSelected, value);
            if (value == false)
            {
                IsRenamed = false;
            }
        }
    }

    private bool _isRenamed;
    public bool IsRenamed
    {
        get => _isRenamed;
        set => SetProperty(ref _isRenamed, value);
    }

    private bool _isFavorite;
    public bool IsFavorite
    {
        get => _isFavorite;
        set => SetProperty(ref _isFavorite, value);
    }

    public IApplicationUser CurrentUser => _currentUser;
    public bool IsSharedFolder => _userProfileFolderService.IsSharedFolder(_folder);
    public bool IsContextMenuItemEnabled => !CurrentUser.IsAssistant;
}
