using System.Collections.Generic;
using System.Collections.Specialized;

namespace Chameleon.Interfaces.Bookmarks
{
    public interface IProfileBookmarks
        : IList<IProfileBookmark>
        , INotifyCollectionChanged
    {
        int ProfileId { get; }
    }
}
