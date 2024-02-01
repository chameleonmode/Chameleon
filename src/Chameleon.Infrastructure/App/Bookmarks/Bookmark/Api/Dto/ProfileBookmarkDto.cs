using Chameleon.Infrastructure.Dto;
using Chameleon.Interfaces.Bookmarks;
using System.Collections.Generic;

namespace Chameleon.Infrastructure.Bookmarks
{
    public class ProfileBookmarkDto
        : CreateProfileBookmarkDto
        , IEntityDto<int>
    {
        public int Id { get; set; }
        public IList<BookmarkFileDto> BookmarkFiles { set; get; }
       
    }
}
