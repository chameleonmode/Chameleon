using Chameleon.Infrastructure.Dto;

namespace Chameleon.Infrastructure.Bookmarks
{
    public class BookmarkFileDto
        : CreateBookmarkFileDto
        , IEntityDto<int>
    {
        public int Id { get; set; }

    }
}
