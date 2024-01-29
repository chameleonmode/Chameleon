using System.Collections.Generic;
using System.Collections.Specialized;

namespace Chameleon.Interfaces.OutReach
{
    public interface IOutReachTemplates
        : IList<IOutReachTemplate>
        , INotifyCollectionChanged
    {
    }
}
