using System.Collections.Generic;
using System.Collections.Specialized;

namespace Chameleon.Interfaces.CookiesExcluded
{
    public interface ICookiesExcludedDomains
        : IList<ICookiesExcludedDomain>
        , INotifyCollectionChanged
    {
        int ProfileId { get; }
    }
}
