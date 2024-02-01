using Chameleon.Interfaces.OutReach;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Chameleon.Domain.Entities
{
    public class UserProfileOutReachRsses
        : ObservableCollection<IUserProfileOutReachRss>
        , IUserProfileOutReachRsses
    {
        public UserProfileOutReachRsses(
            int profileId,
            IEnumerable<IUserProfileOutReachRss> collection
            ) : base(collection)
        {
            ProfileId = profileId;
        }

        public int ProfileId { get; }
    }
}
