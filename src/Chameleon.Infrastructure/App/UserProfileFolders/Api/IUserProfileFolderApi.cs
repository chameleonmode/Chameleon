using Chameleon.Infrastructure.Api;
using Chameleon.Interfaces.Ioc;

namespace Chameleon.Infrastructure.UserProfileFolders
{
    public interface IUserProfileFolderApi 
        : IApiLayer<UserProfileFolderDto, int, CreateUserProfileFolderDto, UserProfileFolderDto>
        , ISingletonDependency
    {
    }
}
