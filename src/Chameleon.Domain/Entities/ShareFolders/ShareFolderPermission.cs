using Chameleon.Interfaces.App.ShareFolders;

namespace Chameleon.Domain.Entities.ShareFolders
{
    public class ShareFolderPermission
        : IShareFolderPermission
    {
        public int PermissionId { get; set; }
        public string PermissionName { get; set; }
        public string DisplayName { get; set; }
        public bool IsGranted { get; set; }
    }
}
