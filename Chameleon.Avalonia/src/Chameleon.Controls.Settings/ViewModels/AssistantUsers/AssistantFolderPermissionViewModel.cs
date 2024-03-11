using Chameleon.Avalonia.Prism.Module.Base;
using Chameleon.Interfaces.App.ShareFolders;
using Chameleon.Interfaces.Dialogs;
using Prism.Commands;

namespace Chameleon.Avalonia.Controls.Settings.ViewModels.AssistantUsers;

public class AssistantFolderPermissionViewModel
       : ViewModelBase
{
    private readonly IShareFoldersService _shareFoldersService;
    private readonly IToastNotificationService _toastNotificationService;

    public AssistantFolderPermissionViewModel(
        IShareFoldersService shareFoldersService,
        IToastNotificationService toastNotificationService,
        IShareFolderPermission shareFolderPermission,
        IShareFolder shareFolder)
    {
        _shareFoldersService = shareFoldersService;
        _toastNotificationService = toastNotificationService;

        ShareFolderPermission = shareFolderPermission;
        ShareFolder = shareFolder;

        UpdatePermissionCommand = new DelegateCommand(UpdatePermission);
    }

    public IShareFolderPermission ShareFolderPermission { get; }
    public IShareFolder ShareFolder { get; }

    public string PermissionName => ShareFolderPermission.PermissionName;
    public bool IsGranted
    {
        get => ShareFolderPermission.IsGranted;
        set
        {
            if (ShareFolderPermission.IsGranted != value)
            {
                ShareFolderPermission.IsGranted = value;
                RaisePropertyChanged(nameof(IsGranted));
            }
        }
    }

    public DelegateCommand UpdatePermissionCommand { get; }
    private void UpdatePermission()
    {
        try
        {
            if (IsGranted)
            {
                _shareFoldersService.AddPermission(ShareFolder.Id, ShareFolderPermission.PermissionId);
            }
            else
            {
                _shareFoldersService.DeletePermission(ShareFolder.Id, ShareFolderPermission.PermissionId);
            }

            _toastNotificationService.ShowSuccess($"{ShareFolderPermission.DisplayName} was updated successfully");
        }
        catch
        {
            IsGranted = !IsGranted;

            _toastNotificationService.ShowError($"{ShareFolderPermission.DisplayName} update failed. Please try again.");
        }
    }
}
