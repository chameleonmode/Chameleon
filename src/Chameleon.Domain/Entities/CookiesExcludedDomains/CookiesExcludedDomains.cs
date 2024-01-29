using Chameleon.Interfaces.CookiesExcluded;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Chameleon.Domain.Entities
{
    public class CookiesExcludedDomains
        : ObservableCollection<ICookiesExcludedDomain>
        , ICookiesExcludedDomains
    {
        public CookiesExcludedDomains(
           int profileId,
           IEnumerable<ICookiesExcludedDomain> collection
           ) : base(collection)
        {
            ProfileId = profileId;
        }

        public int ProfileId { get; }
    }
}
