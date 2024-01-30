using Chameleon.Interfaces.Entities;

namespace Chameleon.Interfaces.App.Permissions.ProfileFolderPermission
{
    public interface IProfileFolderPermission
         : IEntity<int>
    {
        string PermissionName { get; set; }
        string DisplayName { get; set; }
    }
}
