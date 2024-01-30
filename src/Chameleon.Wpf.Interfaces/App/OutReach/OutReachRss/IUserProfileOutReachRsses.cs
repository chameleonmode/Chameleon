using System.Collections.Generic;
using System.Collections.Specialized;

namespace Chameleon.Interfaces.OutReach
{
    public interface IUserProfileOutReachRsses
        : IList<IUserProfileOutReachRss>
        , INotifyCollectionChanged
    {
        int ProfileId { get; }
    }
}
