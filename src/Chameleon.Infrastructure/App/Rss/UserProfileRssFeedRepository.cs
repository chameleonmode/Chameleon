using AutoMapper;
using Chameleon.Domain.Entities;
using Chameleon.Infrastructure.Repositories;
using Chameleon.Interfaces.Repository;
using Chameleon.Interfaces.Rss;


namespace Chameleon.Infrastructure.Rss
{
    public class UserProfileRssFeedRepository
        : Repository<
            UserProfileRssFeed,
            IUserProfileRssFeed,
            int,
            UserProfileRssFeedDto,
            CreateUserProfileRssFeedDto
            >
        , IUserProfileRssFeedRepository
    {
        public UserProfileRssFeedRepository(
            IMapper mapper,
            IUserProfileRssFeedApi clientApi,
            IEventAggregator eventAggregator
            ) : base(mapper, clientApi, eventAggregator)
        {
        }

        public override IUserProfileRssFeed[] GetAll(GetAllRequestDto request = null)
        {
            _entities = null;
            return base.GetAll(request);
        }
    }
}
