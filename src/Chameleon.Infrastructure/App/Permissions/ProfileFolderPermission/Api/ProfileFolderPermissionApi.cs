using Chameleon.Infrastructure.Api;
using Chameleon.Infrastructure.App.Permissions.ProfileFolderPermission.Api.Dto;
using Chameleon.Interfaces.Api;

namespace Chameleon.Infrastructure.App.Permissions.ProfileFolderPermission.Api
{
    public class ProfileFolderPermissionApi
         : ApiLayer<ProfileFolderPermissionDto>
         , IProfileFolderPermissionApi
    {
        public ProfileFolderPermissionApi(
            IApiClient apiClient
            ) : base(apiClient, "ProfileFolderPermission")
        {
        }
    }
}
