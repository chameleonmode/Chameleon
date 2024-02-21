using Abp.Domain.Entities.Auditing;
using System.Collections.Generic;

namespace Chameleon.App.Entities
{
    public class Bookmark
        : FullAuditedEntity
        , IMustHaveProfile
    {
        public string Url { get; set; }
        public string Name { get; set; }
        public BookmarkType BookmarkType { get; set; }

        public int ProfileId { get; set; }
        public virtual Profile Profile { get; set; }

        public virtual IList<BookmarkFile> BookmarkFiles { set; get; }
    }
}
