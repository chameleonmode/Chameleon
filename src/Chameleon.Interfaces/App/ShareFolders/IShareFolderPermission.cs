namespace Chameleon.Interfaces.App.ShareFolders
{
    public interface IShareFolderPermission
    {
        int PermissionId { get; set; }
        string PermissionName { get; set; }
        string DisplayName { get; set; }
        bool IsGranted { get; set; }
    }
}
