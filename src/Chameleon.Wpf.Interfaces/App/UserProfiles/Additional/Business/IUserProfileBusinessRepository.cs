using Chameleon.Interfaces.Repository;

namespace Chameleon.Interfaces.UserProfiles.Additional
{
    public interface IUserProfileBusinessRepository
        : IRepository<IUserProfileBusiness, int, UserProfileGetAllRequestDto>
    {
    }
}
