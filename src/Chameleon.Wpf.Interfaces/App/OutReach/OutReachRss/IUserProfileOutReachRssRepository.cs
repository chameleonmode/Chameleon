using Chameleon.Interfaces.Repository;

namespace Chameleon.Interfaces.OutReach
{
    public interface IUserProfileOutReachRssRepository
        : IRepository<IUserProfileOutReachRss, int, UserProfileGetAllRequestDto>
    {
    }
}
