using System.Collections.Generic;
using System.Collections.Specialized;

namespace Chameleon.Interfaces.OutReach
{
    public interface IProfileOutReachLinks
        : IList<IProfileOutReachLink>
        , INotifyCollectionChanged
    {
        int ProfileId { get; }
    }
}
