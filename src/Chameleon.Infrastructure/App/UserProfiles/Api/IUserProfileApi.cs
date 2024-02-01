using Chameleon.Infrastructure.Api;
using Chameleon.Interfaces.Ioc;

namespace Chameleon.Infrastructure.Profiles
{
    public interface IUserProfileApi
        : IApiLayer<UserProfileDto, int, CreateUserProfileDto, UserProfileDto>
        , ISingletonDependency
    {
        UserProfileDto[] GetAllByUserId(int id);
        void Execute(string endpoint, object query = null);
    }
}
