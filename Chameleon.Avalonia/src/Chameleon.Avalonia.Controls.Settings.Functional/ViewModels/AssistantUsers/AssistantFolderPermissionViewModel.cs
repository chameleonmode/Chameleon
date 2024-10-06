using Chameleon.CT.Common.Base;
using Chameleon.Interfaces.App.ShareFolders;
using Chameleon.Interfaces.Dialogs;
using Chameleon.lib.Common.ServiceManagers;

using CommunityToolkit.Mvvm.Input;

namespace Chameleon.Avalonia.Controls.Settings.ViewModels.AssistantUsers;

public partial class AssistantFolderPermissionViewModel
       : SubPageViewModelBase
{
    private readonly IShareFoldersService _shareFoldersService;

    public AssistantFolderPermissionViewModel(
        IShareFoldersService shareFoldersService,
        IShareFolderPermission shareFolderPermission,
        IShareFolder shareFolder)
    {
        _shareFoldersService = shareFoldersService;

        ShareFolderPermission = shareFolderPermission;
        ShareFolder = shareFolder;
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
                OnPropertyChanged(nameof(IsGranted));
            }
        }
    }

    [RelayCommand]
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

            Toaster.ShowSuccess($"{ShareFolderPermission.DisplayName} was updated successfully");
        }
        catch
        {
            IsGranted = !IsGranted;

			Toaster.ShowErr($"{ShareFolderPermission.DisplayName} update failed. Please try again.");
        }
    }
}
