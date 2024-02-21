using Abp.Application.Services.Dto;

namespace Chameleon.App.Permissions.Dto.Base
{
    public class ProfileFolderPermissionEntityDto
        : ProfileFolderPermissionBaseDto
        , IEntityDto
        , IEntityDto<int>
    {
        public int Id { get; set; }
    }
}
