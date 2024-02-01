using Chameleon.Interfaces.OutReach;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Chameleon.Domain.Entities
{
    public class ProfileOutReachLinks
         : ObservableCollection<IProfileOutReachLink>
        , IProfileOutReachLinks
    {
        public ProfileOutReachLinks(IEnumerable<IProfileOutReachLink> collection) 
            : base(collection)
        {}

        public ProfileOutReachLinks(
           int profileId,
           IEnumerable<IProfileOutReachLink> collection
           ) : base(collection)
        {
            ProfileId = profileId;
        }

        public int ProfileId { get; }
    }
}
