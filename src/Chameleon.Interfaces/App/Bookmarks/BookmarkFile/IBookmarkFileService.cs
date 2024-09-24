using Chameleon.Interfaces.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chameleon.Interfaces.Bookmarks
{
    public interface IBookmarkFileService
        : Chameleon.lib.Common.Interfaces.Systemics.ISingletonDependency
    {
        IBookmarkFiles GetAll();
        IBookmarkFile Update(IBookmarkFile bookmarkFile);
        void Delete(IBookmarkFile bookmarkFile);
        IBookmarkFile Save(IBookmarkFile bookmarkFile);
    }
}
