using Chameleon.Domain.Entities;
using Chameleon.Interfaces.Rss;
using System.Collections.Generic;
using Chameleon.Interfaces.Repository;

namespace Chameleon.Infrastructure.Rss
{
    public class UserProfileRssFeedService 
        : IUserProfileRssFeedService
    {
        private readonly IUserProfileRssFeedRepository _repository;
        private readonly Dictionary<int, UserProfileRssFeeds> _entities 
            = [];

        public UserProfileRssFeedService(
            IUserProfileRssFeedRepository repository
            )
        {
            _repository = repository;
        }

        public IUserProfileRssFeeds GetAll(int profileId, bool ignoreCache = false)
        {
            if (ignoreCache)
            {
                _entities.Clear();
            }

            if (_entities.TryGetValue(profileId, out var feeds))
            {
                return feeds;
            }

            var request = new UserProfileGetAllRequestDto(profileId);
            var items = _repository.GetAll(true, request);
            feeds = new UserProfileRssFeeds(profileId, items);
            _entities[profileId] = feeds;
            return feeds;
        }

        public void Save(UpsertItems<IUserProfileRssFeed> feeds)
        {
            _repository.Upsert(feeds);
        }
    }
}
