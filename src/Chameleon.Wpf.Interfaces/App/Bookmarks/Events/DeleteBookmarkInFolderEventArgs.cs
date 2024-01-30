using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chameleon.Interfaces.Bookmarks
{
    public class DeleteBookmarkInFolderEventArgs : EventArgs
    {
        public IBookmarkFile BookmarkFile { get; }
        public IProfileBookmark ProfileBookmark { get; }
        public DeleteBookmarkInFolderEventArgs(
            IBookmarkFile bookmarkFile,
            IProfileBookmark profileBookmark
            )
        {
            BookmarkFile = bookmarkFile;
            ProfileBookmark = profileBookmark;
        }
    }
}
