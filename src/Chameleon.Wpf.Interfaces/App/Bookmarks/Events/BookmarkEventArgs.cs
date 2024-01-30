using System;

namespace Chameleon.Interfaces.Bookmarks
{
    public class BookmarkEventArgs : EventArgs
    {
        public IProfileBookmark ProfileBookmark { get; }

        public BookmarkEventArgs(IProfileBookmark profileBookmark)
        {
            ProfileBookmark = profileBookmark;
        }
    }
}
