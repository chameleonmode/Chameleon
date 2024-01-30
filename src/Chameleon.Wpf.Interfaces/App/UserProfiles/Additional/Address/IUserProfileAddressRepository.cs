using Chameleon.Interfaces.Repository;

namespace Chameleon.Interfaces.UserProfiles.Additional
{
    public interface IUserProfileAddressRepository 
        : IRepository<IUserProfileAddress, int, UserProfileGetAllRequestDto>
    {
    }
}
