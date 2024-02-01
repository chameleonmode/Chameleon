using Chameleon.Interfaces.App.Permissions.ProfileFolderPermission;

namespace Chameleon.Domain.Entities.Permissions.ProfileFolderPermission
{
    public class ProfileFolderPermission
         : IProfileFolderPermission
    {
        public int Id { get; set; }
        public string PermissionName { get; set; }
        public string DisplayName { get; set; }
    }
}
