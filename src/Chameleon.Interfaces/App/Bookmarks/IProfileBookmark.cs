using Chameleon.Interfaces.UserProfiles;
using System.Collections.Generic;

namespace Chameleon.Interfaces.Bookmarks
{
    public interface IProfileBookmark
         : IUserProfileRequiredEntity
    {
        string Url { get; set; }
        string Name { get; set; }
        string SiteName { get; set; }
        BookmarkType BookmarkType { get; set; }
        IList<IBookmarkFile> BookmarkFiles { set; get; }
    }
}
