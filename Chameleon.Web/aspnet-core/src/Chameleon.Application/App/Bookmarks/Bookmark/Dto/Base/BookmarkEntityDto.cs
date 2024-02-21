using Abp.Application.Services.Dto;

namespace Chameleon.App
{
    public class BookmarkEntityDto
     : BookmarkBaseDto
        , IEntityDto
        , IEntityDto<int>
    {
        public int Id { get; set; }
    }
}
