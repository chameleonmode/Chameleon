using Chameleon.App.Entities;
using System.Collections.Generic;

namespace Chameleon.App
{
    public class BookmarkBaseDto
    {
        public string Url { get; set; }
        public string Name { get; set; }
        public BookmarkType BookmarkType { get; set; }
        public IList<BookmarkFile> BookmarkFiles { set; get; }

        [Identity]
        public int ProfileId { get; set; }
    }
}
