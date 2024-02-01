using System.Collections.Generic;
using System.Collections.Specialized;

namespace Chameleon.Interfaces.App.Prospector
{
    public interface IUserProfileProspectorBlogsOfInterests
        : IList<IUserProfileProspectorBlogsOfInterest>
        , INotifyCollectionChanged
    {
        int ProfileId { get; }
    }
}
