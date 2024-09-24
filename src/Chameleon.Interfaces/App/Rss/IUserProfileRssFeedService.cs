using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.Repository;

namespace Chameleon.Interfaces.Rss
{
    public interface IUserProfileRssFeedService
        : Chameleon.lib.Common.Interfaces.Systemics.ISingletonDependency
    {
        IUserProfileRssFeeds GetAll(int profileId, bool ignoreCache = false);
        void Save(UpsertItems<IUserProfileRssFeed> result);
    }
}
