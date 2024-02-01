using Chameleon.Infrastructure.Api;
using Chameleon.Interfaces.Api;

namespace Chameleon.Infrastructure.UserProfileFolders
{
    public class UserProfileFolderApi
        : ApiLayer<UserProfileFolderDto, int, CreateUserProfileFolderDto, UserProfileFolderDto>
        , IUserProfileFolderApi
    {
        public UserProfileFolderApi(
            IApiClient apiClient
            ) : base(apiClient, "folder")
        {
        }
    }
}
