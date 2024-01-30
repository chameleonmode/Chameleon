using Chameleon.Interfaces.Repository;

namespace Chameleon.Interfaces.App.Prospector
{
    public interface IUserProfileProspectorBlogsOfInterestRepository
       : IRepository<IUserProfileProspectorBlogsOfInterest, int, UserProfileGetAllRequestDto>
    {
    }
}
