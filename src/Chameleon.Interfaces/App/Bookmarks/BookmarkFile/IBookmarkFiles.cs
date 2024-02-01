using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chameleon.Interfaces.Bookmarks
{
    public interface IBookmarkFiles
        : IList<IBookmarkFile>
        , INotifyCollectionChanged
    {
    }
}
