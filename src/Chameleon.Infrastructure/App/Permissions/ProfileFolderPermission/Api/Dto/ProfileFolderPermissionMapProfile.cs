using AutoMapper;
using Chameleon.Interfaces.App.Permissions.ProfileFolderPermission;

namespace Chameleon.Infrastructure.App.Permissions.ProfileFolderPermission.Api.Dto
{
    public class ProfileFolderPermissionMapProfile 
        : Profile
    {
        public ProfileFolderPermissionMapProfile()
        {
            var map = CreateMap<ProfileFolderPermissionDto, IProfileFolderPermission>();

            map.ReverseMap();
        }
    }
}
