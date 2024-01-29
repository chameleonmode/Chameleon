using Chameleon.Interfaces.Repository;

namespace Chameleon.Interfaces.UserProfiles.Additional
{
    public interface IUserProfileLoginRepository 
        : IRepository<IUserProfileLogin, int, UserProfileGetAllRequestDto>
    {
    }
}
