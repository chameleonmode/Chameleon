
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Chameleon.lib.Common.Interfaces.Sys;

namespace Chameleon.Interfaces.Bookmarks {
	public class DeleteBookmarkInFolderEvent
        : PubSubEvent<DeleteBookmarkInFolderEventArgs>
    {
    }
}
