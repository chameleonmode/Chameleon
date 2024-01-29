using Chameleon.Interfaces.Repository;

namespace Chameleon.Interfaces.UserProfiles.Additional
{
    public interface IUserProfilePersonRepository 
        : IRepository<IUserProfilePerson, int, UserProfileGetAllRequestDto>
    {
    }
}
