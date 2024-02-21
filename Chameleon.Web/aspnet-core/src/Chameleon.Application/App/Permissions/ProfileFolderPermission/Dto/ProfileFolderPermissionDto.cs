using Abp.AutoMapper;
using Chameleon.App.Entities.Permissions;
using Chameleon.App.Permissions.Dto.Base;

namespace Chameleon.App.Permissions.Dto
{
    [AutoMap(typeof(ProfilePermission))]
    public class ProfileFolderPermissionDto
        : ProfileFolderPermissionEntityDto
    {
    }
}
