using Chameleon.Authorization;
using Chameleon.CT.Common.Base;
using Chameleon.Core.Collections;
using Chameleon.Core.Collections.Views;
using Chameleon.Interfaces.App.ShareFolders;
using Chameleon.Interfaces.App.Users.AssistantUser.Events;
using Chameleon.Interfaces.Dialogs;
using Chameleon.Prism.Events;
using CommunityToolkit.Mvvm.Input;

namespace Chameleon.Avalonia.Controls.Settings.ViewModels.AssistantUsers;

public partial class AssistantUserFolderViewModel
      : AssistantViewModelBase
{
    private readonly IEventAggregator _eventAggregator;
    private readonly IShareFoldersService _shareFoldersService;
    private readonly IToastNotificationService _toastNotificationService;

    private ObservableCollection<IShareFolderPermission, AssistantFolderPermissionViewModel> _permissionMapping;

    public AssistantUserFolderViewModel(
        IEventAggregator eventAggregator,
        IShareFoldersService shareFoldersService,
        IToastNotificationService toastNotificationService,
        IShareFolder shareFolder)
    {
        _eventAggregator = eventAggregator;
        _shareFoldersService = shareFoldersService;
        _toastNotificationService = toastNotificationService;

        ShareFolder = shareFolder;

        InitPermissions();
    }

    public IShareFolder ShareFolder { get; }
    public override string Name => ShareFolder.FolderName;

    private ObservableCollectionView<AssistantFolderPermissionViewModel> _permissionViewModels;
    public ObservableCollectionView<AssistantFolderPermissionViewModel> PermissionViewModels
    {
        get
        {
            if (_permissionViewModels == null && _permissionMapping != null)
            {
                _permissionViewModels = new ObservableCollectionView<AssistantFolderPermissionViewModel>(_permissionMapping);
            }
            return _permissionViewModels;
        }
        set => SetProperty(ref _permissionViewModels, value);
    }

    public AssistantFolderPermissionViewModel Outreach =>
        PermissionViewModels
            .Single(a => a.PermissionName == PermissionNames.Pages_Outreach);

    public AssistantFolderPermissionViewModel Prospector =>
        PermissionViewModels
            .Single(a => a.PermissionName == PermissionNames.Pages_Prospector);

    public AssistantFolderPermissionViewModel YouTube =>
        PermissionViewModels
            .Single(a => a.PermissionName == PermissionNames.Pages_YouTube);

    public AssistantFolderPermissionViewModel RSS =>
        PermissionViewModels
            .Single(a => a.PermissionName == PermissionNames.Pages_RSS);

    public AssistantFolderPermissionViewModel Curate =>
        PermissionViewModels
            .Single(a => a.PermissionName == PermissionNames.Pages_Curate);

    public override void Unshare()
    {
        _eventAggregator
            .GetEvent<UnshareFolderEvent>()
            .Publish(new UnshareFolderEventArgs(ShareFolder));

        base.Unshare();
    }

    private void InitPermissions()
    {
        _permissionMapping = new ObservableCollection<IShareFolderPermission, AssistantFolderPermissionViewModel>(
            ShareFolder.FolderPermissions, folderPermission => new AssistantFolderPermissionViewModel(
                _shareFoldersService,
                _toastNotificationService,
                folderPermission,
                ShareFolder)
            );
    }
}
