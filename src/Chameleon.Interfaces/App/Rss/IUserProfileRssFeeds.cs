using Chameleon.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Chameleon.Interfaces.Rss
{
    public interface IUserProfileRssFeeds
        : IList<IUserProfileRssFeed>
        , INotifyCollectionChanged
    {
        int ProfileId { get; }
        UpsertItems<IUserProfileRssFeed> Reset(IEnumerable<Uri> urls);
    }
}
