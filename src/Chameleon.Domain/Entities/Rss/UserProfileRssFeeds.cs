using Chameleon.Interfaces.Repository;
using Chameleon.Interfaces.Rss;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Chameleon.Domain.Entities
{
    public class UserProfileRssFeeds
        : ObservableCollection<IUserProfileRssFeed>
        , IUserProfileRssFeeds
    {
        public UserProfileRssFeeds(
            int profileId,
            IEnumerable<IUserProfileRssFeed> collection
            ) : base(collection)
        {
            ProfileId = profileId;
        }

        public int ProfileId { get; }

        public UpsertItems<IUserProfileRssFeed> Reset(IEnumerable<Uri> urls)
        {
            var items = urls.ToList();
            var itemsCount = items.Count;
            
            var result = new UpsertItems<IUserProfileRssFeed>();
            result.Deleted.AddRange(
                RemoveAllAfter(itemsCount)
                );

            for (var i = 0; i < items.Count; ++i)
            {
                var url = items[i].ToString();
                if (i == Items.Count)
                {
                    var item = new UserProfileRssFeed
                    {
                        Url = url,
                        ProfileId = ProfileId
                    };
                    Add(item);
                    result.Inserted.Add(item);
                }
                else
                {
                    var item = Items[i];
                    item.Url = url;
                    result.Updated.Add(item);
                }
            }
            return result;
        }

        private List<IUserProfileRssFeed> RemoveAllAfter(int index)
        {
            var removed = new List<IUserProfileRssFeed>();
            while (index < Items.Count)
            {
                var itemIndex = Items.Count - 1;
                var item = Items[itemIndex];
                Items.RemoveAt(itemIndex);
                removed.Add(item);
            }
            return removed;
        }
    }
}
