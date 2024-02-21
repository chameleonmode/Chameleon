using Abp.Domain.Entities.Auditing;

namespace Chameleon.App.Entities
{
    public class BookmarkFile
        : FullAuditedEntity
    {
        public string Url { set; get; }
        public string Name { set; get; }

        public int? BookmarkId { set; get; }
    }
}
