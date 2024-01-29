using Chameleon.Interfaces.Bookmarks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chameleon.Domain.Entities
{
    public class ProfileBookmark : IProfileBookmark
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Name { get; set; }
        public string SiteName { get; set; }
        public BookmarkType BookmarkType { get; set; }
        public IList<IBookmarkFile> BookmarkFiles { get; set; }
        public int ProfileId { get; set; }
        public bool IsShowArrow => BookmarkType == BookmarkType.GlobalFolder ||
                          BookmarkType == BookmarkType.ProfileFolder;

        public override string ToString()
        {
            return Name;
        }
    }
}
