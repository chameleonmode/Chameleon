using Chameleon.Infrastructure.Dto;

namespace Chameleon.Infrastructure.App.Permissions.ProfileFolderPermission.Api.Dto
{
    public class ProfileFolderPermissionDto
         : IEntityDto<int>
    {
        public int Id { get; set; }
        public string PermissionName { get; set; }
        public string DisplayName { get; set; }
    }
}
