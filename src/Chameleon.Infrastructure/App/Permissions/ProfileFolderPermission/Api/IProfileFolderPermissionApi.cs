using Chameleon.Infrastructure.Api;
using Chameleon.Infrastructure.App.Permissions.ProfileFolderPermission.Api.Dto;
using Chameleon.Interfaces.Ioc;

namespace Chameleon.Infrastructure.App.Permissions.ProfileFolderPermission.Api
{
    public interface IProfileFolderPermissionApi
         : IApiLayer<ProfileFolderPermissionDto>
         , ISingletonDependency
    {
    }
}
